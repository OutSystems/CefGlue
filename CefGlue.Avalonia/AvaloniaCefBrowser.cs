using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
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
        private IPlatformHandle _handle;

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
            if (_handle == null)
            {
                throw new InvalidOperationException("Handle should not be null");
            }

            return new AvaloniaControl(this, _handle, image =>
            {
                LogicalChildren.Add(image);
                VisualChildren.Add(image);
                InvalidateArrange();
            });
        }

        internal override IPopup CreatePopup()
        {
            var popup = new ExtendedAvaloniaPopup
            {
                PlacementTarget = this
            };
            return new AvaloniaPopup(popup, _handle);
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle handle)
        {
            _handle = handle;
            var bounds = TransformedBounds.Value.Bounds;
            CreateOrUpdateBrowser((int) bounds.X, (int) bounds.Y, (int) bounds.Width, (int) bounds.Height);

            return base.CreateNativeControlCore(handle);
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            // TODO
        }
    }
}
