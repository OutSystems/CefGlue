using System.Windows.Controls.Primitives;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.WPF.Platform
{
    internal class WpfPopup : WpfControl, IPopup
    {
        private readonly Popup _popup;

        public WpfPopup(Popup popup) : base(popup)
        {
            _popup = popup;
        }

        public int Width { get => (int) _popup.Width; set => _popup.Width = value; }

        public int Height { get => (int)_popup.Height; set => _popup.Height = value; }

        public int OffsetX { get => (int)_popup.HorizontalOffset; set => _popup.HorizontalOffset = value; }

        public int OffsetY { get => (int)_popup.VerticalOffset; set => _popup.VerticalOffset = value; }

        public void Show()
        {
            _popup.Dispatcher.Invoke(() => _popup.IsOpen = true);
        }
    }
}
