namespace Xilium.CefGlue;

using Xilium.CefGlue.Interop;

public class CefAcceleratedPaintNativePixmapPlane
{
    internal static CefAcceleratedPaintNativePixmapPlane FromNative(cef_accelerated_paint_native_pixmap_plane_t plane)
    {
        return new CefAcceleratedPaintNativePixmapPlane
        {
            Stride = plane.stride,
            Offset = plane.offset,
            Size = plane.size,
            FileDescriptor = plane.fd,
        };
    }

    public uint Stride { get; init; }
    public ulong Offset { get; init; }
    public ulong Size { get; init; }
    public int FileDescriptor { get; init; }
}
