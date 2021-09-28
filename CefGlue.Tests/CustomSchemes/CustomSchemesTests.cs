using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace CefGlue.Tests.CustomSchemes
{
    public class CustomSchemesTests : TestBase
    {
        [Test]
        public async Task LoadCustomScheme()
        {
            var loadTask = Browser.AwaitLoad();

            Browser.Address = $"{CustomSchemeHandlerFactory.SchemeName}://testdomain/test";
            await loadTask;
        }

        [Test]
        public async Task JavascriptExecutionEngine_DisposeBrowser_ShouldHandleInnerTaskCanceledExceptions()
        {
            // Arrange
            Exception exception = null;
            var t = Browser.EvaluateJavaScript<string>("function ff(){ let finishDate = new Date((new Date()).getTime() + 10000); let sum = 0; while(new Date() < finishDate) {sum++;} return sum;} ff()");

            var t1 = Task.Run(async () => {
                try
                {
                    await t;
                }
                catch (Exception e)
                {
                    exception = e;
                }
            });

            // Act
            var t2 = Task.Run(() => Browser.Dispose());
            Task.WaitAll(t1, t2);

            // Assert
            Assert.IsNull(exception);
        }
    }
}
