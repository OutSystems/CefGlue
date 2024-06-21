using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// The Avalonia popup control wrapper.
    /// </summary>
    internal class AvaloniaPopup : AvaloniaOffScreenControlHost, IOffScreenPopupHost
    {
        private readonly ExtendedAvaloniaPopup _popup;

        public AvaloniaPopup(ExtendedAvaloniaPopup popup, IAvaloniaList<Visual> visualChildren) : 
            base(popup, visualChildren)
        {
            _popup = popup;
        }

        protected override Visual MousePositionReferential => _popup.PlacementTarget;

        public int Width => (int)_popup.Width;

        public int Height => (int)_popup.Height;

        public int OffsetX => _popup.Position.X;

        public int OffsetY => _popup.Position.Y;

        public void MoveAndResize(int x, int y, int width, int height)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var origin = _popup.PlacementTarget.PointToScreen(new global::Avalonia.Point(x, y));
                _popup.Position = new PixelPoint(origin.X, origin.Y);
                _popup.Width = width;
                _popup.Height = height;
            });
        }

        public void Open()
        {
            Dispatcher.UIThread.Post(() =>
            {
                _popup.Show(_popup.PlacementTarget.GetVisualRoot() as Window);
            });
        }

        public void Close()
        {
            Dispatcher.UIThread.Post(() => _popup.Hide());
        }
    }
}
