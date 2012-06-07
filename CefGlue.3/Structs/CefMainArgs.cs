namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    public sealed unsafe class CefMainArgs
    {
        private readonly Module _module;
        private readonly string[] _args;

        public CefMainArgs(string[] args)
        {
            _module = Assembly.GetEntryAssembly().GetModules()[0];
            _args = args;
        }

        internal cef_main_args_t* ToNative()
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    return (cef_main_args_t*)ToNativeWindows();

                case CefRuntimePlatform.Linux:
                case CefRuntimePlatform.MacOSX:
                    throw new NotImplementedException();
                    return (cef_main_args_t*)ToNativePosix();

                default:
                    throw ExceptionBuilder.UnsupportedPlatform();
            }
        }

        private cef_main_args_t_windows* ToNativeWindows()
        {
            var ptr = cef_main_args_t_windows.Alloc();
            ptr->instance = Marshal.GetHINSTANCE(_module);
            return ptr;
        }

        private cef_main_args_t_posix* ToNativePosix()
        {
            var ptr = cef_main_args_t_posix.Alloc();
            // TODO:
            return ptr;
        }

        internal static void Free(cef_main_args_t* ptr)
        {
            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    cef_main_args_t_windows.Free((cef_main_args_t_windows*)ptr);
                    return;

                case CefRuntimePlatform.Linux:
                case CefRuntimePlatform.MacOSX:
                    cef_main_args_t_posix.Free((cef_main_args_t_posix*)ptr);
                    return;

                default:
                    throw ExceptionBuilder.UnsupportedPlatform();
            }
        }
    }
}
