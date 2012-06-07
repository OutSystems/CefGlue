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
        public cef_string_t spec;
        public cef_string_t scheme;
        public cef_string_t username;
        public cef_string_t password;
        public cef_string_t host;
        public cef_string_t port;
        public cef_string_t path;
        public cef_string_t query;
    }
}
