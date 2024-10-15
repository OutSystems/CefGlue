using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Demo.WPF
{
    public partial class BrowserView : IDisposable
    {
        public BrowserView()
        {
            InitializeComponent();
            browser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
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

        private void OnBrowserLoadStart(object sender, LoadStartEventArgs e)
        {
            if (!e.Frame.IsMain)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                addressTextBox.Text = e.Frame.Url;
            }));
        }

        private void OnAddressTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                browser.Address = ((TextBox)sender).Text;
            }
        }

        public async void EvaluateJavascript()
        {
            var result = new StringWriter();

            result.Write(await browser.EvaluateJavaScript<string>("return \"Hello World!\""));

            result.Write("; " + await browser.EvaluateJavaScript<int>("return 1+1"));

            result.Write("; " + await browser.EvaluateJavaScript<bool>("return false"));

            result.Write ("; " + await browser.EvaluateJavaScript<double>("return 1.5+1.5"));

            result.Write("; " + await browser.EvaluateJavaScript<double>("return 3+1.5"));

            result.Write("; " + await browser.EvaluateJavaScript<DateTime>("return new Date()"));

            result.Write("; " + string.Join(", ", await browser.EvaluateJavaScript<object[]>("return [1, 2, 3]")));

            result.Write("; " + string.Join(", ", (await browser.EvaluateJavaScript<ExpandoObject>("return (function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));

            browser.ExecuteJavaScript($"alert(\"{result.ToString().Replace("\r\n", " | ").Replace("\"", "\\\"")}\")");
        }

        public void BindJavascriptObject()
        {
            const string TestObject = "dotNetObject";

            var obj = new BindingTestClass();
            browser.RegisterJavascriptObject(obj, TestObject, methodHandler: AsyncCallNativeMethod);

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
}