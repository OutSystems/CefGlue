using System.Windows;
using System.Windows.Input;

namespace Xilium.CefGlue.WPF
{
    internal static class InputExtensions
    {

        public static CefMouseEvent AsCefMouseEvent(this MouseEventArgs eventArgs, IInputElement mouseCoordinatesReferencial)
        {
            var cursorPos = eventArgs.GetPosition(mouseCoordinatesReferencial);

            return new CefMouseEvent((int) cursorPos.X, (int) cursorPos.Y, Keyboard.Modifiers.AsCefKeyboardModifiers());
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
            var modifiers = eventArgs.KeyboardDevice.Modifiers.AsCefKeyboardModifiers();
            return new CefKeyEvent()
            {
                EventType = isKeyUp ? CefKeyEventType.KeyUp : CefKeyEventType.RawKeyDown,
                WindowsKeyCode = KeyInterop.VirtualKeyFromKey(eventArgs.Key == Key.System ? eventArgs.SystemKey : eventArgs.Key),
                NativeKeyCode = 0,
                IsSystemKey = eventArgs.Key == Key.System,
                Modifiers = modifiers
            };
        }

        public static CefEventFlags AsCefMouseModifiers(this MouseEventArgs eventArgs)
        {
            CefEventFlags modifiers = new CefEventFlags();
            
            if (eventArgs.LeftButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.LeftMouseButton;

            if (eventArgs.MiddleButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.MiddleMouseButton;

            if (eventArgs.RightButton == MouseButtonState.Pressed)
                modifiers |= CefEventFlags.RightMouseButton;

            return modifiers;
        }

        public static CefEventFlags AsCefKeyboardModifiers(this ModifierKeys keyboardModifiers)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (keyboardModifiers.HasFlag(ModifierKeys.Alt))
                modifiers |= CefEventFlags.AltDown;

            if (keyboardModifiers.HasFlag(ModifierKeys.Control))
                modifiers |= CefEventFlags.ControlDown;

            if (keyboardModifiers.HasFlag(ModifierKeys.Shift))
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }
    }
}
