using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// The WPF builtin surface 
    /// </summary>
    internal class WpfRenderSurface : OffScreenRenderSurface
    {
        private WriteableBitmap _bitmap;

        public WpfRenderSurface(Image image)
        {
            Image = image;
        }

        public Image Image { get; }

        public override bool AllowsTransparency => true;

        private PixelFormat PixelFormat => AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32;

        protected override int BytesPerPixel => PixelFormat.BitsPerPixel / 8;

        protected override bool UpdateDirtyRegionsOnly => true;

        protected override void CreateBitmap(int width, int height)
        {
            _bitmap = new WriteableBitmap(width, height, Dpi, Dpi, PixelFormat, null);
            Image.Source = _bitmap;
        }

        protected override Action BeginBitmapUpdate()
        {
            _bitmap.Lock();
            return () => _bitmap.Unlock();
        }

        protected override Task ExecuteInUIThread(Action action)
        {
            return Image.Dispatcher.InvokeAsync(action, DispatcherPriority.Render).Task;
        }

        protected override int RenderedHeight =>  _bitmap?.PixelHeight ?? 0;

        protected override int RenderedWidth => _bitmap?.PixelWidth ?? 0;

        protected override void UpdateBitmap(IntPtr sourceBuffer, int sourceBufferSize, int stride, CefRectangle updateRegion)
        {
            var sourceRect = new Int32Rect(updateRegion.X, updateRegion.Y, updateRegion.Width, updateRegion.Height);
            _bitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, updateRegion.X, updateRegion.Y);
        }
    }
}
