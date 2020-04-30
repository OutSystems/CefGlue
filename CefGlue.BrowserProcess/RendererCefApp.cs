using Xilium.CefGlue.BrowserProcess.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.BrowserProcess
{
    internal class RendererCefApp : CommonCefApp
    {
        private readonly CefRenderProcessHandler _renderProcessHandler = new RenderProcessHandler();

        internal RendererCefApp(CustomScheme[] customSchemes) : base(customSchemes)
        {
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }
    }
}
