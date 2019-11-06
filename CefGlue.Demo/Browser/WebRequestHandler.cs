namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    class WebRequestHandler : CefRequestHandler
    {
        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }

        protected override bool OnBeforeBrowse(CefBrowser browser, CefFrame frame, CefRequest request, bool userGesture, bool isRedirect)
        {
            //DemoApp.BrowserMessageRouter.OnBeforeBrowse(browser, frame);
            return base.OnBeforeBrowse(browser, frame, request, userGesture, isRedirect);
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            //DemoApp.BrowserMessageRouter.OnRenderProcessTerminated(browser);
        }
    }
}
