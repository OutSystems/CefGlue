namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebLoadHandler : CefLoadHandler
    {
        private readonly WebBrowser _core;

        public WebLoadHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            _core.OnLoadingStateChanged(isLoading, canGoBack, canGoForward);
        }
    }
}
