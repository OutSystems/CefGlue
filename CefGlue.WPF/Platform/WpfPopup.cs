using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.WPF.Platform
{
    /// <summary>
    /// The WPF popup wrapper.
    /// </summary>
    internal class WpfPopup : WpfControl, IOffScreenPopupHost
    {
        private readonly Popup _popup;

        public WpfPopup(Popup popup) : base(popup)
        {
            _popup = popup;
        }

        protected override IInputElement MousePositionReferential => _popup.PlacementTarget;

        public int Width => (int) _popup.Width;

        public int Height => (int)_popup.Height;

        public int OffsetX => (int)_popup.HorizontalOffset;

        public int OffsetY => (int)_popup.VerticalOffset;

        public void MoveAndResize(int x, int y, int width, int height)
        {
            _popup.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, 
                new Action(() =>
                {
                    _popup.HorizontalOffset = x;
                    _popup.VerticalOffset = y;
                    _popup.Width = width;
                    _popup.Height = height;
                }));
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
            _popup.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    _popup.IsOpen = isOpen;
                }));
        }

        protected override void SetContent(Image image)
        {
            _popup.Child = image;
        }
    }
}
