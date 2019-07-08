using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IPopup : IControl
    {
        int Width { get; }

        int Height { get; }

        int OffsetX { get; }

        int OffsetY { get; }

        void Show();

        void MoveAndResize(int x, int y, int width, int height);
    }
}
