using System;
using NUnit.Framework;
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

            try
            {
                await loadTask.WaitAsync(TimeSpan.FromSeconds(5));
            }
            catch (TimeoutException)
            {
                Assert.Fail(
                    "Fail to load url" + Environment.NewLine +
                    $"Current url: {Browser.Address}" + Environment.NewLine +
                    $"Initialized: {Browser.IsBrowserInitialized}." + Environment.NewLine +
                    $"Loading: {Browser.IsLoading}");
            }
        }
    }
}
