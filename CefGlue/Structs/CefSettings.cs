namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefSettings
    {
        /// <summary>
        /// Set to <c>true</c> to use a single process for the browser and renderer. This
        /// run mode is not officially supported by Chromium and is less stable than
        /// the multi-process default.
        /// </summary>
        public bool SingleProcess { get; set; }

        /// <summary>
        /// The path to a separate executable that will be launched for sub-processes.
        /// By default the browser process executable is used. See the comments on
        /// CefExecuteProcess() for details.
        /// </summary>
        public string BrowserSubprocessPath { get; set; }

        /// <summary>
        /// Set to <c>true</c> to have the browser process message loop run in a separate
        /// thread. If <c>false</c> than the CefDoMessageLoopWork() function must be
        /// called from your application message loop.
        /// </summary>
        public bool MultiThreadedMessageLoop { get; set; }

        /// <summary>
        /// Set to <c>true</c> to disable configuration of browser process features using
        /// standard CEF and Chromium command-line arguments. Configuration can still
        /// be specified using CEF data structures or via the
        /// CefApp::OnBeforeCommandLineProcessing() method.
        /// </summary>
        public bool CommandLineArgsDisabled { get; set; }

        /// <summary>
        /// The location where cache data will be stored on disk. If empty an in-memory
        /// cache will be used. HTML5 databases such as localStorage will only persist
        /// across sessions if a cache path is specified.
        /// </summary>
        public string CachePath { get; set; }

        /// <summary>
        /// Value that will be returned as the User-Agent HTTP header. If empty the
        /// default User-Agent string will be used.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Value that will be inserted as the product portion of the default
        /// User-Agent string. If empty the Chromium product version will be used. If
        /// |userAgent| is specified this value will be ignored.
        /// </summary>
        public string ProductVersion { get; set; }

        /// <summary>
        /// The locale string that will be passed to WebKit. If empty the default
        /// locale of "en-US" will be used. This value is ignored on Linux where locale
        /// is determined using environment variable parsing with the precedence order:
        /// LANGUAGE, LC_ALL, LC_MESSAGES and LANG.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The directory and file name to use for the debug log. If empty, the
        /// default name of "debug.log" will be used and the file will be written
        /// to the application directory.
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// The log severity. Only messages of this severity level or higher will be
        /// logged.
        /// </summary>
        public CefLogSeverity LogSeverity { get; set; }

        /// <summary>
        /// Custom flags that will be used when initializing the V8 JavaScript engine.
        /// The consequences of using custom flags may not be well tested.
        /// </summary>
        public string JavaScriptFlags { get; set; }

        /// <summary>
        /// Set to <c>true</c> to use the system proxy resolver on Windows when
        /// "Automatically detect settings" is checked. This setting is disabled
        /// by default for performance reasons.
        /// </summary>
        public bool AutoDetectProxySettingsEnabled { get; set; }

        /// <summary>
        /// The fully qualified path for the resources directory. If this value is
        /// empty the cef.pak and/or devtools_resources.pak files must be located in
        /// the module directory on Windows/Linux or the app bundle Resources directory
        /// on Mac OS X.
        /// </summary>
        public string ResourcesDirPath { get; set; }

        /// <summary>
        /// The fully qualified path for the locales directory. If this value is empty
        /// the locales directory must be located in the module directory. This value
        /// is ignored on Mac OS X where pack files are always loaded from the app
        /// bundle resource directory.
        /// </summary>
        public string LocalesDirPath { get; set; }

        /// <summary>
        /// Set to <c>true</c> to disable loading of pack files for resources and locales.
        /// A resource bundle handler must be provided for the browser and render
        /// processes via CefApp::GetResourceBundleHandler() if loading of pack files
        /// is disabled.
        /// </summary>
        public bool PackLoadingDisabled { get; set; }

        /// <summary>
        /// Set to a value between 1024 and 65535 to enable remote debugging on the
        /// specified port. For example, if 8080 is specified the remote debugging URL
        /// will be http://localhost:8080. CEF can be remotely debugged from any CEF or
        /// Chrome browser window.
        /// </summary>
        public int RemoteDebuggingPort { get; set; }

        /// <summary>
        /// The number of stack trace frames to capture for uncaught exceptions.
        /// Specify a positive value to enable the CefV8ContextHandler::
        /// OnUncaughtException() callback. Specify 0 (default value) and
        /// OnUncaughtException() will not be called.
        /// </summary>
        public int UncaughtExceptionStackSize { get; set; }

        /// <summary>
        /// By default CEF V8 references will be invalidated (the IsValid() method will
        /// return false) after the owning context has been released. This reduces the
        /// need for external record keeping and avoids crashes due to the use of V8
        /// references after the associated context has been released.
        ///
        /// CEF currently offers two context safety implementations with different
        /// performance characteristics. The default implementation (value of 0) uses a
        /// map of hash values and should provide better performance in situations with
        /// a small number contexts. The alternate implementation (value of 1) uses a
        /// hidden value attached to each context and should provide better performance
        /// in situations with a large number of contexts.
        ///
        /// If you need better performance in the creation of V8 references and you
        /// plan to manually track context lifespan you can disable context safety by
        /// specifying a value of -1.
        /// </summary>
        public CefContextSafetyImplementation ContextSafetyImplementation { get; set; }


        internal cef_settings_t* ToNative()
        {
            var ptr = cef_settings_t.Alloc();
            ptr->single_process = SingleProcess;
            cef_string_t.Copy(BrowserSubprocessPath, &ptr->browser_subprocess_path);
            ptr->multi_threaded_message_loop = MultiThreadedMessageLoop;
            ptr->command_line_args_disabled = CommandLineArgsDisabled;
            cef_string_t.Copy(CachePath, &ptr->cache_path);
            cef_string_t.Copy(UserAgent, &ptr->user_agent);
            cef_string_t.Copy(ProductVersion, &ptr->product_version);
            cef_string_t.Copy(Locale, &ptr->locale);
            cef_string_t.Copy(LogFile, &ptr->log_file);
            ptr->log_severity = LogSeverity;
            cef_string_t.Copy(JavaScriptFlags, &ptr->javascript_flags);
            ptr->auto_detect_proxy_settings_enabled = AutoDetectProxySettingsEnabled;
            cef_string_t.Copy(ResourcesDirPath, &ptr->resources_dir_path);
            cef_string_t.Copy(LocalesDirPath, &ptr->locales_dir_path);
            ptr->pack_loading_disabled = PackLoadingDisabled;
            ptr->remote_debugging_port = RemoteDebuggingPort;
            ptr->uncaught_exception_stack_size = UncaughtExceptionStackSize;
            ptr->context_safety_implementation = (int)ContextSafetyImplementation;
            return ptr;
        }

        private static void Clear(cef_settings_t* ptr)
        {
            libcef.string_clear(&ptr->browser_subprocess_path);
            libcef.string_clear(&ptr->cache_path);
            libcef.string_clear(&ptr->user_agent);
            libcef.string_clear(&ptr->product_version);
            libcef.string_clear(&ptr->locale);
            libcef.string_clear(&ptr->log_file);
            libcef.string_clear(&ptr->javascript_flags);
            libcef.string_clear(&ptr->resources_dir_path);
            libcef.string_clear(&ptr->locales_dir_path);
        }

        internal static void Free(cef_settings_t* ptr)
        {
            Clear((cef_settings_t*)ptr);
            cef_settings_t.Free((cef_settings_t*)ptr);
        }
    }
}
