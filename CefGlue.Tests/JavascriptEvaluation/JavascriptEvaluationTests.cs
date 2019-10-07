using System.Threading.Tasks;
using CefGlue.Tests.Helpers;
using NUnit.Framework;

namespace CefGlue.Tests.JavascriptEvaluation
{
    public class JavascriptEvaluationTests
    {
        private GenericCefBrowser browser;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var taskSource = new TaskCompletionSource<object>();
            browser = new GenericCefBrowser();
            browser.BrowserInitialized += () => taskSource.SetResult(null);
            await taskSource.Task;
        }

        [Test]
        public async Task Basic()
        {
            browser.LoadString("<script></script>", "about:blank");
            var result = await browser.EvaluateJavaScript<int>("(function() { return 1+1; })()");
        }
    }
}
