﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared;
using System.Runtime.InteropServices;

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

            var basePath = AppContext.BaseDirectory;
            var probingPaths = GetSubProcessPaths(basePath);
            var subProcessPath = probingPaths.FirstOrDefault(p => File.Exists(p));
            if (subProcessPath == null)
                throw new FileNotFoundException($"Unable to find SubProcess. Probed locations: {string.Join(Environment.NewLine, probingPaths)}");

            settings.BrowserSubprocessPath = subProcessPath;

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    settings.MultiThreadedMessageLoop = true;
                    break;

                case CefRuntimePlatform.MacOS:
                    var resourcesPath = Path.Combine(basePath, "Resources");
                    if (!Directory.Exists(resourcesPath))
                    {
                        throw new FileNotFoundException($"Unable to find Resources folder");
                    }

                    settings.NoSandbox = true;
                    settings.MultiThreadedMessageLoop = false;
                    settings.ExternalMessagePump = true;
                    settings.MainBundlePath = basePath;
                    settings.FrameworkDirPath = basePath;
                    settings.ResourcesDirPath = resourcesPath;
                    break;
                
                case CefRuntimePlatform.Linux:
                    settings.NoSandbox = true;
                    settings.MultiThreadedMessageLoop = true;
                    break;
            }

            AppDomain.CurrentDomain.ProcessExit += delegate { CefRuntime.Shutdown(); };

            IsOSREnabled = settings.WindowlessRenderingEnabled;

            // On Linux, with osr disable, the filename in CefMainArgs will be used as accessible name.
            // If the name is empty, chromium will crash at ui::AXNodeData:SetNamechecked.
            var exeFileName = Process.GetCurrentProcess().MainModule.FileName;
            if (string.IsNullOrEmpty(exeFileName))
            {
                exeFileName = "CefGlue";
            }
            
            // Fix crash with youtube https://github.com/chromiumembedded/cef/issues/3643
            {
#if DEBUG
                if (CefRuntime.ChromeVersion.Split(".").First() != "120")
                {
                    throw new Exception("Remove this fix block after CEF upgrade");
                }
#endif
                flags = (flags ?? []).Append(KeyValuePair.Create("disable-features", "FirstPartySets")).ToArray();
            }
            
            InitializeCefRuntime(settings, flags, customSchemes, browserProcessHandler, exeFileName);

            if (customSchemes != null)
            {
                foreach (var scheme in customSchemes)
                {
                    CefRuntime.RegisterSchemeHandlerFactory(scheme.SchemeName, scheme.DomainName, scheme.SchemeHandlerFactory);
                }
            }
        }

                #region FIX_MACOS_PROCESS_HANG

        // P/Invoke - on macOS libc resolves to libSystem.B.dylib; on Linux to libc.so.6
        [DllImport("libc", SetLastError = true)]
        private static extern int sigaction(int sig, IntPtr act, IntPtr oact);

        private static void InitializeCefRuntime(CefSettings settings, KeyValuePair<string, string>[] flags, CustomScheme[] customSchemes,
            BrowserProcessHandler browserProcessHandler, string exeFileName)
        {
            // Only run the sigaction workaround on Unix-like OSes
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Non-Unix: just initialize CEF normally
                CefRuntime.Initialize(new CefMainArgs(new[] { exeFileName }), settings,
                    new BrowserCefApp(customSchemes, flags, browserProcessHandler), IntPtr.Zero);
                return;
            }

            // Signal list (named constants would be better; using common ones with fallback)
            var signals = GetUnixSignalNumbers();

            // Save previous signal handlers
            var allocs = new List<(int Signal, IntPtr Buffer)>();
            try
            {
                foreach (var signal in signals)
                {
                    IntPtr buffer = Marshal.AllocHGlobal(512); // safe over-allocation for struct sigaction
                                                               // (optional) zero memory so we start clean
                    Span<byte> zeros = stackalloc byte[64]; // small stackzero; we'll use Marshal.Copy below to zero more if desired
                                                            // Zero whole buffer:
                    unsafe
                    {
                        byte* p = (byte*)buffer.ToPointer();
                        for (int i = 0; i < 512; i++) p[i] = 0;
                    }

                    allocs.Add((signal, buffer));

                    var rv = sigaction(signal, IntPtr.Zero, buffer);
                    if (rv != 0)
                    {
                        // Log, but continue trying to save other handlers; you may want to throw here
                        var err = Marshal.GetLastWin32Error();
                        Debug.WriteLine($"[StudioCefRuntimeLoader] sigaction(GET) failed for signal {signal}. rv={rv}, errno={err}");
                    }
                }

                // Initialize CEF while handlers are saved
                CefRuntime.Initialize(new CefMainArgs(new[] { exeFileName }), settings,
                    new BrowserCefApp(customSchemes, flags, browserProcessHandler), IntPtr.Zero);
            }
            finally
            {
                // Restore previous handlers and free buffers (always try to restore even if Initialize threw)
                foreach (var entry in allocs)
                {
                    try
                    {
                        var rv = sigaction(entry.Signal, entry.Buffer, IntPtr.Zero);
                        if (rv != 0)
                        {
                            var err = Marshal.GetLastWin32Error();
                            Debug.WriteLine($"[StudioCefRuntimeLoader] sigaction(RESTORE) failed for signal {entry.Signal}. rv={rv}, errno={err}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[StudioCefRuntimeLoader] Exception restoring signal {entry.Signal}: {ex}");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(entry.Buffer);
                    }
                }
            }
        }

        // Helper to provide signal numbers in a cross-platform way
        private static int[] GetUnixSignalNumbers()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // macOS signal numbers (common mapping — verify on your target macOS version)
                const int SIGHUP = 1;
                const int SIGINT = 2;
                const int SIGQUIT = 3;
                const int SIGILL = 4;
                const int SIGTRAP = 5;
                const int SIGABRT = 6;
                const int SIGBUS = 10;   // note: SIGBUS is 10 on macOS
                const int SIGFPE = 8;
                const int SIGSEGV = 11;
                const int SIGPIPE = 13;
                const int SIGALRM = 14;
                const int SIGTERM = 15;
                const int SIGCHLD = 20;  // SIGCHLD often 20 on macOS
                return new[] { SIGHUP, SIGINT, SIGQUIT, SIGILL, SIGTRAP, SIGABRT, SIGBUS, SIGFPE, SIGSEGV, SIGPIPE, SIGALRM, SIGTERM, SIGCHLD };
            }
            else
            {
                // Linux default numbers (glibc/x86_64)
                const int SIGHUP = 1;
                const int SIGINT = 2;
                const int SIGQUIT = 3;
                const int SIGILL = 4;
                const int SIGTRAP = 5;
                const int SIGABRT = 6;
                const int SIGBUS = 7;
                const int SIGFPE = 8;
                const int SIGSEGV = 11;
                const int SIGPIPE = 13;
                const int SIGALRM = 14;
                const int SIGTERM = 15;
                const int SIGCHLD = 17;
                return new[] { SIGHUP, SIGINT, SIGQUIT, SIGILL, SIGTRAP, SIGABRT, SIGBUS, SIGFPE, SIGSEGV, SIGPIPE, SIGALRM, SIGTERM, SIGCHLD };
            }
        }

        #endregion
        
        private static IEnumerable<string> GetSubProcessPaths(string baseDirectory)
        {
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);

            // The executing DLL might not be in the current domain directory (plugins scenario)
            baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            yield return Path.Combine(baseDirectory, DefaultBrowserProcessDirectory, BrowserProcessFileName);
            yield return Path.Combine(baseDirectory, BrowserProcessFileName);
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
