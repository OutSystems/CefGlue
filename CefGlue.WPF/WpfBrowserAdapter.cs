using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.WPF.Platform;

namespace Xilium.CefGlue.WPF
{
    internal class WpfBrowserAdapter : CommonBrowserAdapter
    {
        private readonly ContentControl _control;
        private Dispatcher _mainUiDispatcher;

        private Image _browserImage;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupBitmap;

        public WpfBrowserAdapter(ContentControl control, ILogger logger) : base(logger)
        {
            _control = control;
            _popup = CreatePopup();
            Control = new WpfControl(control);
            Popup = new WpfControl(_popup);

            KeyboardNavigation.SetAcceptsReturn(_control, true);
            _mainUiDispatcher = _control.Dispatcher;
        }

        protected override string Name => nameof(WpfBrowserAdapter);

        protected override UIControl Control { get; }

        protected override UIControl Popup { get; }

        protected override void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            _mainUiDispatcher.Invoke(new Action(() => _popup.IsOpen = show));
        }

        protected override void OnPopupSizeChanged(CefRectangle rect)
        {
            _mainUiDispatcher.Invoke(
                new Action(
                    () =>
                    {
                        _popupBitmap = null;
                        _popupBitmap = new WriteableBitmap(rect.Width, rect.Height, 96, 96, PixelFormats.Bgr32, null);

                        _popupImage.Source = _popupBitmap;

                        _popup.Width = rect.Width;
                        _popup.Height = rect.Height;
                        _popup.HorizontalOffset = rect.X;
                        _popup.VerticalOffset = rect.Y;
                    }));
        }

        private Popup CreatePopup()
        {
            _popupImage = CreatePopupImage();

            return new Popup
            {
                Child = _popupImage,
                PlacementTarget = _control,
                Placement = PlacementMode.Relative
            };
        }

        private Image CreatePopupImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Source = _popupBitmap
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            return image;
        }
    }
}