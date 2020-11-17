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
        public async Task JavascriptContextCreatedAndReleasedAreFired()
        {
            var contextCreatedCompletionSource = new TaskCompletionSource<bool>();
            var contextReleasedCompletionSource = new TaskCompletionSource<bool>();

            void OnJavascriptContextCreated(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextCreatedCompletionSource.SetResult(true);
            }

            void OnJavascriptContextReleased(object sender, JavascriptContextLifetimeEventArgs e)
            {
                contextReleasedCompletionSource.SetResult(true);
            }

            Browser.JavascriptContextCreated += OnJavascriptContextCreated;
            Browser.JavascriptContextReleased += OnJavascriptContextReleased;

            await Browser.LoadContent($"<script>1+1</script>");
            await contextCreatedCompletionSource.Task;

            await Browser.LoadContent($"<html/>");
            await contextReleasedCompletionSource.Task;
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
