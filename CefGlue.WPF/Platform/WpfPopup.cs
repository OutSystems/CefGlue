using System.Windows.Controls.Primitives;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.WPF.Platform
{
    internal class WpfPopup : WpfControl, IPopup
    {
        private readonly Popup _popup;

        public WpfPopup(Popup popup, RenderHandler renderHandler) : base(popup, renderHandler)
        {
            _popup = popup;
        }

        public int Width => (int) _popup.Width;

        public int Height => (int)_popup.Height;

        public int OffsetX => (int)_popup.HorizontalOffset;

        public int OffsetY => (int)_popup.VerticalOffset;

        public void MoveAndResize(int x, int y, int width, int height)
        {
            _popup.Dispatcher.Invoke(() =>
            {
                _popup.HorizontalOffset = x;
                _popup.VerticalOffset = y;
                _popup.Width = width;
                _popup.Height = height;
            });
        }

        public void Show()
        {
            _popup.Dispatcher.Invoke(() => _popup.IsOpen = true);
        }   
    }
}
