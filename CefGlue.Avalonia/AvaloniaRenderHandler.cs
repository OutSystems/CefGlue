using System;
using System.Threading.Tasks;
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
        private IntPtr _destinationBuffer;

        public AvaloniaRenderHandler(Image image)
        {
            Image = image;
        }

        public override void Dispose()
        {
            base.Dispose();
            _bitmap?.Dispose();
            _bitmap = null;
        }

        public Image Image { get; }

        protected override int BytesPerPixel => 4;

        protected override int RenderedHeight => _bitmap?.PixelSize.Height ?? 0;

        protected override int RenderedWidth => _bitmap?.PixelSize.Width ?? 0;

        protected override Task ExecuteInUIThread(Action action)
        {
            return Dispatcher.UIThread.InvokeAsync(action);
        }

        protected override void CreateBitmap(int width, int height)
        {
            // TODO handle transparency
            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(Dpi, Dpi), PixelFormat.Bgra8888);
            Image.Source = _bitmap;
        }

        protected override Action BeginBitmapUpdate()
        {
            var lockedBuffer = _bitmap.Lock();
            _destinationBuffer = lockedBuffer.Address;
            return () =>
            {
                _destinationBuffer = IntPtr.Zero;
                lockedBuffer.Dispose();
                Image.InvalidateVisual();
            };
        }

        protected override void UpdateBitmap(IntPtr sourceBuffer, int sourceBufferSize, int stride, CefRectangle updateRegion)
        {
            unsafe
            {
                Buffer.MemoryCopy(sourceBuffer.ToPointer(), _destinationBuffer.ToPointer(), sourceBufferSize, sourceBufferSize);
            }
        }
    }
}
