//
// This file manually written from cef/include/internal/cef_types.h.
//
using System.Runtime.InteropServices;

namespace Xilium.CefGlue.Interop
{
    /// <summary>
    ///
    /// Structure containing shared texture information for the OnAcceleratedPaint
    /// callback. Resources will be released to the underlying pool for reuse when
    /// the callback returns from client code.
    ///
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    public unsafe struct cef_accelerated_paint_info_t
    {
        public nuint shared_texture_handle;
        public CefColorType format;
    }
}
