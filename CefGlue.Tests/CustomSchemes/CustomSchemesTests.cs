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
            await loadTask;
        }
    }
}
