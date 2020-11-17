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
                if (e.Frame.Url.StartsWith("data"))
                {
                    UnsubcribeEvents();
                    taskCompletionSource.SetResult(true);
                }
            }

            browser.LoadEnd += OnBrowserLoadEnd;
            browser.LoadError += OnBrowserLoadError;

            var url = "data:text/html;charset=utf-8;base64," + Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(content));
            browser.Address = url;
            return taskCompletionSource.Task;
        }


    }
}
