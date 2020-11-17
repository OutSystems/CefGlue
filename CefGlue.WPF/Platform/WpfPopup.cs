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
    internal class WpfPopup : WpfOffScreenControlHost, IOffScreenPopupHost
    {
        public WpfPopup(Popup popup) : base(popup)
        {
        }

        private Popup Popup => (Popup) base._control;

        protected override IInputElement MousePositionReferential => Popup.PlacementTarget;

        public int Width => (int)Popup.Width;

        public int Height => (int)Popup.Height;

        public int OffsetX => (int)Popup.HorizontalOffset;

        public int OffsetY => (int)Popup.VerticalOffset;

        public void MoveAndResize(int x, int y, int width, int height)
        {
            Popup.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal, 
                new Action(() =>
                {
                    Popup.HorizontalOffset = x;
                    Popup.VerticalOffset = y;
                    Popup.Width = width;
                    Popup.Height = height;
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
            Popup.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    Popup.IsOpen = isOpen;
                }));
        }

        protected override void SetContent(Image image)
        {
            Popup.Child = image;
        }
    }
}
