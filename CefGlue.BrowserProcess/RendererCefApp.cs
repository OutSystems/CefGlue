using Xilium.CefGlue.BrowserProcess.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.BrowserProcess
{
    internal class RendererCefApp : CefApp
    {
        private readonly CefRenderProcessHandler _renderProcessHandler = new RenderProcessHandler();
        private readonly CustomScheme[] _customSchemes;

        internal RendererCefApp(CustomScheme[] customSchemes)
        {
            _customSchemes = customSchemes;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        protected override void OnRegisterCustomSchemes(CefSchemeRegistrar registrar)
        {
            if (_customSchemes != null)
            {
                foreach (var scheme in _customSchemes)
                {
                    registrar.AddCustomScheme(scheme.SchemeName, scheme.Options);
                }
            }
        }
    }
}
