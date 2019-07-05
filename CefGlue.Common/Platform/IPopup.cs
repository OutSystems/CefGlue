namespace Xilium.CefGlue.Common.Platform
{
    internal interface IPopup : IControl
    {
        int Width { get; set; }

        int Height { get; set; }

        int OffsetX { get; set; }

        int OffsetY { get; set; }

        void Show();
    }
}
