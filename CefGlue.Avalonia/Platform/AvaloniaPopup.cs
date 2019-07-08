using Avalonia.Controls.Primitives;
using Avalonia.Threading;
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

        public void Show()
        {
            Dispatcher.UIThread.Post(() => _popup.IsOpen = true);
        }
    }
}
