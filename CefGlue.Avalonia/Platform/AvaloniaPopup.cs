using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class AvaloniaPopup : AvaloniaControl, IPopup
    {
        private readonly Popup _popup;

        public AvaloniaPopup(Popup popup) : base(popup)
        {
            _popup = popup;
        }

        public int Width { get => (int) _popup.Width; set => _popup.Width = value; }

        public int Height { get => (int)_popup.Height; set => _popup.Height = value; }

        public int OffsetX { get => (int)_popup.HorizontalOffset; set => _popup.HorizontalOffset = value; }

        public int OffsetY { get => (int)_popup.VerticalOffset; set => _popup.VerticalOffset = value; }

        public void Show()
        {
            Dispatcher.UIThread.Post(() => _popup.IsOpen = true);
        }
    }
}
