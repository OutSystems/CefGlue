using Xilium.CefGlue.Platform;

namespace Xilium.CefGlue;

using System;
using Xilium.CefGlue.Interop;

public abstract unsafe class CefAcceleratedPaintInfo
{
    private bool _disposed;

    internal static CefAcceleratedPaintInfo FromNative(cef_accelerated_paint_info_t* ptr)
    {
        return CefRuntime.Platform switch
        {
            CefRuntimePlatform.Windows => new CefAcceleratedPaintInfoWindowsImpl(ptr),
            CefRuntimePlatform.Linux   => new CefAcceleratedPaintInfoLinuxImpl(ptr),
            CefRuntimePlatform.MacOS   => new CefAcceleratedPaintInfoMacImpl(ptr),
            _                          => throw new NotSupportedException(),
        };
    }

    ~CefAcceleratedPaintInfo()
    {
        _disposed = true;
    }

    public abstract IntPtr SharedTexture { get; }
    public abstract CefColorType Format { get; }
    public abstract CefAcceleratedPaintInfoCommon Extra { get; }

    public abstract ulong Modifier { get; }
    public abstract int PlaneCount { get; }
    public abstract CefAcceleratedPaintNativePixmapPlane[] Planes { get; }

    protected void ThrowIfDisposed()
    {
        if (_disposed) throw ExceptionBuilder.ObjectDisposed();
    }
}
