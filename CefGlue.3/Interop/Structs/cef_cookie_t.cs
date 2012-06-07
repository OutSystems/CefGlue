//
// This file manually written from cef/include/internal/cef_types.h.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    internal struct cef_cookie_t
    {
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_cookie_t_other
    {
        public cef_string_t name;
        public cef_string_t value;
        public cef_string_t domain;
        public cef_string_t path;
        public bool_t secure;
        public bool_t httponly;
        public cef_time_t_other creation;
        public cef_time_t_other last_access;
        public bool_t has_expires;
        public cef_time_t_other expires;

        internal static void Clear(cef_cookie_t_other* ptr)
        {
            libcef.string_clear(&ptr->name);
            libcef.string_clear(&ptr->value);
            libcef.string_clear(&ptr->domain);
            libcef.string_clear(&ptr->path);
        }

        #region Alloc & Free
        private static int _sizeof;

        static cef_cookie_t_other()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_cookie_t_other));
        }

        public static cef_cookie_t_other* Alloc()
        {
            var ptr = (cef_cookie_t_other*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_cookie_t_other();
            return ptr;
        }

        public static void Free(cef_cookie_t_other* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_cookie_t_mac
    {
        public cef_string_t name;
        public cef_string_t value;
        public cef_string_t domain;
        public cef_string_t path;
        public bool_t secure;
        public bool_t httponly;
        public cef_time_t_mac creation;
        public cef_time_t_mac last_access;
        public bool_t has_expires;
        public cef_time_t_mac expires;

        internal static void Clear(cef_cookie_t_mac* ptr)
        {
            libcef.string_clear(&ptr->name);
            libcef.string_clear(&ptr->value);
            libcef.string_clear(&ptr->domain);
            libcef.string_clear(&ptr->path);
        }

        #region Alloc & Free
        private static int _sizeof;

        static cef_cookie_t_mac()
        {
            _sizeof = Marshal.SizeOf(typeof(cef_cookie_t_mac));
        }

        public static cef_cookie_t_mac* Alloc()
        {
            var ptr = (cef_cookie_t_mac*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_cookie_t_mac();
            return ptr;
        }

        public static void Free(cef_cookie_t_mac* ptr)
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
        #endregion
    }
}
