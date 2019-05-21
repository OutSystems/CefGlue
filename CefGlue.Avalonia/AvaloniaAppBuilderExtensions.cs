using Avalonia.Controls;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Avalonia
{
    public static class AvaloniaAppBuilderExtensions
    {
        public static T ConfigureCefGlue<T>(this T builder, string[] args) where T : AppBuilderBase<T>, new()
        {
            CefBrowserProcessHandler browserProcessHandler = null;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    break;
                case CefRuntimePlatform.MacOSX:
                    browserProcessHandler = new AvaloniaBrowserProcessHandler();
                    break;
            }

            var cefApp = new CommonCefApp(args, browserProcessHandler);

            cefApp.Prepare();

            return builder.AfterSetup(_ => cefApp.Run());
        }
    }          
}
