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
        // skip base setup
        protected override Task Setup()
            => Task.CompletedTask;

        [Test]
        public async Task NotGlobalCefReqestContext()
        {
            var browser = () => new AvaloniaCefBrowser(new CefRequestContextSettings(), null);

            await InternalSetup(browser);

            Assert.False(Browser.RequestContext.IsGlobal);
        }

        [Test]
        public async Task CefReqestContextApplied()
        {
            var cachePath = @"C:/path/to/cache";

            var browser = () => new AvaloniaCefBrowser(new CefRequestContextSettings()
            {
                CachePath = cachePath,
            }, null);

            await InternalSetup(browser);

            var result = Browser.RequestContext.CachePath;

            Assert.AreEqual(cachePath, result);
        }
    }
}
