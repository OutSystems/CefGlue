using System;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IControl
    {
        bool IsFocused { get; }

        event Action GotFocus;
        event UIControl.KeyEventHandler KeyDown;
        event UIControl.KeyEventHandler KeyUp;
        event Action LostFocus;
        event Action<IControl, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed;
        event Action<CefMouseEvent, CefMouseButtonType> MouseButtonReleased;
        event Action<CefMouseEvent> MouseLeave;
        event Action<CefMouseEvent> MouseMoved;
        event Action<CefMouseEvent, int, int> MouseWheelChanged;
        event UIControl.TextInputEventHandler TextInput;

        void Focus();
        IntPtr? GetHostWindowHandle();
        Point PointToScreen(Point point);
        void SetCursor(IntPtr cursorHandle);
        void SetTooltip(string text);

        BuiltInRenderHandler RenderHandler { get; }
    }
}