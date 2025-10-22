//
// This file manually written from: 
//  - cef/include/internal/cef_types_mac.h
//  - cef/include/internal/cef_types_linux.h
//  - cef/include/internal/cef_types_win.h

namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    internal struct cef_accelerated_paint_info_t
    {
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_info_t_mac
    {
        public UIntPtr size;
        public IntPtr shared_texture_io_surface;
        public CefColorType format;
        public cef_accelerated_paint_info_common_t extra;
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_info_t_windows
    {
        public UIntPtr size;
        public IntPtr shared_texture_handle;
        public CefColorType format;
        public cef_accelerated_paint_info_common_t extra;
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_info_t_linux
    {
        public UIntPtr size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public cef_accelerated_paint_native_pixmap_plane_t[] planes;

        public int plane_count;
        public ulong modifier;
        public CefColorType format;
        public cef_accelerated_paint_info_common_t extra;
    }

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_native_pixmap_plane_t
    {
        public uint stride;
        public ulong offset;
        public ulong size;
        public int fd;
    }
}
