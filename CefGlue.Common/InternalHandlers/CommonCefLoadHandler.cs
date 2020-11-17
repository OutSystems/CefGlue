namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonCefLoadHandler : CefLoadHandler
    {
        private readonly ICefBrowserHost _owner;

        public CommonCefLoadHandler(ICefBrowserHost owner)
        {
            _owner = owner;
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            _owner.HandleLoadingStateChange(browser, isLoading, canGoBack, canGoForward);
        }

        protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            _owner.HandleLoadError(browser, frame, errorCode, errorText, failedUrl);
        }

        protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
        {
            _owner.HandleLoadStart(browser, frame, transitionType);
        }

        protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
            _owner.HandleLoadEnd(browser, frame, httpStatusCode);
        }
    }
}
