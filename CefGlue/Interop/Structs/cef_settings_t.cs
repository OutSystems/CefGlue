//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_settings_t.
//
// Note: cef_settings_t structure in CEF has 2 layouts (choosed in compile time),
//       so in C# we make different structures and will choose layouts in runtime.
//    Windows platform: cef_settings_t_windows.
//    Non-windows platforms: cef_settings_t_posix.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_settings_t
    {
        public UIntPtr size;
        public bool_t single_process;
        public cef_string_t browser_subprocess_path;
        public bool_t multi_threaded_message_loop;
        public bool_t command_line_args_disabled;
        public cef_string_t cache_path;
        public bool_t persist_session_cookies;
        public cef_string_t user_agent;
        public cef_string_t product_version;
        public cef_string_t locale;
        public cef_string_t log_file;
        public CefLogSeverity log_severity;
        public bool_t release_dcheck_enabled;
        public cef_string_t javascript_flags;
        public cef_string_t resources_dir_path;
        public cef_string_t locales_dir_path;
        public bool_t pack_loading_disabled;
        public int remote_debugging_port;
        public int uncaught_exception_stack_size;
        public int context_safety_implementation;
        public bool_t ignore_certificate_errors;

        #region Alloc & Free
        private static int _sizeof;

        static cef_settings_t()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_settings_t));
        }

        public static cef_settings_t* Alloc()
        {
            var ptr = (cef_settings_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_settings_t();
            ptr->size = (UIntPtr)_sizeof;
            return ptr;
        }

        public static void Free(cef_settings_t* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        #endregion
    }
}
