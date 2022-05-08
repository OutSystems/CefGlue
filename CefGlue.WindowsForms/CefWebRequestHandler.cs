namespace Xilium.CefGlue.WindowsForms
{
    sealed class CefWebRequestHandler : CefRequestHandler
    {
        private readonly CefWebBrowser _core;

        public CefWebRequestHandler(CefWebBrowser core)
        {
            _core = core;
        }

        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            _core.InvokeIfRequired(() => _core.OnRenderProcessTerminated(new RenderProcessTerminatedEventArgs(status)));
        }
    }
}
