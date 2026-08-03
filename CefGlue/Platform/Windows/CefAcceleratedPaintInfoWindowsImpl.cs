using System;
using Xilium.CefGlue.Interop;

namespace Xilium.CefGlue.Platform;

internal sealed unsafe class CefAcceleratedPaintInfoWindowsImpl : CefAcceleratedPaintInfo
{
    private cef_accelerated_paint_info_t_windows* _self;


    internal CefAcceleratedPaintInfoWindowsImpl(cef_accelerated_paint_info_t* ptr)
    {
        _self = (cef_accelerated_paint_info_t_windows*)ptr;

        Format = _self->format;
        Extra = CefAcceleratedPaintInfoCommon.FromNative(_self->extra);
    }

    public override CefColorType Format { get; }
    public override CefAcceleratedPaintInfoCommon Extra { get; }

    public override ulong Modifier => throw new PlatformNotSupportedException();
    public override int PlaneCount => throw new PlatformNotSupportedException();
    public override CefAcceleratedPaintNativePixmapPlane[] Planes => throw new PlatformNotSupportedException();

    // TODO hgo: Noy sure about this
    public override IntPtr SharedTexture
    {
        get
        {
            ThrowIfDisposed();
            return _self->shared_texture_handle;
        }
    }
}
