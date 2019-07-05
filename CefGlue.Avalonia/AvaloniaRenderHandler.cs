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
    internal class AvaloniaRenderHandler : RenderHandler
    {
        private WriteableBitmap _bitmap;

        public AvaloniaRenderHandler(Image image, ILogger logger) : base(logger)
        {
            Image = image;
        }

        public Image Image { get; }

        public override void Paint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Actual browser size changed check.
                if (_sizeChanged && (width != Width || height != Height))
                    return;

                try
                {
                    if (_sizeChanged)
                    {
                        // TODO handle transparency
                        _bitmap?.Dispose();
                        _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
                        Image.Source = _bitmap;

                        _sizeChanged = false;
                    }

                    if (_bitmap != null)
                    {
                        InnerPaint(buffer, width, height, dirtyRects);

                        Image.InvalidateVisual();
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            });
        }

        private void InnerPaint(IntPtr sourceBuffer, int browserWidth, int browserHeight, CefRectangle[] dirtyRects)
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
        }
    }
}
