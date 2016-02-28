namespace Xilium.CefGlue.WindowsForms
{
    sealed class CefWebRequestHandler : CefRequestHandler
    {
        private readonly CefWebBrowser _core;

        public CefWebRequestHandler(CefWebBrowser core)
        {
            _core = core;
        }

        protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
        {
            _core.InvokeIfRequired(() => _core.OnPluginCrashed(new PluginCrashedEventArgs(pluginPath)));
        }

        protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
            _core.InvokeIfRequired(() => _core.OnRenderProcessTerminated(new RenderProcessTerminatedEventArgs(status)));
        }
    }
}
