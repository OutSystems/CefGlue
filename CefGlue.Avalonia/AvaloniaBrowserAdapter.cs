using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia
{
    internal class AvaloniaBrowserAdapter : CommonBrowserAdapter
    {
        protected override string Name => "AvaloniaCefBrowser";

        private readonly TemplatedControl _control;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupBitmap;

        public AvaloniaBrowserAdapter(TemplatedControl control, ILogger logger) : base(new AvaloniaControl(control), logger)
        {
            _control = control;
            _popup = CreatePopup();
            Popup = new AvaloniaControl(_popup);

            // TODO avalonia port
            // KeyboardNavigation.SetAcceptsReturn(this, true);
        }

        protected override UIControl Popup { get; }

        public Image BrowserImage
        {
            get
            {
                return ((AvaloniaRenderHandler) _renderHandler)?.Image;
            }
            set
            {
                _renderHandler = new AvaloniaRenderHandler(value, _logger);
            }
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (BrowserImage != null)
                {
                    BrowserImage.Source = null;
                }

                if (_popupImage != null)
                {
                    _popupImage = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPopupShow(bool show)
        {
            //if (_popup == null)
            //{
            //    return;
            //}

            //_mainUiDispatcher.Post(() => _popup.IsOpen = show);
        }

        protected override void OnPopupSizeChanged(CefRectangle rect)
        {
            //_mainUiDispatcher.Post(
            //    () =>
            //    {
            //        _popupRenderHandler.Dispose();
            //        _popupRenderHandler = new RenderHandler(_popupImage, _logger);

            //        _popupBitmap = new WriteableBitmap(new PixelSize(rect.Width, rect.Height), new Vector(96, 96), PixelFormat.Bgra8888);

            //        _popupImage.Source = _popupBitmap;

            //        _popup.Width = rect.Width;
            //        _popup.Height = rect.Height;
            //        _popup.HorizontalOffset = rect.X;
            //        _popup.VerticalOffset = rect.Y;

                    
                    
            //    });
        }

        private Popup CreatePopup()
        {
            _popupImage = CreatePopupImage();

            return new Popup
            {
                Child = _popupImage,
                PlacementTarget = _control,
                PlacementMode = PlacementMode.Bottom
            };
        }

        private Image CreatePopupImage()
        {
            return new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Source = _popupBitmap
            };
        }
    }
}
