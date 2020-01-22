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
            var cursorPos = mousePositionReferential.IsAttachedToVisualTree ? eventArgs.GetPosition(mousePositionReferential) : new global::Avalonia.Point(0,0);

            return new CefMouseEvent((int) cursorPos.X, (int) cursorPos.Y, eventArgs.AsCefEventFlags());
        }

        /// <summary>
        /// Convert a mouse button into a cef mouse button.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static CefMouseButtonType AsCefMouseButtonType(this PointerEventArgs eventArgs)
        {
            switch (eventArgs.GetCurrentPoint(null).Properties.PointerUpdateKind.GetMouseButton())
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
            var modifiers = eventArgs.KeyModifiers.AsCefKeyboardModifiers();
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
        public static CefEventFlags AsCefEventFlags(this PointerEventArgs eventArgs)
        {
            switch (eventArgs.GetCurrentPoint(null).Properties.PointerUpdateKind.GetMouseButton())
            {
                case MouseButton.Middle:
                    return CefEventFlags.MiddleMouseButton;
                case MouseButton.Right:
                    return CefEventFlags.RightMouseButton;
                default:
                    return CefEventFlags.LeftMouseButton;
            }
        }

        /// <summary>
        /// Convert keyboard modifieers into cef flags.
        /// </summary>
        /// <param name="keyboardModifiers"></param>
        /// <returns></returns>
        public static CefEventFlags AsCefKeyboardModifiers(this KeyModifiers keyboardModifiers)
        {
            var modifiers = new CefEventFlags();

            if (keyboardModifiers.HasFlag(KeyModifiers.Alt))
                modifiers |= CefEventFlags.AltDown;

            if (keyboardModifiers.HasFlag(KeyModifiers.Control))
                modifiers |= CefEventFlags.ControlDown;

            if (keyboardModifiers.HasFlag(KeyModifiers.Shift))
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }
    }
}
