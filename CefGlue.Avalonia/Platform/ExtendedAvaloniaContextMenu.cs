using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.LogicalTree;
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
            base.Open(placementTarget);
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            if (_position != default)
            {
                var popup = (Popup)Parent;
                popup.Opened += OnPopupOpened;
            }
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            var popup = (Popup)sender;
            popup.Host.ConfigurePosition(popup.PlacementTarget, PlacementMode.AnchorAndGravity, _position, PopupPositioningEdge.TopLeft, PopupPositioningEdge.BottomRight);
        }
    }
}
