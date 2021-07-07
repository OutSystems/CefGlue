using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    public delegate void KeyEventHandler(CefKeyEvent e, out bool handled);
    public delegate void TextInputEventHandler(string text, out bool handled);

    internal interface IOffScreenControlHost : IControl
    {
        event Action LostFocus;

        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;
        event TextInputEventHandler TextInput;

        event Action<IOffScreenControlHost, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed;
        event Action<CefMouseEvent, CefMouseButtonType> MouseButtonReleased;
        event Action<CefMouseEvent> MouseLeave;
        event Action<CefMouseEvent> MouseMoved;
        event Action<CefMouseEvent, int, int> MouseWheelChanged;

        event Action<CefMouseEvent, CefDragData, CefDragOperationsMask> DragEnter;
        event Action<CefMouseEvent, CefDragOperationsMask> DragOver;
        event Action DragLeave;
        event Action<CefMouseEvent, CefDragOperationsMask> Drop;

        event Action<float> ScreenInfoChanged;
        event Action<bool> VisibilityChanged;

        void Focus();
        CefPoint PointToScreen(CefPoint point, float deviceScaleFactor);

        Task<CefDragOperationsMask> StartDrag(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y);
        void UpdateDragCursor(CefDragOperationsMask allowedOps);

        OffScreenRenderSurface RenderSurface { get; }
    }
}