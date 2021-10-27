﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Common
{
    public static class CefRuntimeLoader
    {
        private const string DefaultBrowserProcessDirectory = "CefGlueBrowserProcess";
        
        private static Action<BrowserProcessHandler> _delayedInitialization;

        public static void Initialize(CefSettings settings = null, KeyValuePair<string, string>[] flags = null, CustomScheme[] customSchemes = null)
        {
            _delayedInitialization = (browserProcessHandler) => InternalInitialize(settings, flags, customSchemes, browserProcessHandler);
        }

        private static void InternalInitialize(CefSettings settings = null, KeyValuePair<string, string>[] flags = null, CustomScheme[] customSchemes = null, BrowserProcessHandler browserProcessHandler = null)
        {
            CefRuntime.Load();

            if (settings == null)
            {
                settings = new CefSettings();
            }

            settings.UncaughtExceptionStackSize = 100; // for uncaught exception event work properly

            var paths = GetSubProcessPaths();
            var path = paths.FirstOrDefault(r => File.Exists(r));
            if(path == null)
                throw new FileNotFoundException($"Unable to find SubProcess. Probed locations: {string.Join(Environment.NewLine, paths)}");

            settings.BrowserSubprocessPath = path;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    settings.MultiThreadedMessageLoop = true;
                    break;

                case CefRuntimePlatform.MacOS:
                    var resourcesPath = Path.Combine(path, "Resources");
                    if (!Directory.Exists(resourcesPath))
                    {
                        throw new FileNotFoundException($"Unable to find Resources folder");
                    }

                    settings.NoSandbox = true;
                    settings.MultiThreadedMessageLoop = false;
                    settings.ExternalMessagePump = true;
                    settings.MainBundlePath = path;
                    settings.FrameworkDirPath = path;
                    settings.ResourcesDirPath = resourcesPath;
                    break;
            }

            AppDomain.CurrentDomain.ProcessExit += delegate { CefRuntime.Shutdown(); };

            IsOSREnabled = settings.WindowlessRenderingEnabled;
            CefRuntime.Initialize(new CefMainArgs(new string[0]), settings, new BrowserCefApp(customSchemes, flags, browserProcessHandler), IntPtr.Zero);

            if (customSchemes != null)
            {
                foreach (var scheme in customSchemes)
                {
                    CefRuntime.RegisterSchemeHandlerFactory(scheme.SchemeName, scheme.DomainName, scheme.SchemeHandlerFactory);
                }
            }
        }

        private static IEnumerable<string> GetSubProcessPaths()
        {
            var baseDirectory = AppContext.BaseDirectory;
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);

            // The executing DLL might not be in the current domain directory (plugins scenario)
            baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);
        }

        internal static void Load(BrowserProcessHandler browserProcessHandler = null)
        {
            if (_delayedInitialization != null)
            {
                _delayedInitialization.Invoke(browserProcessHandler);
                _delayedInitialization = null;
            }
            else
            {
                InternalInitialize(browserProcessHandler: browserProcessHandler);
            }
        }

        public static bool IsLoaded => CefRuntime.IsInitialized;

        internal static bool IsOSREnabled { get; private set; }

        private static string BrowserProcessFileName {
            get {
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

        private static string GetBaseDirectory()
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return baseDirectory;
        }
    }
}
