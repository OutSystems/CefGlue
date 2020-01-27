using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Xilium.CefGlue.WPF
{
    internal static class InputExtensions
    {
        /// <summary>
        /// Convert a mouse event args into a cef mouse event.
        /// </summary>
        /// <param name="eventArgs">The mouse event args</param>
        /// <param name="mouseCoordinatesReferencial">The element used as the positioning referential</param>
        /// <returns></returns>
        public static CefMouseEvent AsCefMouseEvent(this MouseEventArgs eventArgs, IInputElement mouseCoordinatesReferencial)
        {
            var cursorPos = eventArgs.GetPosition(mouseCoordinatesReferencial);
            var modifiers = CefEventFlags.None;

            if (eventArgs.LeftButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.LeftMouseButton;
            }
            if (eventArgs.RightButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.RightMouseButton;
            }
            if (eventArgs.MiddleButton == MouseButtonState.Pressed)
            {
                modifiers |= CefEventFlags.MiddleMouseButton;
            }

            return new CefMouseEvent((int) cursorPos.X, (int) cursorPos.Y, modifiers | Keyboard.Modifiers.AsCefKeyboardModifiers());
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

        /// <summary>
        /// Convert keyboard modifiers into cef flags.
        /// </summary>
        /// <param name="keyboardModifiers"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert a drag event args into a cef mouse event.
        /// </summary>
        /// <param name="eventArgs">The drag event args</param>
        /// <param name="mouseCoordinatesReferencial">The element used as the positioning referential</param>
        /// <returns></returns>
        public static CefMouseEvent AsCefMouseEvent(this DragEventArgs eventArgs, IInputElement mouseCoordinatesReferencial)
        {
            var cursorPos = eventArgs.GetPosition(mouseCoordinatesReferencial);

            return new CefMouseEvent((int)cursorPos.X, (int)cursorPos.Y, Keyboard.Modifiers.AsCefKeyboardModifiers());
        }

        /// <summary>
        /// Converts a drag drop effects to Cef Drag Operations
        /// </summary>
        /// <param name="dragDropEffects">The drag drop effects.</param>
        /// <returns></returns>
        public static CefDragOperationsMask AsCefDragOperationsMask(this DragDropEffects dragDropEffects)
        {
            var operations = CefDragOperationsMask.None;

            if (dragDropEffects.HasFlag(DragDropEffects.All))
            {
                operations |= CefDragOperationsMask.Every;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Copy))
            {
                operations |= CefDragOperationsMask.Copy;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Move))
            {
                operations |= CefDragOperationsMask.Move;
            }
            if (dragDropEffects.HasFlag(DragDropEffects.Link))
            {
                operations |= CefDragOperationsMask.Link;
            }

            return operations;
        }

        /// <summary>
        /// Gets the drag effects.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        public static DragDropEffects AsDragDropEffects(this CefDragOperationsMask mask)
        {
            if (mask.HasFlag(CefDragOperationsMask.Every))
            {
                return DragDropEffects.Scroll | DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
            }
            if (mask.HasFlag(CefDragOperationsMask.Copy))
            {
                return DragDropEffects.Copy;
            }
            if (mask.HasFlag(CefDragOperationsMask.Move))
            {
                return DragDropEffects.Move;
            }
            if (mask.HasFlag(CefDragOperationsMask.Link))
            {
                return DragDropEffects.Link;
            }
            return DragDropEffects.None;
        }

        /// <summary>
        /// Gets the drag data
        /// </summary>
        /// <param name="e">The <see cref="DragEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static CefDragData GetDragData(this DragEventArgs e)
        {
            var dragData = CefDragData.Create();

            // Files
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var filePath in files)
                {
                    var displayName = Path.GetFileName(filePath);
                    dragData.AddFile(filePath.Replace("\\", "/"), displayName);
                }
            }

            // TODO
            // Link/Url
            //var link = GetLink(e.Data);
            //if (!string.IsNullOrEmpty(link))
            //{
            //    dragData.SetLinkURL(link);
            //}

            // Text/HTML
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                dragData.SetFragmentText((string)e.Data.GetData(DataFormats.Text));

                var htmlData = (string)e.Data.GetData(DataFormats.Html);
                if (!string.IsNullOrEmpty(htmlData))
                {
                    dragData.SetFragmentHtml(htmlData);
                }
            }

            return dragData;
        }
    }
}
