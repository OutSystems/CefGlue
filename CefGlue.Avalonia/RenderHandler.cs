using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Avalonia
{
    internal class RenderHandler
    {
        private WriteableBitmap _bitmap;
        private Dispatcher _uiDispatcher;        
        private ILogger _logger;

        private bool _sizeChanged;
        private int _width;
        private int _height;

        public RenderHandler(Dispatcher uiDispatcher, Image image, WriteableBitmap bitmap, ILogger logger)
        {
            _uiDispatcher = uiDispatcher;
            _bitmap = bitmap;
            _logger = logger;
            Image = image;
        }

        public event Action<Exception> ExceptionOcurred;

        public Image Image { get; private set; }

        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    _sizeChanged = true;
                }
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    _sizeChanged = true;
                }
            }
        }

        public void Paint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            _uiDispatcher.InvokeAsync(() =>
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
                        Paint(_bitmap, buffer, width, height, dirtyRects);

                        Image.InvalidateVisual();
                    }
                }
                catch(Exception e)
                {
                    ExceptionOcurred?.Invoke(e);
                }
            });
        }

        private void Paint(WriteableBitmap bitmap, IntPtr sourceBuffer, int browserWidth, int browserHeight, CefRectangle[] dirtyRects)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("Paint() Bitmap H{0}xW{1}, Browser H{2}xW{3}", bitmap.Size.Height, bitmap.Size.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            // TODO avalonia port - render only dirty regions
            // bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);
            using (var l = bitmap.Lock())
            {
                byte[] managedArray = new byte[sourceBufferSize];

                Marshal.Copy(sourceBuffer, managedArray, 0, sourceBufferSize);
                Marshal.Copy(managedArray, 0, l.Address, sourceBufferSize);
            }
        }
    }
}
