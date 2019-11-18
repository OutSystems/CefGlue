using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Common
{
    public static class CefRuntimeLoader
    {
        private static BrowserProcessHandler browserProcessHandler;

        internal static void RegisterBrowserProcessHandler(BrowserProcessHandler browserProcessHandler)
        {
            if (CefRuntime.IsInitialized)
            {
                throw new InvalidOperationException("Cannot register BrowserProcessHandler after cef runtime is initialized");
            }
            CefRuntimeLoader.browserProcessHandler = browserProcessHandler;
        }

        public static void Initialize(CefSettings settings = null, KeyValuePair<string, string>[] flags = null, CustomScheme[] customSchemes = null)
        {
            CefRuntime.Load();

            if (settings == null)
            {
                settings = new CefSettings();
            }

            settings.WindowlessRenderingEnabled = true;
            settings.UncaughtExceptionStackSize = 100; // for uncaught exception event work properly

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                    path = Path.Combine(Path.GetDirectoryName(path), "Xilium.CefGlue.BrowserProcess.exe");
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException($"Unable to find \"${path}\"");
                    }
                    settings.BrowserSubprocessPath = path;
                    settings.MultiThreadedMessageLoop = true;
                    break;

                case CefRuntimePlatform.MacOSX:
                    settings.MultiThreadedMessageLoop = false;
                    settings.ExternalMessagePump = true;
                    break;
            }

            AppDomain.CurrentDomain.ProcessExit += delegate { CefRuntime.Shutdown(); };

            CefRuntime.Initialize(new CefMainArgs(new string[0]), settings, new CommonCefApp(customSchemes, flags, browserProcessHandler), IntPtr.Zero);

            if (customSchemes != null)
            {
                foreach (var scheme in customSchemes)
                {
                    CefRuntime.RegisterSchemeHandlerFactory(scheme.SchemeName, scheme.DomainName, scheme.SchemeHandlerFactory);
                }
            }
        }

        public static bool IsInitialized => CefRuntime.IsInitialized;
    }
}
