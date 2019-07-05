using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.WPF
{
    internal class WpfRenderHandler : RenderHandler
    {
        private WriteableBitmap _bitmap;
        
        public WpfRenderHandler(Image image, ILogger logger) : base(logger)
        {
            Image = image;
        }

        public Image Image { get; }

        public override void Dispose()
        {
        }

        public bool AllowsTransparency { get; set; }

        public override void Paint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            Image.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate
            {
                // Actual browser size changed check.
                if (_sizeChanged && (width != Width || height != Height))
                {
                    return;
                }

                try
                {
                    if (_sizeChanged)
                    {
                        _bitmap = new WriteableBitmap(Width, Height, 96, 96, AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32, null);
                        Image.Source = _bitmap;

                        _sizeChanged = false;
                    }

                    if (_bitmap != null)
                    {
                        InnerPaint(width, height, dirtyRects, buffer);
                    }
                }
                catch (Exception e)
                {
                    HandleException(e);
                }
            }));
        }

        private void InnerPaint(int browserWidth, int browserHeight, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            _logger.Debug("DoRenderBrowser() Bitmap H{0}xW{1}, Browser H{2}xW{3}", _bitmap.Height, _bitmap.Width, browserHeight, browserWidth);

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            foreach (CefRectangle dirtyRect in dirtyRects)
            {
                _logger.Debug(string.Format("Dirty rect [{0},{1},{2},{3}]", dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height));

                if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                {
                    continue;
                }

                // If the window has been resized, make sure we never try to render too much
                int adjustedWidth = (int)dirtyRect.Width;
                //if (dirtyRect.X + dirtyRect.Width > (int) bitmap.Width)
                //{
                //    adjustedWidth = (int) bitmap.Width - (int) dirtyRect.X;
                //}

                int adjustedHeight = (int)dirtyRect.Height;
                //if (dirtyRect.Y + dirtyRect.Height > (int) bitmap.Height)
                //{
                //    adjustedHeight = (int) bitmap.Height - (int) dirtyRect.Y;
                //}

                // Update the dirty region
                Int32Rect sourceRect = new Int32Rect((int)dirtyRect.X, (int)dirtyRect.Y, adjustedWidth, adjustedHeight);
                _bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, (int)dirtyRect.X, (int)dirtyRect.Y);

                // 			int adjustedWidth = browserWidth;
                // 			if (browserWidth > (int)bitmap.Width)
                // 				adjustedWidth = (int)bitmap.Width;
                // 
                // 			int adjustedHeight = browserHeight;
                // 			if (browserHeight > (int)bitmap.Height)
                // 				adjustedHeight = (int)bitmap.Height;
                // 
                // 			int sourceBufferSize = browserWidth * browserHeight * 4;
                // 			int stride = browserWidth * 4;
                // 
                // 			Int32Rect sourceRect = new Int32Rect(0, 0, adjustedWidth, adjustedHeight);
                // 			bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, 0, 0);
            }
        }
    }
}
