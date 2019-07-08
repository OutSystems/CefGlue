using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class AvaloniaPopup : AvaloniaControl, IPopup
    {
        private readonly Popup _popup;

        public AvaloniaPopup(Popup popup, RenderHandler renderHandler) : base(popup, renderHandler)
        {
            _popup = popup;
        }

        protected override IVisual MousePositionReferential => _popup.PlacementTarget;

        public int Width => (int)_popup.Width;

        public int Height => (int)_popup.Height;

        public int OffsetX => (int)_popup.HorizontalOffset;

        public int OffsetY => (int)_popup.VerticalOffset;

        public void MoveAndResize(int x, int y, int width, int height)
        {
            Dispatcher.UIThread.Post(() =>
            {
                _popup.HorizontalOffset = x;
                _popup.VerticalOffset = y;
                _popup.Width = width;
                _popup.Height = height;
            });
        }

        public void Open()
        {
            SetIsOpen(true);
        }

        public void Close()
        {
            SetIsOpen(false);
        }

        private void SetIsOpen(bool isOpen)
        {
            Dispatcher.UIThread.Post(() => _popup.IsOpen = isOpen);
        }
    }
}
