using System;
using System.IO;
using System.Reflection;

namespace Xilium.CefGlue.Common
{
    public static class CefRuntimeLoader
    {
        public static void Initialize(string[] args, CefSettings settings = null, CefBrowserProcessHandler browserProcessHandler = null)
        {
            CefRuntime.Load();

            if (settings == null)
            {
                settings = new CefSettings();
            }

            settings.WindowlessRenderingEnabled = true;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                    path = Path.Combine(Path.GetDirectoryName(path), "Xilium.CefGlue.BrowserProcess.exe");
                    settings.BrowserSubprocessPath = path;
                    settings.MultiThreadedMessageLoop = true;
                    break;

                case CefRuntimePlatform.MacOSX:
                    settings.MultiThreadedMessageLoop = false;
                    settings.ExternalMessagePump = true;
                    break;
            }

            AppDomain.CurrentDomain.ProcessExit += delegate { CefRuntime.Shutdown(); };

            CefRuntime.Initialize(new CefMainArgs(args), settings, new CommonCefApp(browserProcessHandler), IntPtr.Zero);
        }

        public static bool IsInitialized => CefRuntime.IsInitialized;
    }
}
