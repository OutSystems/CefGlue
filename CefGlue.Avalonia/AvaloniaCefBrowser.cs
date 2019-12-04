using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Avalonia
{
    /// <summary>
    /// The Avalonia CEF browser.
    /// </summary>
    public class AvaloniaCefBrowser : BaseCefBrowser
    {
        static AvaloniaCefBrowser()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX && !CefRuntimeLoader.IsLoaded) { 
                CefRuntimeLoader.Load(new AvaloniaBrowserProcessHandler());
            }
        }

        public AvaloniaCefBrowser()
        {
            this.GetPropertyChangedObservable(Control.TransformedBoundsProperty).Subscribe(OnTransformedBoundsChanged);
        }

        private void OnTransformedBoundsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            var root = this.GetVisualRoot();
            if (root != null)
            {
                var size = Bounds.Size;
                var position = this.TranslatePoint(new Point(), root);
                if (position != null)
                {
                    CreateOrUpdateBrowser((int)position.Value.X, (int)position.Value.Y, (int)size.Width, (int)size.Height);
                }
            }
        }

        internal override CommonBrowserAdapter CreateAdapter()
        {
            var controlAdapter = new AvaloniaControl(this, image =>
            {
                LogicalChildren.Add(image);
                VisualChildren.Add(image);
            });

            var popup = new ExtendedAvaloniaPopup
            {
                PlacementTarget = this
            };
            var popupAdapter = new AvaloniaPopup(popup);

            return new CommonBrowserAdapter(this, nameof(AvaloniaCefBrowser), controlAdapter, popupAdapter, _logger);
        }
    }
}
