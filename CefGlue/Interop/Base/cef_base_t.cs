namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_base_t
    {
        internal UIntPtr _size;
        internal IntPtr _add_ref;
        internal IntPtr _release;
        internal IntPtr _get_refct;

        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
#if !DEBUG
        [SuppressUnmanagedCodeSecurity]
#endif
        public delegate int add_ref_delegate(cef_base_t* self);

        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
#if !DEBUG
        [SuppressUnmanagedCodeSecurity]
#endif
        public delegate int release_delegate(cef_base_t* self);

        [UnmanagedFunctionPointer(libcef.CEF_CALLBACK)]
#if !DEBUG
        [SuppressUnmanagedCodeSecurity]
#endif
        public delegate int get_refct_delegate(cef_base_t* self);
    }
}
