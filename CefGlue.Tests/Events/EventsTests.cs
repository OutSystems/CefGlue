using Avalonia.Controls;
using NUnit.Framework;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;

namespace CefGlue.Tests.Events
{
    public class EventsTests : TestBase
    {
        private const string Url = "data:";

        [Test]
        public async Task LoadStartIsFired()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            void OnBrowserLoadStart(object sender, LoadStartEventArgs e)
            {
                if (e.Frame.Url.StartsWith(Url))
                {
                    taskCompletionSource.SetResult(true);
                }
            }
            Browser.LoadStart += OnBrowserLoadStart;
            await Browser.LoadContent("<html/>");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task LoadEndIsFired()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            void OnBrowserLoadEnd(object sender, LoadEndEventArgs e)
            {
                if (e.Frame.Url.StartsWith(Url))
                {
                    taskCompletionSource.SetResult(true);
                }
            }
            Browser.LoadEnd += OnBrowserLoadEnd;
            await Browser.LoadContent("<html/>");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task LoadErrorIsFired()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            void OnBrowserLoadError(object sender, LoadErrorEventArgs e)
            {
                taskCompletionSource.SetResult(true);
            }
            Browser.LoadError += OnBrowserLoadError;
            Browser.Address = "http://0.0.0.0"; // navigate to an invalid url
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task LoadingStateChangeIsFired()
        {
            var completed = false;
            var loadStateChangeCount = 0;
            var taskCompletionSource = new TaskCompletionSource<bool>();
            void OnLoadingStateChange(object sender, LoadingStateChangeEventArgs e)
            {
                loadStateChangeCount++;
                if (loadStateChangeCount > 1 && !completed)
                {
                    completed = true;
                    taskCompletionSource.SetResult(true);
                }
            }
            Browser.LoadingStateChange += OnLoadingStateChange;
            await Browser.LoadContent("<html/>");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task AddressChangedIsFired()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            void OnAddressChanged(object sender, string address)
            {
                if (address.StartsWith(Url))
                {
                    taskCompletionSource.SetResult(true);
                }
            }
            Browser.AddressChanged += OnAddressChanged;
            await Browser.LoadContent("<html/>");
            await taskCompletionSource.Task;
        }

        [Test]
        public async Task ConsoleMessageIsFired()
        {
            const string ConsoleMessage = "this is a test";
            var taskCompletionSource = new TaskCompletionSource<string>();
            void OnConsoleMessage(object sender, ConsoleMessageEventArgs e)
            {
                taskCompletionSource.SetResult(e.Message);
            }
            Browser.ConsoleMessage += OnConsoleMessage;
            await Browser.LoadContent($"<html><script>console.log('{ConsoleMessage}')</script></html>");
            var message = await taskCompletionSource.Task;
            Assert.AreEqual(ConsoleMessage, message);
        }

        [Test]
        public async Task JavascriptContextCreatedIsFired()
        {
            var contextCreatedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextCreatedEventsCompletionSource.SetResult(true);
            }
            try
            {
                Browser.JavascriptContextCreated += OnJavascriptContextCreated;

                await Browser.LoadContent($"<script>1+1</script>");
                await contextCreatedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextCreated -= OnJavascriptContextCreated;
            }
        }

        [Test]
        public async Task JavascriptContextCreatedAreFiredWhenMultipleContextsCreated()
        {
            var mainFrameContextCreatedEventsCompletionSource = new TaskCompletionSource<bool>();
            var innerFrameContextCreatedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                if (e.Frame.IsMain)
                {
                    mainFrameContextCreatedEventsCompletionSource.SetResult(true);
                } else
                {
                    innerFrameContextCreatedEventsCompletionSource.SetResult(true);
                }
            }

