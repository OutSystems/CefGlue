using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Styling;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class ExtendedAvaloniaContextMenu : ContextMenu, IStyleable
    {
        private Point _position;
        
        Type IStyleable.StyleKey => typeof(ContextMenu);

        public void Open(Control placementTarget, Point position)
        {
            _position = position;
            Open(placementTarget);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (_position != default)
            {
                var popup = (PopupRoot)e.Root;
                popup.ConfigurePosition(
                    ((Popup)Parent).PlacementTarget,
                    PlacementMode.AnchorAndGravity,
                    _position,
                    PopupPositioningEdge.TopLeft,
                    PopupPositioningEdge.BottomRight);
            }
            base.OnAttachedToVisualTree(e);
        }
    }
}
