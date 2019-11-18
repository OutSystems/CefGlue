using System.Collections.Generic;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.InternalHandlers;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefApp : CefApp
    {
        private readonly CefBrowserProcessHandler _browserProcessHandler;
        private readonly CustomScheme[] _customSchemes;
        private readonly KeyValuePair<string, string>[] _flags;

        internal CommonCefApp(CustomScheme[] customSchemes = null, KeyValuePair<string, string>[] flags = null, BrowserProcessHandler browserProcessHandler = null)
        {
            _customSchemes = customSchemes;
            _browserProcessHandler = new CommonBrowserProcessHandler(browserProcessHandler, customSchemes);
            _flags = flags;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (string.IsNullOrEmpty(processType))
            {
                commandLine.AppendSwitch("disable-gpu", "1");
                commandLine.AppendSwitch("disable-gpu-compositing", "1");
                commandLine.AppendSwitch("enable-begin-frame-scheduling", "1");
                commandLine.AppendSwitch("disable-smooth-scrolling", "1");

                if (_flags != null)
                {
                    foreach (var flag in _flags)
                    {
                        if (!commandLine.HasSwitch(flag.Key))
                        {
                            commandLine.AppendSwitch(flag.Key, flag.Value);
                        }
                    }
                }
            }

        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return _browserProcessHandler;
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
