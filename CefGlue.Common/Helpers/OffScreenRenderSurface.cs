using System;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Helpers
{
    /// <summary>
    /// Render surface that supports rendering into the browser contents into and image.
    /// </summary>
    internal abstract class OffScreenRenderSurface : IDisposable
    {
        protected const int DefaultDpi = 96;

        private int _width;
        private int _height;

        private readonly object _renderLock = new object();

        private MemoryMappedFile _mappedFile;
        private MemoryMappedViewAccessor _viewAccessor;

        public virtual void Dispose()
        {
            ReleaseMemoryMap();
            GC.SuppressFinalize(this);
        }

        public abstract bool AllowsTransparency { get; }

        public int Width => _width;

        public int Height => _height;

        public int ScaledWidth => (int)Math.Ceiling(DeviceScaleFactor * _width);

        public int ScaledHeight => (int)Math.Ceiling(DeviceScaleFactor * _height);

        public float DeviceScaleFactor { get; set; } = 1;

        protected float Dpi => DeviceScaleFactor * DefaultDpi;

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

        protected abstract int BytesPerPixel { get; }

        protected abstract int RenderedWidth { get; }
        protected abstract int RenderedHeight { get; }

        protected abstract void CreateBitmap(int width, int height);

        protected abstract Task ExecuteInUIThread(Action action);

        protected abstract void UpdateBitmap(IntPtr sourceBuffer, int sourceBufferSize, int stride, CefRectangle updateRegion);

        protected virtual bool UpdateDirtyRegionsOnly => false;

        protected abstract Action BeginBitmapUpdate();

        public Task Render(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (width != ScaledWidth || height != ScaledHeight)
            {
                return Task.CompletedTask;
            }

            lock (_renderLock)
            {
                var pixels = width * height;
                var bytesPerPixel = BytesPerPixel;
                var byteCount = pixels * bytesPerPixel;

                if (_viewAccessor == null || _viewAccessor.Capacity < byteCount)
                {
                    ReleaseMemoryMap();

                    // needs new buffer or buffer size must increase
                    _mappedFile = MemoryMappedFile.CreateNew(null, byteCount, MemoryMappedFileAccess.ReadWrite);
                    _viewAccessor = _mappedFile.CreateViewAccessor();
                }

                var imageBuffer = _viewAccessor.SafeMemoryMappedViewHandle;

                // copy pixels to our buffer, which we will then use to update the bitmap (on the UI thread)
                unsafe
                {
                    Buffer.MemoryCopy(buffer.ToPointer(), imageBuffer.DangerousGetHandle().ToPointer(), (long)imageBuffer.ByteLength, byteCount);
                }

                return ExecuteInUIThread(() =>
                {
                    lock (_renderLock)
                    {
                        // quick size check - actual browser size changed?
                        if (width != ScaledWidth || height != ScaledHeight)
                        {
                            return;
                        }

                        if (imageBuffer.IsInvalid || imageBuffer.IsClosed)
                        {
                            // buffer is not valid anymore, bail-out
                            return;
                        }

                        if (RenderedWidth != width || RenderedHeight != height)
                        {
                            CreateBitmap(width, height);
                        }

                        InnerRender(width, height, bytesPerPixel, dirtyRects, imageBuffer.DangerousGetHandle());
                    }
                });
            }
        }

        private void InnerRender(int browserWidth, int browserHeight, int bytesPerPixel, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * bytesPerPixel;
            int sourceBufferSize = stride * browserHeight;

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            var endBitmapUpdate = BeginBitmapUpdate();
            try
            {
                if (UpdateDirtyRegionsOnly)
                {
                    foreach (var dirtyRect in dirtyRects)
                    {

                        if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                        {
                            continue;
                        }

                        UpdateBitmap(sourceBuffer, sourceBufferSize, stride, dirtyRect);
                    }
                }
                else
                {
                    UpdateBitmap(sourceBuffer, sourceBufferSize, stride, new CefRectangle(0, 0, browserWidth, browserHeight));
                }
            }
            finally
            {
                endBitmapUpdate();
            }
        }

        public void Resize(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}