            try
            {
                Browser.JavascriptContextCreated += OnJavascriptContextCreated;

                await Browser.LoadContent($"<html><body><iframe /></body></html>");
                await mainFrameContextCreatedEventsCompletionSource.Task;
                await innerFrameContextCreatedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextCreated -= OnJavascriptContextCreated;
            }
        }

        [Test]
        public async Task JavascriptContextCreatedAreFiredWhenLoadingNewContent()
        {
            var contextCreatedCalls = 0;
            var contextCreatedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                if (e.Frame.IsMain)
                {
                    contextCreatedCalls++;
                }

                if (contextCreatedCalls == 2)
                {
                    contextCreatedEventsCompletionSource.SetResult(true);
                }
            }

            try
            {
                Browser.JavascriptContextCreated += OnJavascriptContextCreated;

                await Browser.LoadContent($"<script>1+1</script>");
                await Browser.LoadContent($"<html/>");
                await contextCreatedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextCreated -= OnJavascriptContextCreated;
            }

        }

        [Test]
        public async Task JavascriptContextReleasedIsFiredWhenLoadingNewContent()
        {
            var contextReleasedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextReleased(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextReleasedEventsCompletionSource.SetResult(true);
            }

            try
            {
                Browser.JavascriptContextReleased += OnJavascriptContextReleased;

                await Browser.LoadContent($"<script>1+1</script>");
                await Browser.LoadContent($"<html/>");
                await contextReleasedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextReleased -= OnJavascriptContextReleased;
            }
        }

        [Test]
        public async Task JavascriptContextCreatedIsFiredWhenReloadingContent()
        {
            var contextCreatedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextCreatedEventsCompletionSource.SetResult(true);
            }

            try
            {
                await Browser.LoadContent($"<html/>");

                Browser.JavascriptContextCreated += OnJavascriptContextCreated;

                Browser.Reload();

                await contextCreatedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextCreated -= OnJavascriptContextCreated;
            }
        }

        [Test]
        public async Task JavascriptContextReleasedIsFiredWhenReloadingContent()
        {
            var contextReleasedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextReleased(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextReleasedEventsCompletionSource.SetResult(true);
            }

            try
            {
                await Browser.LoadContent($"<html/>");

                Browser.JavascriptContextReleased += OnJavascriptContextReleased;

                Browser.Reload();

                await contextReleasedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextReleased -= OnJavascriptContextReleased;
            }
        }

        [Test]
        public async Task JavascriptContextReleasedAreFiredWhenMultipleContextsReleased()
        {
            var mainFrameContextReleasedEventsCompletionSource = new TaskCompletionSource<bool>();
            var innerFrameContextReleasedEventsCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextReleased(object sender, JavascriptContextLifetimeEventArgs e)
            {
                if (e.Frame.IsMain)
                {
                    mainFrameContextReleasedEventsCompletionSource.SetResult(true);
                }
                else
                {
                    innerFrameContextReleasedEventsCompletionSource.SetResult(true);
                }
            }

            try
            {
                Browser.JavascriptContextReleased += OnJavascriptContextReleased;

                await Browser.LoadContent($"<html><body><iframe /></body></html>");
                await Browser.LoadContent($"<html/>");
                await mainFrameContextReleasedEventsCompletionSource.Task;
                await innerFrameContextReleasedEventsCompletionSource.Task;
            }
            finally
            {
                Browser.JavascriptContextReleased -= OnJavascriptContextReleased;
            }
        }

        [Test]
        public async Task JavascriptUncaughExceptionIsFired()
        {
            const string ExceptionMessage = "ups";
            var taskCompletionSource = new TaskCompletionSource<string>();
            void OnJavascriptUncaughException(object sender, JavascriptUncaughtExceptionEventArgs e)
            {
                taskCompletionSource.SetResult(e.Message);
            }
            Browser.JavascriptUncaughException += OnJavascriptUncaughException;
            await Browser.LoadContent($"<script>throw new Error('{ExceptionMessage}')</script>");
            var message = await taskCompletionSource.Task;
            StringAssert.Contains(ExceptionMessage, message);
        }
    }
}
