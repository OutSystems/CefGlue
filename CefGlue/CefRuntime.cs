namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
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
        /// <exception cref="CefVersionMismatchException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Load()
        {
            if (_loaded) return;

            CheckVersion();

            _loaded = true;
        }

        #region cef_version

        private static void CheckVersion()
        {
            try
            {
                CheckVersionByApiHash();
            }
            catch (NotSupportedException) // TODO: once load options will be implemented, we can control how perform version
            {
                CheckVersionByBuildRevision();
            }
        }

        private static void CheckVersionByApiHash()
        {
            // get CEF_API_HASH_PLATFORM
            string actual;
            try
            {
                var n_actual = libcef.api_hash(0);
                actual = n_actual != null ? new string(n_actual) : null;
            }
            catch (EntryPointNotFoundException ex)
            {
                throw new NotSupportedException("cef_api_hash call is not supported.", ex);
            }
            if (string.IsNullOrEmpty(actual)) throw new NotSupportedException();

            string expected;
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows: expected = libcef.CEF_API_HASH_PLATFORM_WIN; break;
                case CefRuntimePlatform.MacOSX: expected = libcef.CEF_API_HASH_PLATFORM_MACOSX; break;
                case CefRuntimePlatform.Linux: expected = libcef.CEF_API_HASH_PLATFORM_LINUX; break;
                default: throw new PlatformNotSupportedException();
            }

            if (string.Compare(actual, expected, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw ExceptionBuilder.RuntimeVersionApiHashMismatch(actual, expected);
            }
        }

        private static void CheckVersionByBuildRevision()
        {
            var revision = libcef.build_revision();
            if (revision != libcef.CEF_REVISION)
            {
                throw ExceptionBuilder.RuntimeVersionBuildRevisionMismatch(revision, libcef.CEF_REVISION);
            }
        }

        #endregion

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

        /// <summary>
        /// Set to true before calling Windows APIs like TrackPopupMenu that enter a
        /// modal message loop. Set to false after exiting the modal message loop.
        /// </summary>
        public static void SetOSModalLoop(bool osModalLoop)
        {
            libcef.set_osmodal_loop(osModalLoop ? 1 : 0);
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

        #region cef_geolocation

        /// <summary>
        /// Request a one-time geolocation update. This function bypasses any user
        /// permission checks so should only be used by code that is allowed to access
        /// location information.
        /// </summary>
        public static bool GetGeolocation(CefGetGeolocationCallback callback)
        {
            if (callback == null) throw new ArgumentNullException("callback");

            return libcef.get_geolocation(callback.ToNative()) != 0;
        }

        #endregion

        #region cef_origin_whitelist

        /// <summary>
        /// Add an entry to the cross-origin access whitelist.
        ///
        /// The same-origin policy restricts how scripts hosted from different origins
        /// (scheme + domain + port) can communicate. By default, scripts can only access
        /// resources with the same origin. Scripts hosted on the HTTP and HTTPS schemes
        /// (but no other schemes) can use the "Access-Control-Allow-Origin" header to
        /// allow cross-origin requests. For example, https://source.example.com can make
        /// XMLHttpRequest requests on http://target.example.com if the
        /// http://target.example.com request returns an "Access-Control-Allow-Origin:
        /// https://source.example.com" response header.
        ///
        /// Scripts in separate frames or iframes and hosted from the same protocol and
        /// domain suffix can execute cross-origin JavaScript if both pages set the
        /// document.domain value to the same domain suffix. For example,
        /// scheme://foo.example.com and scheme://bar.example.com can communicate using
        /// JavaScript if both domains set document.domain="example.com".
        ///
        /// This method is used to allow access to origins that would otherwise violate
        /// the same-origin policy. Scripts hosted underneath the fully qualified
        /// |source_origin| URL (like http://www.example.com) will be allowed access to
        /// all resources hosted on the specified |target_protocol| and |target_domain|.
        /// If |target_domain| is non-empty and |allow_target_subdomains| if false only
        /// exact domain matches will be allowed. If |target_domain| contains a top-
        /// level domain component (like "example.com") and |allow_target_subdomains| is
        /// true sub-domain matches will be allowed. If |target_domain| is empty and
        /// |allow_target_subdomains| if true all domains and IP addresses will be
        /// allowed.
        ///
        /// This method cannot be used to bypass the restrictions on local or display
        /// isolated schemes. See the comments on CefRegisterCustomScheme for more
        /// information.
        ///
        /// This function may be called on any thread. Returns false if |source_origin|
        /// is invalid or the whitelist cannot be accessed.
        /// </summary>
        public static bool AddCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
        {
            if (string.IsNullOrEmpty("sourceOrigin")) throw new ArgumentNullException("sourceOrigin");
            if (string.IsNullOrEmpty("targetProtocol")) throw new ArgumentNullException("targetProtocol");

            fixed (char* sourceOrigin_ptr = sourceOrigin)
            fixed (char* targetProtocol_ptr = targetProtocol)
            fixed (char* targetDomain_ptr = targetDomain)
            {
                var n_sourceOrigin = new cef_string_t(sourceOrigin_ptr, sourceOrigin.Length);
                var n_targetProtocol = new cef_string_t(targetProtocol_ptr, targetProtocol.Length);
                var n_targetDomain = new cef_string_t(targetDomain_ptr, targetDomain != null ? targetDomain.Length : 0);

                return libcef.add_cross_origin_whitelist_entry(
                    &n_sourceOrigin,
                    &n_targetProtocol,
                    &n_targetDomain,
                    allowTargetSubdomains ? 1 : 0
                    ) != 0;
            }
        }

        /// <summary>
        /// Remove an entry from the cross-origin access whitelist. Returns false if
        /// |source_origin| is invalid or the whitelist cannot be accessed.
        /// </summary>
        public static bool RemoveCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
        {
            if (string.IsNullOrEmpty("sourceOrigin")) throw new ArgumentNullException("sourceOrigin");
            if (string.IsNullOrEmpty("targetProtocol")) throw new ArgumentNullException("targetProtocol");

            fixed (char* sourceOrigin_ptr = sourceOrigin)
            fixed (char* targetProtocol_ptr = targetProtocol)
            fixed (char* targetDomain_ptr = targetDomain)
            {
                var n_sourceOrigin = new cef_string_t(sourceOrigin_ptr, sourceOrigin.Length);
                var n_targetProtocol = new cef_string_t(targetProtocol_ptr, targetProtocol.Length);
                var n_targetDomain = new cef_string_t(targetDomain_ptr, targetDomain != null ? targetDomain.Length : 0);

                return libcef.remove_cross_origin_whitelist_entry(
                    &n_sourceOrigin,
                    &n_targetProtocol,
                    &n_targetDomain,
                    allowTargetSubdomains ? 1 : 0
                    ) != 0;
            }
        }

        /// <summary>
        /// Remove all entries from the cross-origin access whitelist. Returns false if
        /// the whitelist cannot be accessed.
        /// </summary>
        public static bool ClearCrossOriginWhitelist()
        {
            return libcef.clear_cross_origin_whitelist() != 0;
        }

        #endregion

        #region cef_scheme

        /// <summary>
        /// Register a scheme handler factory for the specified |scheme_name| and
        /// optional |domain_name|. An empty |domain_name| value for a standard scheme
        /// will cause the factory to match all domain names. The |domain_name| value
        /// will be ignored for non-standard schemes. If |scheme_name| is a built-in
        /// scheme and no handler is returned by |factory| then the built-in scheme
        /// handler factory will be called. If |scheme_name| is a custom scheme then
        /// also implement the CefApp::OnRegisterCustomSchemes() method in all processes.
        /// This function may be called multiple times to change or remove the factory
        /// that matches the specified |scheme_name| and optional |domain_name|.
        /// Returns false if an error occurs. This function may be called on any thread
        /// in the browser process.
        /// </summary>
        public static bool RegisterSchemeHandlerFactory(string schemeName, string domainName, CefSchemeHandlerFactory factory)
        {
            if (string.IsNullOrEmpty(schemeName)) throw new ArgumentNullException("schemeName");
            if (factory == null) throw new ArgumentNullException("factory");

            fixed (char* schemeName_str = schemeName)
            fixed (char* domainName_str = domainName)
            {
                var n_schemeName = new cef_string_t(schemeName_str, schemeName.Length);
                var n_domainName = new cef_string_t(domainName_str, domainName != null ? domainName.Length : 0);

                return libcef.register_scheme_handler_factory(&n_schemeName, &n_domainName, factory.ToNative()) != 0;
            }
        }

        /// <summary>
        /// Clear all registered scheme handler factories. Returns false on error. This
        /// function may be called on any thread in the browser process.
        /// </summary>
        public static bool ClearSchemeHandlerFactories()
        {
            return libcef.clear_scheme_handler_factories() != 0;
        }

        #endregion

        #region cef_trace

        // TODO: CefBeginTracing
        //[DllImport(libcef.DllName, EntryPoint = "cef_begin_tracing", CallingConvention = libcef.CEF_CALL)]
        //public static extern int begin_tracing(cef_trace_client_t* client, cef_string_t* categories);

        // TODO: CefGetTraceBufferPercentFullAsync
        //[DllImport(libcef.DllName, EntryPoint = "cef_get_trace_buffer_percent_full_async", CallingConvention = libcef.CEF_CALL)]
        //public static extern int get_trace_buffer_percent_full_async();

        // TODO: CefEndTracingAsync
        //[DllImport(libcef.DllName, EntryPoint = "cef_end_tracing_async", CallingConvention = libcef.CEF_CALL)]
        //public static extern int end_tracing_async();

        // TODO: functions from cef_trace_event.h (not generated automatically)

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

        /// <summary>
        /// Register a new V8 extension with the specified JavaScript extension code and
        /// handler. Functions implemented by the handler are prototyped using the
        /// keyword 'native'. The calling of a native function is restricted to the scope
        /// in which the prototype of the native function is defined. This function may
        /// only be called on the render process main thread.
        ///
        /// Example JavaScript extension code:
        /// <code>
        ///   // create the 'example' global object if it doesn't already exist.
        ///   if (!example)
        ///     example = {};
        ///   // create the 'example.test' global object if it doesn't already exist.
        ///   if (!example.test)
        ///     example.test = {};
        ///   (function() {
        ///     // Define the function 'example.test.myfunction'.
        ///     example.test.myfunction = function() {
        ///       // Call CefV8Handler::Execute() with the function name 'MyFunction'
        ///       // and no arguments.
        ///       native function MyFunction();
        ///       return MyFunction();
        ///     };
        ///     // Define the getter function for parameter 'example.test.myparam'.
        ///     example.test.__defineGetter__('myparam', function() {
        ///       // Call CefV8Handler::Execute() with the function name 'GetMyParam'
        ///       // and no arguments.
        ///       native function GetMyParam();
        ///       return GetMyParam();
        ///     });
        ///     // Define the setter function for parameter 'example.test.myparam'.
        ///     example.test.__defineSetter__('myparam', function(b) {
        ///       // Call CefV8Handler::Execute() with the function name 'SetMyParam'
        ///       // and a single argument.
        ///       native function SetMyParam();
        ///       if(b) SetMyParam(b);
        ///     });
        ///
        ///     // Extension definitions can also contain normal JavaScript variables
        ///     // and functions.
        ///     var myint = 0;
        ///     example.test.increment = function() {
        ///       myint += 1;
        ///       return myint;
        ///     };
        ///   })();
        /// </code>
        /// Example usage in the page:
        /// <code>
        ///   // Call the function.
        ///   example.test.myfunction();
        ///   // Set the parameter.
        ///   example.test.myparam = value;
        ///   // Get the parameter.
        ///   value = example.test.myparam;
        ///   // Call another function.
        ///   example.test.increment();
        /// </code>
        /// </summary>
        public static bool RegisterExtension(string extensionName, string javascriptCode, CefV8Handler handler)
        {
            if (string.IsNullOrEmpty(extensionName)) throw new ArgumentNullException("extensionName");
            if (string.IsNullOrEmpty(javascriptCode)) throw new ArgumentNullException("javascriptCode");

            fixed (char* extensionName_str = extensionName)
            fixed (char* javascriptCode_str = javascriptCode)
            {
                var n_extensionName = new cef_string_t(extensionName_str, extensionName.Length);
                var n_javascriptCode = new cef_string_t(javascriptCode_str, javascriptCode.Length);

                return libcef.register_extension(&n_extensionName, &n_javascriptCode, handler != null ? handler.ToNative() : null) != 0;
            }
        }

        #endregion

        #region cef_web_plugin

        // TODO: move web plugins methods to CefRuntime.WebPlugin.Xxx

        /// <summary>
        /// Visit web plugin information. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void VisitWebPluginInfo(CefWebPluginInfoVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            libcef.visit_web_plugin_info(visitor.ToNative());
        }

        /// <summary>
        /// Cause the plugin list to refresh the next time it is accessed regardless
        /// of whether it has already been loaded. Can be called on any thread in the
        /// browser process.
        /// </summary>
        public static void RefreshWebPlugins()
        {
            libcef.refresh_web_plugins();
        }

        /// <summary>
        /// Add a plugin path (directory + file). This change may not take affect until
        /// after CefRefreshWebPlugins() is called. Can be called on any thread in the
        /// browser process.
        /// </summary>
        public static void AddWebPluginPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.add_web_plugin_path(&n_path);
            }
        }

        /// <summary>
        /// Add a plugin directory. This change may not take affect until after
        /// CefRefreshWebPlugins() is called. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void AddWebPluginDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException("path");

            fixed (char* directory_str = directory)
            {
                var n_directory = new cef_string_t(directory_str, directory.Length);
                libcef.add_web_plugin_directory(&n_directory);
            }
        }

        /// <summary>
        /// Remove a plugin path (directory + file). This change may not take affect
        /// until after CefRefreshWebPlugins() is called. Can be called on any thread in
        /// the browser process.
        /// </summary>
        public static void RemoveWebPluginPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.remove_web_plugin_path(&n_path);
            }
        }

        /// <summary>
        /// Unregister an internal plugin. This may be undone the next time
        /// CefRefreshWebPlugins() is called. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void UnregisterInternalWebPlugin(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.unregister_internal_web_plugin(&n_path);
            }
        }

        /// <summary>
        /// Force a plugin to shutdown. Can be called on any thread in the browser
        /// process but will be executed on the IO thread.
        /// </summary>
        public static void ForceWebPluginShutdown(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.force_web_plugin_shutdown(&n_path);
            }
        }

        /// <summary>
        /// Register a plugin crash. Can be called on any thread in the browser process
        /// but will be executed on the IO thread.
        /// </summary>
        public static void RegisterWebPluginCrash(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.register_web_plugin_crash(&n_path);
            }
        }

        /// <summary>
        /// Query if a plugin is unstable. Can be called on any thread in the browser
        /// process.
        /// </summary>
        public static void IsWebPluginUnstable(string path, CefWebPluginUnstableCallback callback)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (callback == null) throw new ArgumentNullException("callback");

            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path.Length);
                libcef.is_web_plugin_unstable(&n_path, callback.ToNative());
            }
        }

        #endregion

        #region cef_path_util

        /// <summary>
        /// Retrieve the path associated with the specified |key|. Returns true on
        /// success. Can be called on any thread in the browser process.
        /// </summary>
        public static string GetPath(CefPathKey pathKey)
        {
            var n_value = new cef_string_t();
            var success = libcef.get_path(pathKey, &n_value) != 0;
            var value = cef_string_t.ToString(&n_value);
            libcef.string_clear(&n_value);
            if (!success)
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "Failed to get path for key {0}.", pathKey)
                    );
            }
            return value;
        }

        #endregion

        #region cef_process_util

        /// <summary>
        /// Launches the process specified via |command_line|. Returns true upon
        /// success. Must be called on the browser process TID_PROCESS_LAUNCHER thread.
        ///
        /// Unix-specific notes:
        /// - All file descriptors open in the parent process will be closed in the
        ///   child process except for stdin, stdout, and stderr.
        /// - If the first argument on the command line does not contain a slash,
        ///   PATH will be searched. (See man execvp.)
        /// </summary>
        public static bool LaunchProcess(CefCommandLine commandLine)
        {
            if (commandLine == null) throw new ArgumentNullException("commandLine");

            return libcef.launch_process(commandLine.ToNative()) != 0;
        }

        #endregion

        private static void LoadIfNeed()
        {
            if (!_loaded) Load();
        }
    }
}
