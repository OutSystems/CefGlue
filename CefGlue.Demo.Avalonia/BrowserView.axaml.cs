using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public partial class BrowserView : UserControl
    {
        private AvaloniaCefBrowser browser;

        public BrowserView()
        {
            InitializeComponent();

            var browserWrapper = this.FindControl<Decorator>("browserWrapper");

            browser = new AvaloniaCefBrowser();
            browser.Address = "https://www.google.com";
            browser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
            browser.LoadStart += OnBrowserLoadStart;
            browser.TitleChanged += OnBrowserTitleChanged;
            browser.LifeSpanHandler = new BrowserLifeSpanHandler();
            browser.PermissionHandler = new BrowserPermissionHandler();
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

        private class BrowserLifeSpanHandler : LifeSpanHandler
        {
            protected override bool OnBeforePopup(
                CefBrowser browser,
                CefFrame frame,
                string targetUrl,
                string targetFrameName,
                CefWindowOpenDisposition targetDisposition,
                bool userGesture,
                CefPopupFeatures popupFeatures,
                CefWindowInfo windowInfo,
                ref CefClient client,
                CefBrowserSettings settings,
                ref CefDictionaryValue extraInfo,
                ref bool noJavascriptAccess)
            {
                var bounds = windowInfo.Bounds;
                Dispatcher.UIThread.Post(() =>
                {
                    var window = new Window();
                    var popupBrowser = new AvaloniaCefBrowser();
                    popupBrowser.Address = targetUrl;
                    window.Content = popupBrowser;
                    window.Position = new PixelPoint(bounds.X, bounds.Y);
                    window.Height = bounds.Height;
                    window.Width = bounds.Width;
                    window.Title = targetUrl;
                    window.Show();
                });
                return true;
            }
        }
        
        /// <summary>
        /// Simplified permission handler for browser requests.
        /// </summary>
        internal sealed class BrowserPermissionHandler : PermissionHandler
        {
            protected override bool OnRequestMediaAccessPermission(CefBrowser browser, CefFrame frame,
                string requestingOrigin,
                CefMediaAccessPermissionTypes requestedPermissions, CefMediaAccessCallback callback)
            {
                if (browser == null || callback == null) return false;

                // Grant all requested media permissions for all sites.
                callback.Continue(requestedPermissions);
                return true;
            }

            /// <summary>
            /// Called when a permission prompt is shown (e.g., clipboard, geolocation, notifications).
            /// Grants permissions for all sites. For testing, you can filter by checking requestingOrigin, 
            /// e.g., if (requestingOrigin.Contains("test-site.com")) to restrict to specific domains.
            /// </summary>
            /// <param name="browser">The browser instance initiating the prompt.</param>
            /// <param name="promptId">The unique ID of the permission prompt.</param>
            /// <param name="requestingOrigin">The origin requesting the permission.</param>
            /// <param name="requestedPermissions">The requested permission types.</param>
            /// <param name="callback">The callback to invoke with the permission result.</param>
            /// <returns>True if the prompt was handled, false otherwise.</returns>
            protected override bool OnShowPermissionPrompt(CefBrowser browser, ulong promptId, string requestingOrigin,
                CefPermissionRequestTypes requestedPermissions, CefPermissionPromptCallback callback)
            {
                if (browser == null || callback == null) return false;

                // Allow clipboard, geolocation, and notifications for all sites.
                var result = CefPermissionRequestResult.Deny;
                if (requestedPermissions.HasFlag(CefPermissionRequestTypes.Clipboard) ||
                    requestedPermissions.HasFlag(CefPermissionRequestTypes.Geolocation) ||
                    requestedPermissions.HasFlag(CefPermissionRequestTypes.Notifications))
                {
                    result = CefPermissionRequestResult.Accept;
                }

                callback.Continue(result);
                return true;
            }
        }
    }
}
