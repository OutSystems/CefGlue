using CefGlue.Tests.CustomSchemes;
using System;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common.Events;

namespace CefGlue.Tests
{
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
                if (e.Frame.Url.StartsWith("data:") || e.Frame.Url.StartsWith(CustomSchemeHandlerFactory.SchemeName + ":"))
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
