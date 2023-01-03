using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public class BrowserView : UserControl
    {
        private AvaloniaCefBrowser browser;

        public BrowserView()
        {
            AvaloniaXamlLoader.Load(this);

            var browserWrapper = this.FindControl<Decorator>("browserWrapper");

            browser = new AvaloniaCefBrowser();
            browser.Address = "https://www.google.com";
            browser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
            browser.LoadStart += OnBrowserLoadStart;
            browser.TitleChanged += OnBrowserTitleChanged;
            browserWrapper.Child = browser;
        }

        static Task<object> AsyncCallNativeMethod(Func<object> nativeMethod)
        {
            return Task.Run(() =>
            {
                var result = nativeMethod.Invoke();
                if (result is Task task)
                {
                    if (task.GetType().IsGenericType)
                    {
                        return ((dynamic) task).Result;
                    }

                    return task;
                }

                return result;
            });
        }

        public event Action<string> TitleChanged;

        private void OnBrowserTitleChanged(object sender, string title)
        {
            TitleChanged?.Invoke(title);
        }

        private void OnBrowserLoadStart(object sender, Common.Events.LoadStartEventArgs e)
        {
            if (e.Frame.Browser.IsPopup || !e.Frame.IsMain)
            {
                return;
            }

            Dispatcher.UIThread.Post(() =>
            {
                var addressTextBox = this.FindControl<TextBox>("addressTextBox");

                addressTextBox.Text = e.Frame.Url;
            });
        }

        private void OnAddressTextBoxKeyDown(object sender, global::Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                browser.Address = ((TextBox)sender).Text;
            }
        }

        public async void EvaluateJavascript()
        {
            Debug.WriteLine("BrowserView#EvaluateJavascript#Started", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff"));

            var contextCreatedCompletionSource = new TaskCompletionSource<bool>();
            var contextReleasedCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextCreatedCompletionSource.SetResult(true);
                browser.JavascriptContextCreated -= OnJavascriptContextCreated;
            }

            void OnJavascriptContextReleased(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextReleasedCompletionSource.SetResult(true);
                browser.JavascriptContextReleased -= OnJavascriptContextReleased;
            }

            browser.JavascriptContextCreated += OnJavascriptContextCreated;
            browser.JavascriptContextReleased += OnJavascriptContextReleased;

            await browser.LoadContent($"<script>1+1</script>");
            await contextCreatedCompletionSource.Task;

            await browser.LoadContent($"<html/>");
            await contextReleasedCompletionSource.Task;
            Debug.WriteLine("BrowserView#EvaluateJavascript#Finished", DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff"));
        }

        public void BindJavascriptObject()
        {
            const string TestObject = "dotNetObject";

            var obj = new BindingTestClass();
            browser.RegisterJavascriptObject(obj, TestObject, AsyncCallNativeMethod);

            var methods = obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                                       .Where(m => m.GetParameters().Length == 0)
                                       .Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1));

            var script = "(function () {" +
                "let calls = [];" +
                string.Join("", methods.Select(m => $"calls.push({{ name: '{m}', promise: {TestObject}.{m}() }});")) +
                $"calls.push({{ name: 'asyncGetObjectWithParams', promise: {TestObject}.asyncGetObjectWithParams('a string') }});" +
                $"calls.push({{ name: 'getObjectWithParams', promise: {TestObject}.getObjectWithParams(5, 'a string', {{ Name: 'obj name', Value: 10 }}, [ 1, 2 ]) }});" +
                "calls.forEach(c => c.promise.then(r => console.log(c.name + ': ' + JSON.stringify(r))).catch(e => console.log(e)));" +
                "})()";

            browser.ExecuteJavaScript(script);
        }

        public void OpenDevTools()
        {
            browser.ShowDeveloperTools();
        }

        public void Dispose()
        {
            browser.Dispose();
        }
    }

    public static class BrowserExtensions
    {
        public static Task LoadContent(this AvaloniaCefBrowser browser, string content)
        {
            var loadTask = browser.AwaitLoad();

            var url = "data:text/html;charset=utf-8;base64," + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(content));
            browser.Address = url;
            return loadTask;
        }

        public static Task AwaitLoad(this AvaloniaCefBrowser browser)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            void UnsubcribeEvents()
            {
                browser.LoadEnd -= OnBrowserLoadEnd;
                browser.LoadError -= OnBrowserLoadError;
            }

            void OnBrowserLoadError(object sender, LoadErrorEventArgs e)
            {
                UnsubcribeEvents();
                taskCompletionSource.SetException(new Exception(e.ErrorText));
            }

            void OnBrowserLoadEnd(object sender, LoadEndEventArgs e)
            {
                if (e.Frame.Url.StartsWith("data:") || e.Frame.Url.StartsWith("test:"))
                {
                    UnsubcribeEvents();
                    taskCompletionSource.SetResult(true);
                }
            }

            browser.LoadEnd += OnBrowserLoadEnd;
            browser.LoadError += OnBrowserLoadError;
            return taskCompletionSource.Task;
        }
    }
}