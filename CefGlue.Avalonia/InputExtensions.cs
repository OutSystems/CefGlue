using Avalonia.Input;
using Avalonia.VisualTree;

namespace Xilium.CefGlue.Avalonia
{
    internal static class InputExtensions
    {
        /// <summary>
        /// Convert a mouse event args into a cef mouse event.
        /// </summary>
        /// <param name="eventArgs">The mouse event args</param>
        /// <param name="mouseCoordinatesReferencial">The element used as the positioning referential</param>
        /// <returns></returns>
        public static CefMouseEvent AsCefMouseEvent(this PointerEventArgs eventArgs, IVisual mousePositionReferential)
        {
            var cursorPos = eventArgs.GetPosition(mousePositionReferential);

            return new CefMouseEvent((int) cursorPos.X, (int) cursorPos.Y, eventArgs.InputModifiers.AsCefMouseModifiers());
        }

        /// <summary>
        /// Convert a mouse button into a cef mouse button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static CefMouseButtonType AsCefMouseButtonType(this MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Middle:
                    return CefMouseButtonType.Middle;
                case MouseButton.Right:
                    return CefMouseButtonType.Right;
                default:
                    return CefMouseButtonType.Left;
            }
        }

        /// <summary>
        /// Convert a key event into a cef key event.
        /// </summary>
        /// <param name="eventArgs"></param>
        /// <param name="isKeyUp"></param>
        /// <returns></returns>
        public static CefKeyEvent AsCefKeyEvent(this KeyEventArgs eventArgs, bool isKeyUp)
        {
            var modifiers = eventArgs.Modifiers.AsCefKeyboardModifiers();
            return new CefKeyEvent()
            {
                EventType = isKeyUp ? CefKeyEventType.KeyUp : CefKeyEventType.RawKeyDown,
                WindowsKeyCode = KeyInterop.VirtualKeyFromKey(eventArgs.Key),
                NativeKeyCode = (int) modifiers,
                IsSystemKey = eventArgs.Key == Key.System,
                Modifiers = modifiers
            };
        }

        /// <summary>
        /// Convert mouse modifiers into cef flags.
        /// </summary>
        /// <param name="keyboardModifiers"></param>
        /// <returns></returns>
        public static CefEventFlags AsCefMouseModifiers(this InputModifiers mouseModifiers)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (mouseModifiers == InputModifiers.LeftMouseButton)
                modifiers |= CefEventFlags.LeftMouseButton;

            if (mouseModifiers == InputModifiers.MiddleMouseButton)
                modifiers |= CefEventFlags.MiddleMouseButton;

            if (mouseModifiers == InputModifiers.RightMouseButton)
                modifiers |= CefEventFlags.RightMouseButton;

            return modifiers;
        }

        /// <summary>
        /// Convert keyboard modifieers into cef flags.
        /// </summary>
        /// <param name="keyboardModifiers"></param>
        /// <returns></returns>
        public static CefEventFlags AsCefKeyboardModifiers(this InputModifiers keyboardModifiers)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (keyboardModifiers == InputModifiers.Alt)
                modifiers |= CefEventFlags.AltDown;

            if (keyboardModifiers == InputModifiers.Control)
                modifiers |= CefEventFlags.ControlDown;

            if (keyboardModifiers == InputModifiers.Shift)
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }
    }
}
