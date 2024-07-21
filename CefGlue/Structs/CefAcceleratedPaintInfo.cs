using System.Runtime.InteropServices;
using Xilium.CefGlue;
using Xilium.CefGlue.Interop;

namespace CefGlue.Structs
{
    /// <summary>
    ///
    /// Structure containing shared texture information for the OnAcceleratedPaint
    /// callback. Resources will be released to the underlying pool for reuse when
    /// the callback returns from client code.
    ///
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = libcef.ALIGN)]
    public unsafe struct CefAcceleratedPaintInfo
    {
        public nuint shared_texture_handle;
        public CefColorType format;
    }
}
