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
            try
            {
                var loadTask = Browser.AwaitLoad();

                Browser.Address = $"{CustomSchemeHandlerFactory.SchemeName}://testdomain/test";
                await Task.WhenAny(Task.Delay(5000), loadTask);
            }
            catch (Exception e)
            {
                var message = "Fail to load url" + Environment.NewLine +
                    $"Current url: {Browser.Address}" + Environment.NewLine +
                    $"Initialized: {Browser.IsBrowserInitialized}." + Environment.NewLine +
                    $"Loading: {Browser.IsLoading}";
                Console.WriteLine(e.ToString());
                Console.WriteLine(message);
                Assert.Fail(message);
            }
        }
    }
}
