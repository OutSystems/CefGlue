using Avalonia.Controls;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Avalonia
{
    public static class AvaloniaAppBuilderExtensions
    {
        public static T ConfigureCefGlue<T>(this T builder, string[] args) where T : AppBuilderBase<T>, new()
        {
            return builder.AfterSetup((b) =>
            {
                CefRuntime.Load();

                CefBrowserProcessHandler browserProcessHandler = null;

                switch (CefRuntime.Platform)
                {
                    case CefRuntimePlatform.Windows:
                        break;
                    case CefRuntimePlatform.MacOSX:
                        browserProcessHandler = new AvaloniaBrowserProcessHandler();
                        break;
                }

                var cefApp = new CommonCefApp(browserProcessHandler);
                
                cefApp.Run(args);
            });
        }
    }          
}
