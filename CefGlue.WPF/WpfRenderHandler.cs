using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// The WPF builtin render 
    /// </summary>
    internal class WpfRenderHandler : BuiltInRenderHandler
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, int length);

        private readonly object _renderLock = new object();

        private WriteableBitmap _bitmap;

        private MemoryMappedFile _mappedFile;
        private MemoryMappedViewAccessor _viewAccessor;

        public WpfRenderHandler(Image image, ILogger logger) : base(logger)
        {
            Image = image;
        }

        public Image Image { get; }

        public override void Dispose()
        {
            ReleaseMemoryMap();
            _bitmap = null;
            GC.SuppressFinalize(this);
        }

        public bool AllowsTransparency { get; set; } = true;

        private void ReleaseMemoryMap()
        {
            lock (_renderLock)
            {
                _viewAccessor?.Dispose();
                _viewAccessor = null;
                _mappedFile?.Dispose();
                _mappedFile = null;
            }
        }

        protected override void InnerPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            lock(_renderLock)
            {
                var pixelFormat = AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32;
                var pixels = width * height;
                var bytesPerPixel = (pixelFormat.BitsPerPixel / 8);
                var byteCount = pixels * bytesPerPixel;

                if (_viewAccessor == null || _viewAccessor.Capacity < byteCount)
                {
                    ReleaseMemoryMap();

                    // needs new buffer or buffer size must increase
                    _mappedFile = MemoryMappedFile.CreateNew(null, byteCount, MemoryMappedFileAccess.ReadWrite);
                    _viewAccessor = _mappedFile.CreateViewAccessor();
                }

                var sourceBuffer = _viewAccessor.SafeMemoryMappedViewHandle;

                // copy pixels to our buffer, which we will then use to update the wpf bitmap (on the UI thread)
                CopyMemory(sourceBuffer.DangerousGetHandle(), buffer, byteCount);

                Image.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    lock (_renderLock)
                    {
                        // quick size check - actual browser size changed?
                        if (width != Width || height != Height)
                        {
                            return;
                        }

                        if (sourceBuffer.IsInvalid || sourceBuffer.IsClosed)
                        {
                            // buffer is not valid anymore, bail-out
                            return;
                        }

                        try
                        {
                            if (_bitmap == null || _bitmap.PixelWidth != width || _bitmap.PixelHeight != height)
                            {
                                _bitmap = new WriteableBitmap(width, height, Dpi, Dpi, pixelFormat, null);
                                Image.Source = _bitmap;
                            }

                            InnerPaint(width, height, bytesPerPixel, dirtyRects, sourceBuffer.DangerousGetHandle());
                        }
                        catch (Exception e)
                        {
                            HandleException(e);
                        }
                    }
                }));
            }
        }

        private void InnerPaint(int browserWidth, int browserHeight, int bytesPerPixel, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * bytesPerPixel;
            int sourceBufferSize = stride * browserHeight;

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            _bitmap.Lock();

            foreach (var dirtyRect in dirtyRects)
            {
   
                if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                {
                    continue;
                }
                
                // Update the dirty region
                var sourceRect = new Int32Rect(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);
                _bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, dirtyRect.X, dirtyRect.Y);

            }

            _bitmap.Unlock();
        }
    }
}
