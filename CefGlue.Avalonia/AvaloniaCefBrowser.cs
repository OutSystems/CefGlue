using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Xilium.CefGlue.Avalonia.Platform;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Platform;

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
                var position = this.TranslatePoint(new global::Avalonia.Point(), root);
                if (position != null)
                {
                    CreateOrUpdateBrowser((int)position.Value.X, (int)position.Value.Y, (int)size.Width, (int)size.Height);
                }
            }
        }

        internal override Common.Platform.IControl CreateControl()
        {
            return new AvaloniaControl(this, null);
        }

        internal override IPopup CreatePopup()
        {
            var popup = new ExtendedAvaloniaPopup
            {
                PlacementTarget = this
            };
            return new AvaloniaPopup(popup, null);
        }
    }
}
