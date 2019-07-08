using Avalonia.Input;
using Avalonia.VisualTree;

namespace Xilium.CefGlue.Avalonia
{
    internal static class InputExtensions
    {

        public static CefMouseEvent AsCefMouseEvent(this PointerEventArgs eventArgs, IVisual mousePositionReferential)
        {
            var cursorPos = eventArgs.GetPosition(mousePositionReferential);

            return new CefMouseEvent((int) cursorPos.X, (int) cursorPos.Y, eventArgs.InputModifiers.AsCefMouseModifiers());
        }

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
