using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IOffScreenPopupHost : IOffScreenControlHost
    {
        int Width { get; }

        int Height { get; }

        int OffsetX { get; }

        int OffsetY { get; }

        void Open();

        void Close();

        void MoveAndResize(int x, int y, int width, int height);
    }
}
