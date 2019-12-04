using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal interface IControl
    {
        event Action GotFocus;
        event Action LostFocus;

        event UIControl.KeyEventHandler KeyDown;
        event UIControl.KeyEventHandler KeyUp;
        event UIControl.TextInputEventHandler TextInput;

        event Action<IControl, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed;
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
        IntPtr? GetHostWindowHandle();
        IntPtr? GetHostViewHandle();
        Point PointToScreen(Point point, float deviceScaleFactor);
        void SetCursor(IntPtr cursorHandle);
        void SetTooltip(string text);

        void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback);
        void CloseContextMenu();

        BuiltInRenderHandler CreateRenderHandler();
    }
}