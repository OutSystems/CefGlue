//
// This file manually written from cef/include/internal/cef_types.h.
//
namespace Xilium.CefGlue.Interop
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    internal unsafe struct cef_accelerated_paint_info_t
    {
        public IntPtr size;

        ///
        /// Handle for the shared texture IOSurface.
        ///
        public IntPtr shared_texture_io_surface;

        ///
        /// The pixel format of the texture.
        ///
        public CefColorType format;

        ///
        /// The extra common info.
        ///
        public cef_accelerated_paint_info_common_t extra;
    }
}
