using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia builtin render.
    /// </summary>
    internal class AvaloniaRenderHandler : BuiltInRenderHandler
    {
        private WriteableBitmap _bitmap;
        private bool _disposed;

        public AvaloniaRenderHandler(Image image, ILogger logger) : base(logger)
        {
            Image = image;
        }

        public Image Image { get; }

        protected override void InnerPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            // TODO handle cases where buffer might be freed

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_disposed)
                {
                    return;
                }

                // Actual browser size changed check.
                if (width != Width || height != Height)
                {
                    return;
                }

                try
                {
                    if (_bitmap == null || _bitmap.PixelSize.Width != width || _bitmap.PixelSize.Height != height)
                    {
                        // TODO handle transparency
                        _bitmap?.Dispose();
                        _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(Dpi, Dpi), PixelFormat.Bgra8888);
                        Image.Source = _bitmap;
                    }

                    InternalPaint(buffer, width, height, dirtyRects);
                    Image.InvalidateVisual();
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            });
        }

        private void InternalPaint(IntPtr sourceBuffer, int browserWidth, int browserHeight, CefRectangle[] dirtyRects)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("Paint() Bitmap H{0}xW{1}, Browser H{2}xW{3}", _bitmap.Size.Height, _bitmap.Size.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            // TODO avalonia port - render only dirty regions
            // bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);
            using (var l = _bitmap.Lock())
            {
                byte[] managedArray = new byte[sourceBufferSize];

                Marshal.Copy(sourceBuffer, managedArray, 0, sourceBufferSize);
                Marshal.Copy(managedArray, 0, l.Address, sourceBufferSize);
            }
        }

        public override void Dispose()
        {
            _bitmap?.Dispose();
            _bitmap = null;
            _disposed = true;
        }
    }
}
