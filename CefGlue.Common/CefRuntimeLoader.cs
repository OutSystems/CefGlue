using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Common
{
    public static class CefRuntimeLoader
    {
        private const string DefaultBrowserProcessDirectory = "CefGlueBrowserProcess";
        
        private static void InternalInitialize(CefSettings settings = null, KeyValuePair<string, string>[] flags = null, CustomScheme[] customSchemes = null, BrowserProcessHandler browserProcessHandler = null)
        {
            CefRuntime.Load();

            if (settings == null)
            {
                settings = new CefSettings();
            }


            var basePath = AppContext.BaseDirectory;
            var probingPaths = GetSubProcessPaths(basePath);
            var subProcessPath = probingPaths.FirstOrDefault(File.Exists);
            if (subProcessPath == null)
                throw new FileNotFoundException($"Unable to find SubProcess. Probed locations: {string.Join(Environment.NewLine, probingPaths)}");

            settings.BrowserSubprocessPath = subProcessPath;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                case CefRuntimePlatform.Linux:
                    settings.NoSandbox = true;
                    settings.MultiThreadedMessageLoop = true;
                    break;
            }
            
            IsOSREnabled = settings.WindowlessRenderingEnabled;

            // On Linux, with osr disable, the filename in CefMainArgs will be used as accessible name.
            // If the name is empty, chromium will crash at ui::AXNodeData:SetNamechecked.
            var exeFileName = Process.GetCurrentProcess().MainModule.FileName;
            if (string.IsNullOrEmpty(exeFileName))
            {
                exeFileName = "CefGlue";
            }

            CefRuntime.Initialize(new CefMainArgs([exeFileName]), settings, new BrowserCefApp(customSchemes, flags, browserProcessHandler), IntPtr.Zero);
        }

        private static IEnumerable<string> GetSubProcessPaths(string baseDirectory)
        {
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);

            // The executing DLL might not be in the current domain directory (plugins scenario)
            baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);
        }

        public static void Load(BrowserProcessHandler browserProcessHandler = null)
        {
            InternalInitialize(browserProcessHandler: browserProcessHandler);
        }

        public static bool IsLoaded => CefRuntime.IsInitialized;

        internal static bool IsOSREnabled { get; private set; }

        private static string BrowserProcessFileName
        {
            get
            {
                const string Filename = "Xilium.CefGlue.BrowserProcess";
                switch (CefRuntime.Platform)
                {
                    case CefRuntimePlatform.Windows:
                        return Filename + ".exe";
                    default:
                        return Filename;
                }
            }
        }
    }
}
