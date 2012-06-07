namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    public static unsafe class CefRuntime
    {
        private static readonly CefRuntimePlatform _platform;

        private static bool _loaded;
        private static bool _initialized;

        static CefRuntime()
        {
            _platform = DetectPlatform();
        }

        #region Platform Detection
        private static CefRuntimePlatform DetectPlatform()
        {
            var platformId = Environment.OSVersion.Platform;

            if (platformId == PlatformID.MacOSX)
                return CefRuntimePlatform.MacOSX;

            int p = (int)platformId;
            if ((p == 4) || (p == 128))
                return IsRunningOnMac() ? CefRuntimePlatform.MacOSX : CefRuntimePlatform.Linux;

            return CefRuntimePlatform.Windows;
        }

        //From Managed.Windows.Forms/XplatUI
        private static bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch { }
            finally
            {
                if (buf != IntPtr.Zero)
                    Marshal.FreeHGlobal(buf);
            }

            return false;
        }

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);

        public static CefRuntimePlatform Platform
        {
            get { return _platform; }
        }
        #endregion

        /// <summary>
        /// Loads CEF runtime.
        /// </summary>
        /// <exception cref="DllNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Load()
        {
            if (_loaded) return;

            var rev = BuildRevision;
            if (rev != libcef.CEF_REVISION) throw ExceptionBuilder.VersionMismatch(rev, libcef.CEF_REVISION);

            _loaded = true;
        }

        #region cef_app

        /// <summary>
        /// This function should be called from the application entry point function to
        /// execute a secondary process. It can be used to run secondary processes from
        /// the browser client executable (default behavior) or from a separate
        /// executable specified by the CefSettings.browser_subprocess_path value. If
        /// called for the browser process (identified by no "type" command-line value)
        /// it will return immediately with a value of -1. If called for a recognized
        /// secondary process it will block until the process should exit and then return
        /// the process exit code. The |application| parameter may be empty.
        /// </summary>
        public static int ExecuteProcess(CefMainArgs args, CefApp application)
        {
            LoadIfNeed();

            var n_args = args.ToNative();
            var n_app = application != null ? application.ToNative() : null;

            try
            {
                return libcef.execute_process(n_args, n_app);
            }
            finally
            {
                CefMainArgs.Free(n_args);
            }
        }

        /// <summary>
        /// This function should be called on the main application thread to initialize
        /// the CEF browser process. The |application| parameter may be empty. A return
        /// value of true indicates that it succeeded and false indicates that it failed.
        /// </summary>
        public static void Initialize(CefMainArgs args, CefSettings settings, CefApp application)
        {
            LoadIfNeed();

            if (args == null) throw new ArgumentNullException("args");
            if (settings == null) throw new ArgumentNullException("settings");

            if (_initialized) throw ExceptionBuilder.CefRuntimeAlreadyInitialized();

            var n_main_args = args.ToNative();
            var n_settings = settings.ToNative();
            var n_app = application != null ? application.ToNative() : null;

            try
            {
                if (libcef.initialize(n_main_args, n_settings, n_app) != 0)
                {
                    _initialized = true;
                }
                else
                {
                    throw ExceptionBuilder.CefRuntimeFailedToInitialize();
                }
            }
            finally
            {
                CefMainArgs.Free(n_main_args);
                CefSettings.Free(n_settings);
            }
        }

        /// <summary>
        /// This function should be called on the main application thread to shut down
        /// the CEF browser process before the application exits.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            libcef.shutdown();
        }

        /// <summary>
        /// Perform a single iteration of CEF message loop processing. This function is
        /// used to integrate the CEF message loop into an existing application message
        /// loop. Care must be taken to balance performance against excessive CPU usage.
        /// This function should only be called on the main application thread and only
        /// if CefInitialize() is called with a CefSettings.multi_threaded_message_loop
        /// value of false. This function will not block.
        /// </summary>
        public static void DoMessageLoopWork()
        {
            libcef.do_message_loop_work();
        }

        /// <summary>
        /// Run the CEF message loop. Use this function instead of an application-
        /// provided message loop to get the best balance between performance and CPU
        /// usage. This function should only be called on the main application thread and
        /// only if CefInitialize() is called with a
        /// CefSettings.multi_threaded_message_loop value of false. This function will
        /// block until a quit message is received by the system.
        /// </summary>
        public static void RunMessageLoop()
        {
            libcef.run_message_loop();
        }

        /// <summary>
        /// Quit the CEF message loop that was started by calling CefRunMessageLoop().
        /// This function should only be called on the main application thread and only
        /// if CefRunMessageLoop() was used.
        /// </summary>
        public static void QuitMessageLoop()
        {
            libcef.quit_message_loop();
        }

        #endregion

        #region cef_task

        /// <summary>
        /// CEF maintains multiple internal threads that are used for handling different
        /// types of tasks in different processes. See the cef_thread_id_t definitions in
        /// cef_types.h for more information. This function will return true if called on
        /// the specified thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool CurrentlyOn(CefThreadId threadId)
        {
            return libcef.currently_on(threadId) != 0;
        }

        /// <summary>
        /// Post a task for execution on the specified thread. This function may be
        /// called on any thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool PostTask(CefThreadId threadId, CefTask task)
        {
            if (task == null) throw new ArgumentNullException("task");

            return libcef.post_task(threadId, task.ToNative()) != 0;
        }

        /// <summary>
        /// Post a task for delayed execution on the specified thread. This function may
        /// be called on any thread. It is an error to request a thread from the wrong
        /// process.
        /// </summary>
        public static bool PostTask(CefThreadId threadId, CefTask task, long delay)
        {
            if (task == null) throw new ArgumentNullException("task");

            return libcef.post_delayed_task(threadId, task.ToNative(), delay) != 0;
        }
        #endregion

        #region cef_version

        internal static int BuildRevision
        {
            get { return libcef.build_revision(); }
        }

        #endregion

        #region cef_origin_whitelist
        // TODO: CefRuntime.AddCrossOriginWhitelistEntry
        // TODO: CefRuntime.RemoveCrossOriginWhitelistEntry
        // TODO: CefRuntime.ClearCrossOriginWhitelist
        /*
        // CefAddCrossOriginWhitelistEntry
        [DllImport(libcef.DllName, EntryPoint = "cef_add_cross_origin_whitelist_entry", CallingConvention = libcef.CEF_CALL)]
        public static extern int add_cross_origin_whitelist_entry(cef_string_t* source_origin, cef_string_t* target_protocol, cef_string_t* target_domain, int allow_target_subdomains);

        // CefRemoveCrossOriginWhitelistEntry
        [DllImport(libcef.DllName, EntryPoint = "cef_remove_cross_origin_whitelist_entry", CallingConvention = libcef.CEF_CALL)]
        public static extern int remove_cross_origin_whitelist_entry(cef_string_t* source_origin, cef_string_t* target_protocol, cef_string_t* target_domain, int allow_target_subdomains);

        // CefClearCrossOriginWhitelist
        [DllImport(libcef.DllName, EntryPoint = "cef_clear_cross_origin_whitelist", CallingConvention = libcef.CEF_CALL)]
        public static extern int clear_cross_origin_whitelist();
        */

        #endregion

        #region cef_scheme
        // TODO: CefRuntime.RegisterSchemeHandlerFactory
        // TODO: CefRuntime.ClearSchemeHandlerFactories
        /*
        // CefRegisterSchemeHandlerFactory
        [DllImport(libcef.DllName, EntryPoint = "cef_register_scheme_handler_factory", CallingConvention = libcef.CEF_CALL)]
        public static extern int register_scheme_handler_factory(cef_string_t* scheme_name, cef_string_t* domain_name, cef_scheme_handler_factory_t* factory);

        // CefClearSchemeHandlerFactories
        [DllImport(libcef.DllName, EntryPoint = "cef_clear_scheme_handler_factories", CallingConvention = libcef.CEF_CALL)]
        public static extern int clear_scheme_handler_factories();
        */

        #endregion

        #region cef_url
        // TODO: CefRuntime.ParseUrl
        // TODO: CefRuntime.CreateUrl
        /*
        // CefParseURL
        [DllImport(libcef.DllName, EntryPoint = "cef_parse_url", CallingConvention = libcef.CEF_CALL)]
        public static extern int parse_url(cef_string_t* url, cef_urlparts_t* parts);

        // CefCreateURL
        [DllImport(libcef.DllName, EntryPoint = "cef_create_url", CallingConvention = libcef.CEF_CALL)]
        public static extern int create_url(cef_urlparts_t* parts, cef_string_t* url);
        */

        #endregion

        #region cef_v8
        // TODO: CefRuntime.RegisterExtension
        /*
        // CefRegisterExtension
        [DllImport(libcef.DllName, EntryPoint = "cef_register_extension", CallingConvention = libcef.CEF_CALL)]
        public static extern int register_extension(cef_string_t* extension_name, cef_string_t* javascript_code, cef_v8handler_t* handler);
        */
        #endregion

        #region cef_web_plugin
        // TODO: CefRuntime.VisitWebPluginInfo
        /*
        // CefVisitWebPluginInfo
        [DllImport(libcef.DllName, EntryPoint = "cef_visit_web_plugin_info", CallingConvention = libcef.CEF_CALL)]
        public static extern void visit_web_plugin_info(cef_web_plugin_info_visitor_t* visitor);
        */
        #endregion

        private static void LoadIfNeed()
        {
            if (!_loaded) Load();
        }
    }
}
