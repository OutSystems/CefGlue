using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CefGlue.Tests.Javascript
{
    public class NativeObjectUnregistrationTests : TestBase
    {
        private const string ReporterObjName = "reporterObj";
        private const string TempObjName = "tempObj";

        protected class ReporterObject
        {
            private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            public Task<object> ResultTask => _tcs.Task;

            public void SetResult(object result)
            {
                _tcs.TrySetResult(result);
            }
        }

        private ReporterObject reporter;

        protected override async Task ExtraSetup()
        {
            reporter = new ReporterObject();
            Browser.RegisterJavascriptObject(reporter, ReporterObjName);
            Browser.RegisterJavascriptObject(new object(), TempObjName);
            await Browser.LoadContent("<script></script>");
            await base.ExtraSetup();
        }

        [Test]
        public async Task BoundQueryOfUnregisteredObjectWaitsForReRegistration()
        {
            Browser.UnregisterJavascriptObject(TempObjName);

            // a stale completed query used to answer this bind with true for the deleted global
            Browser.ExecuteJavaScript($"cefglue.checkObjectBound('{TempObjName}').then(r => {ReporterObjName}.setResult('bound:' + r))");

            var earlyCompletion = await Task.WhenAny(reporter.ResultTask, Task.Delay(TimeSpan.FromSeconds(2)));
            Assert.AreNotSame(reporter.ResultTask, earlyCompletion, "a bound query for an unregistered object must not resolve against the deleted global");

            Browser.RegisterJavascriptObject(new object(), TempObjName);

            var result = await reporter.ResultTask;
            Assert.AreEqual("bound:true", result);
        }
    }
}
