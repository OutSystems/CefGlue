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
        // kAcceleratedPaintMaxPlanes in cef_types_linux.h
        public const int MaxPlanes = 4;

        public UIntPtr size;

        // The native field is an inline plane[MaxPlanes] array. It is spelled out as
        // individual fields because a C# array field would make this struct a managed
        // type, which can neither be pointed at nor laid out like the native struct.
        public cef_accelerated_paint_native_pixmap_plane_t plane0;
        public cef_accelerated_paint_native_pixmap_plane_t plane1;
        public cef_accelerated_paint_native_pixmap_plane_t plane2;
        public cef_accelerated_paint_native_pixmap_plane_t plane3;

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
