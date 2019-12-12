using System;
using System.Collections.Generic;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal abstract class UIControl : IControl
    {
        public delegate void KeyEventHandler(CefKeyEvent e, out bool handled);
        public delegate void TextInputEventHandler(string text, out bool handled);

        public abstract BuiltInRenderHandler CreateRenderHandler();

        public abstract Point PointToScreen(Point point, float deviceScaleFactor);

        public abstract IntPtr? GetHostWindowHandle();

        public virtual IntPtr? GetHostViewHandle() => GetHostWindowHandle();

        public abstract void Focus();

        public abstract void SetCursor(IntPtr cursorHandle);

        public abstract void SetTooltip(string text);

        public abstract void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback);
        public abstract void CloseContextMenu();

        public event Action GotFocus;

        protected void TriggerGotFocus()
        {
            GotFocus?.Invoke();
        }

        public event Action LostFocus;

        protected void TriggerLostFocus()
        {
            LostFocus?.Invoke();
        }

        public event Action<CefMouseEvent> MouseMoved;

        protected void TriggerMouseMoved(CefMouseEvent e)
        {
            MouseMoved?.Invoke(e);
        }

        public event Action<CefMouseEvent> MouseLeave;

        protected void TriggerMouseLeave(CefMouseEvent e)
        {
            MouseLeave?.Invoke(e);
        }

        public event Action<IControl, CefMouseEvent, CefMouseButtonType, int> MouseButtonPressed;

        protected void TriggerMouseButtonPressed(IControl control, CefMouseEvent e, CefMouseButtonType button, int clickCount)
        {
            MouseButtonPressed?.Invoke(control, e, button, clickCount);
        }

        public event Action<CefMouseEvent, CefMouseButtonType> MouseButtonReleased;

        protected void TriggerMouseButtonReleased(CefMouseEvent e, CefMouseButtonType button)
        {
            MouseButtonReleased?.Invoke(e, button);
        }

        public event Action<CefMouseEvent, int, int> MouseWheelChanged;

        protected void TriggerMouseWheelChanged(CefMouseEvent e, int deltaX, int deltaY)
        {
            MouseWheelChanged?.Invoke(e, deltaX, deltaY);
        }

        public event KeyEventHandler KeyDown;

        protected void TriggerKeyDown(CefKeyEvent e, out bool handled)
        {
            handled = false;
            KeyDown?.Invoke(e, out handled);
        }

        public event KeyEventHandler KeyUp;

        protected void TriggerKeyUp(CefKeyEvent e, out bool handled)
        {
            handled = false;
            KeyUp?.Invoke(e, out handled);
        }

        public event TextInputEventHandler TextInput;

        protected void TriggerTextInput(string text, out bool handled)
        {
            handled = false;
            TextInput?.Invoke(text, out handled);
        }

        public event Action<CefMouseEvent, CefDragData, CefDragOperationsMask> DragEnter;

        protected void TriggerDragEnter(CefMouseEvent mouseEvent, CefDragData dragData, CefDragOperationsMask effects)
        {
            DragEnter?.Invoke(mouseEvent, dragData, effects);
        }

        public event Action<CefMouseEvent, CefDragOperationsMask> DragOver;

        protected void TriggerDragOver(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            DragOver?.Invoke(mouseEvent, effects);
        }

        public event Action DragLeave;

        protected void TriggerDragLeave()
        {
            DragLeave?.Invoke();
        }

        public event Action<CefMouseEvent, CefDragOperationsMask> Drop;

        protected void TriggerDrop(CefMouseEvent mouseEvent, CefDragOperationsMask effects)
        {
            Drop?.Invoke(mouseEvent, effects);
        }

        public event Action<float> ScreenInfoChanged;

        protected void TriggerScreenInfoChanged(float deviceScaleFactor)
        {
            ScreenInfoChanged?.Invoke(deviceScaleFactor);
        }

        public event Action<bool> VisibilityChanged;

        protected void TriggerVisibilityChanged(bool isVisible)
        {
            VisibilityChanged?.Invoke(isVisible);
        }
    }
}
