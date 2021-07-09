using CefGlue.Tests.CustomSchemes;
using NUnit.Framework;
using System.Threading.Tasks;

namespace CefGlue.Tests.State
{
    public class StateTests : TestBase
    {
        [Test]
        public async Task CantGoBackAfterFirstNavigation()
        {
            var loadTask = Browser.AwaitLoad();

            Browser.Address = $"{CustomSchemeHandlerFactory.SchemeName}://testdomain/test";
            await loadTask;
            Assert.IsFalse(Browser.CanGoBack);
        }
    }
}
