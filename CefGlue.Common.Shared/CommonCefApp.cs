namespace Xilium.CefGlue.Common.Shared
{
    internal abstract class CommonCefApp : CefApp
    {
        private readonly CustomScheme[] _customSchemes;

        internal CommonCefApp(CustomScheme[] customSchemes = null)
        {
            _customSchemes = customSchemes;
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
