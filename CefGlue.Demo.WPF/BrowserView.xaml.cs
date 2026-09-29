using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Demo.WPF
{
    public partial class BrowserView : IDisposable
    {
        public BrowserView()
        {
            InitializeComponent();
            //browser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
            //browser.PermissionHandler = new BrowserPermissionHandler();
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
            //Console.WriteLine(await browser.EvaluateJavaScript<string>("\"Hello World!\""));

            //Console.WriteLine(await browser.EvaluateJavaScript<int>("1+1"));

            //Console.WriteLine(await browser.EvaluateJavaScript<bool>("false"));

            //Console.WriteLine(await browser.EvaluateJavaScript<double>("1.5+1.5"));

            //Console.WriteLine(await browser.EvaluateJavaScript<double>("3+1.5"));

            //Console.WriteLine(await browser.EvaluateJavaScript<DateTime>("new Date()"));

            //Console.WriteLine(string.Join(", ", await browser.EvaluateJavaScript<object[]>("[1, 2, 3]")));

            //Console.WriteLine(string.Join(", ", (await browser.EvaluateJavaScript<ExpandoObject>("(function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));
        }

        public void BindJavascriptObject()
        {
            //const string TestObject = "dotNetObject";

            //var obj = new BindingTestClass();
            //browser.RegisterJavascriptObject(obj, "dotNetObject");

            //var methods = obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
            //                           .Where(m => m.GetParameters().Length == 0)
            //                           .Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1));

            //var script = "(function () {" +
            //    "let calls = [];" +
            //    //string.Join("", methods.Select(m => $"calls.push({{ name: '{m}', promise: {TestObject}.{m}() }});")) +
            //    $"calls.push({{ name: 'getObjectWithParams', promise: {TestObject}.getObjectWithParams(5, 'a string', {{ Name: 'obj name', Value: 10 }}, [ 1, 2 ]) }});" +
            //    "calls.forEach(c => c.promise.then(r => console.log(c.name + ': ' + JSON.stringify(r))).catch(e => console.log(e)));" +
            //    "})()";

            //browser.ExecuteJavaScript(script);
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