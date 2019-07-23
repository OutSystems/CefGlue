using Avalonia.Controls;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Avalonia
{
    public static class AvaloniaAppBuilderExtensions
    {
        public static T ConfigureCefGlue<T>(this T builder, string[] args, CefSettings settings = null, CustomScheme[] customSchemes = null) where T : AppBuilderBase<T>, new()
        {
            BrowserProcessHandler browserProcessHandler = null;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    break;
                case CefRuntimePlatform.MacOSX:
                    browserProcessHandler = new AvaloniaBrowserProcessHandler();
                    break;
            }
            
            return builder.AfterSetup(_ => CefRuntimeLoader.Initialize(args, settings, customSchemes, browserProcessHandler));
        }
    }          
}
