using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CefGlue.Tests.CustomSchemes;
using CefGlue.Tests.Helpers;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace CefGlue.Tests.RequestContext
{ 
    public class CustomRequestContext: TestBase
    {
        private class CustomCefRequestContextHandler : CefRequestContextHandler
        {
            protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
            {
                return null;
            }
        }

        // skip base setup
        protected override Task Setup()
            => Task.CompletedTask;

        [Test]
        public async Task NonGlobalCefRequestContext()
        {
            var browserFactory = () => new AvaloniaCefBrowser(() => CefRequestContext.CreateContext(new CefRequestContextSettings(), null));

            await InternalSetup(browserFactory);

            Assert.False(Browser.RequestContext.IsGlobal);
        }

        [Test]
        public async Task CefRequestContextApplied()
        {
            const string cachePath = @"C:/path/to/cache";
            var browserFactory = () => new AvaloniaCefBrowser(() => CefRequestContext.CreateContext(new CefRequestContextSettings()
            {
                CachePath = cachePath,
            }, null));

            await InternalSetup(browserFactory);

            var result = Browser.RequestContext.CachePath;

            Assert.AreEqual(cachePath, result);
        }

        [Test]
        public async Task CustomCefRequestContextHandlerApplied()
        {
            var customRequestContextHandler = new CustomCefRequestContextHandler();
            var browserFactory = () => new AvaloniaCefBrowser(() => CefRequestContext.CreateContext(new CefRequestContextSettings(), customRequestContextHandler));

            await InternalSetup(browserFactory);

            var result = Browser.RequestContext.GetHandler();

            Assert.AreEqual(customRequestContextHandler, result);
        }
    }
}
