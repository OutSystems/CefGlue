using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.WPF.Platform;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// The WPF CEF browser.
    /// </summary>
    public class WpfCefBrowser : BaseCefBrowser
    {
        private IDisposable[] _disposables;

        public WpfCefBrowser()
        {
            KeyboardNavigation.SetAcceptsReturn(this, true);
        }

        internal override CommonBrowserAdapter CreateAdapter()
        {
            var image = CreateImage();
            Content = image;

            var popupImage = CreateImage();
            var popup = new Popup
            {
                PlacementTarget = this,
                Placement = PlacementMode.Relative,
                Child = popupImage
            };

            var renderHandler = new WpfRenderHandler(image, _logger);
            var controlAdapter = new WpfControl(this, renderHandler);

            var popupRenderHandler = new WpfRenderHandler(popupImage, _logger);
            var popupAdapter = new WpfPopup(popup, popupRenderHandler);

            _disposables = new IDisposable[] { controlAdapter, popupAdapter };

            var adapter = new CommonBrowserAdapter(this, nameof(WpfCefBrowser), controlAdapter, popupAdapter, _logger);
            adapter.AllowsTransparency = true;
            return adapter;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #endregion

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            CreateOrUpdateBrowser((int)size.Width, (int)size.Height);
            return size;
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            return image;
        }
    }
}
