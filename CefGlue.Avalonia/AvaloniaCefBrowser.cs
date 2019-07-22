using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia CEF browser.
    /// </summary>
    public class AvaloniaCefBrowser : BaseCefBrowser
    {
        internal override CommonBrowserAdapter CreateAdapter()
        {
            var image = CreateImage();
            VisualChildren.Add(image);

            var popupImage = CreateImage();
            var popup = new ExtendedAvaloniaPopup
            {
                Content = popupImage,
                PlacementTarget = this
            };

            var renderHandler = new AvaloniaRenderHandler(image, _logger);
            var controlAdapter = new AvaloniaControl(image, renderHandler); // use the image, otherwise some behaviors won't work as expected (eg: cursors do not change)

            var popupRenderHandler = new AvaloniaRenderHandler(popupImage, _logger);
            var popupAdapter = new AvaloniaPopup(popup, popupRenderHandler);

            return new CommonBrowserAdapter(this, nameof(AvaloniaCefBrowser), controlAdapter, popupAdapter, _logger);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            CreateOrUpdateBrowser((int) size.Width, (int) size.Height);
            return size;
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            return new Image()
            {
                Focusable = false,
                Stretch = Stretch.None,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
            };
        }
    }
}
