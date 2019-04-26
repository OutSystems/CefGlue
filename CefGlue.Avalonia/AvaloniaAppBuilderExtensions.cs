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

                var mainArgs = new CefMainArgs(args);

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

                var exitCode = CefRuntime.ExecuteProcess(mainArgs, cefApp, IntPtr.Zero);
                if (exitCode != -1)
                {
                    return;
                }

                var cefSettings = new CefSettings
                {
                    WindowlessRenderingEnabled = true,
                };

                switch (CefRuntime.Platform)
                {
                    case CefRuntimePlatform.Windows:
                        var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                        path = Regex.Replace(path, ".dll$", ".exe");
                        cefSettings.BrowserSubprocessPath = path;
                        cefSettings.MultiThreadedMessageLoop = true;
                        break;

                    case CefRuntimePlatform.MacOSX:
                        cefSettings.MultiThreadedMessageLoop = false;
                        cefSettings.ExternalMessagePump = true;
                        break;
                }

                CefRuntime.Initialize(mainArgs, cefSettings, cefApp, IntPtr.Zero);

                // TODO call shutdown when on exit
            });
        }
    }          
}
