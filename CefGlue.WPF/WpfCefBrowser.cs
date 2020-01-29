using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;
using Xilium.CefGlue.WPF.Platform;

namespace Xilium.CefGlue.WPF
{
    /// <summary>
    /// The WPF CEF browser.
    /// </summary>
    public class WpfCefBrowser : BaseCefBrowser
    {
        private bool _hasLayoutUpdatedOnce = false;

        public WpfCefBrowser()
        {
            KeyboardNavigation.SetAcceptsReturn(this, true);

            LayoutUpdated += OnLayoutUpdated;
        }

        protected override bool AllowsTransparency => true;

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            if (_hasLayoutUpdatedOnce)
            {
                // skip until layout is updated, because before LayoutUpdated is called, 
                // the control coordinates relative to the window are unknown
                CreateOrUpdateBrowser(size);
            }
            return size;
        }

        private void OnLayoutUpdated(object sender, System.EventArgs e)
        {
            // when layout updated event is fired for the first time we can get the control coordinates relative to the window
            _hasLayoutUpdatedOnce = true;
            LayoutUpdated -= OnLayoutUpdated;
            CreateOrUpdateBrowser(RenderSize);
        }

        internal override IControl CreateControl()
        {
            return new WpfControl(this);
        }

        internal override IPopup CreatePopup()
        {
            var popup = new Popup
            {
                PlacementTarget = this,
                Placement = PlacementMode.Relative,
            };

            return new WpfPopup(popup);
        }

        private void CreateOrUpdateBrowser(Size size)
        {
            if (IsVisible)
            {
                var root = PresentationSource.FromVisual(this)?.RootVisual;
                if (root != null)
                {
                    var position = TransformToAncestor(root).Transform(new System.Windows.Point());
                    CreateOrUpdateBrowser((int)position.X, (int)position.Y, (int)size.Width, (int)size.Height);
                }
            }
        }
    }
}
