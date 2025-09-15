//
// This file manually written from cef/include/internal/cef_types.h.
//

namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_urlparts_t
    {
        public UIntPtr size;
        public cef_string_t spec;
        public cef_string_t scheme;
        public cef_string_t username;
        public cef_string_t password;
        public cef_string_t host;
        public cef_string_t port;
        public cef_string_t origin;
        public cef_string_t path;
        public cef_string_t query;
        public cef_string_t fragment;

        #region Alloc & Free

        private static readonly int _sizeof = Marshal.SizeOf(typeof(cef_urlparts_t));

        public static cef_urlparts_t* Alloc()
        {
            var ptr = (cef_urlparts_t*)Marshal.AllocHGlobal(_sizeof);
            *ptr = new cef_urlparts_t { size = (UIntPtr)_sizeof };
            return ptr;
        }

        public static void Free(cef_urlparts_t* self)
        {
            if (self != null)
            {
                libcef.string_clear(&self->spec);
                libcef.string_clear(&self->scheme);
                libcef.string_clear(&self->username);
                libcef.string_clear(&self->password);
                libcef.string_clear(&self->host);
                libcef.string_clear(&self->port);
                libcef.string_clear(&self->origin);
                libcef.string_clear(&self->path);
                libcef.string_clear(&self->query);
                libcef.string_clear(&self->fragment);

                Marshal.FreeHGlobal((IntPtr)self);
            }
        }

        #endregion
    }
}
