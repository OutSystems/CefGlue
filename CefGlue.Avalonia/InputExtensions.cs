using System.IO;
using Avalonia;
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
        public static CefMouseEvent AsCefMouseEvent(this PointerEventArgs eventArgs, Visual mousePositionReferential)
        {
            var cursorPos = mousePositionReferential.IsAttachedToVisualTree() ? eventArgs.GetPosition(mousePositionReferential) : new Point(0,0);

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
            var flags = CefEventFlags.None;
            var properties = eventArgs.GetCurrentPoint(null).Properties;

            if (properties.IsLeftButtonPressed)
            {
                flags |= CefEventFlags.LeftMouseButton;
            }

            if (properties.IsMiddleButtonPressed)
            {
                flags |= CefEventFlags.MiddleMouseButton;
            }

            if (properties.IsRightButtonPressed)
            {
                flags |= CefEventFlags.RightMouseButton;
            }

            return flags;
        }

        /// <summary>
        /// Convert keyboard modifiers into cef flags.
        /// </summary>
        /// <param name="keyboardModifiers"></param>
        /// <returns></returns>
        public static CefEventFlags AsCefKeyboardModifiers(this KeyModifiers keyboardModifiers)
        {
            var modifiers = new CefEventFlags();

            if (keyboardModifiers.HasFlag(KeyModifiers.Alt))
            {
                modifiers |= CefEventFlags.AltDown;
            }

            if (keyboardModifiers.HasFlag(KeyModifiers.Control))
            {
                modifiers |= CefEventFlags.ControlDown;
            }

            if (keyboardModifiers.HasFlag(KeyModifiers.Shift))
            {
                modifiers |= CefEventFlags.ShiftDown;
            }

            return modifiers;
        }

        /// <summary>
        /// Convert a drag event args into a cef mouse event.
        /// </summary>
        /// <param name="eventArgs">The drag event args</param>
        /// <param name="mouseCoordinatesReferencial">The element used as the positioning referential</param>
        /// <returns></returns>
        public static CefMouseEvent AsCefMouseEvent(this DragEventArgs eventArgs, Visual mouseCoordinatesReferencial)
        {
            var cursorPos = eventArgs.GetPosition(mouseCoordinatesReferencial);

            return new CefMouseEvent((int)cursorPos.X, (int)cursorPos.Y, eventArgs.KeyModifiers.AsCefKeyboardModifiers());
        }

        /// <summary>
        /// Converts a drag drop effects to Cef Drag Operations
        /// </summary>
        /// <param name="dragDropEffects">The drag drop effects.</param>
        /// <returns></returns>
        public static CefDragOperationsMask AsCefDragOperationsMask(this DragDropEffects dragDropEffects)
        {
            var operationsCount = 0;
            var operations = CefDragOperationsMask.None;

            if (dragDropEffects.HasFlag(DragDropEffects.Copy))
            {
                operations |= CefDragOperationsMask.Copy;
                operationsCount++;
            }

            if (dragDropEffects.HasFlag(DragDropEffects.Move))
            {
                operations |= CefDragOperationsMask.Move;
                operationsCount++;
            }

            if (dragDropEffects.HasFlag(DragDropEffects.Link))
            {
                operations |= CefDragOperationsMask.Link;
                operationsCount++;
            }

            if (operationsCount == 3)
            {
                return CefDragOperationsMask.Every;
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
                return DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link;
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
            if (e.Data.Contains(DataFormats.FileNames))
            {
                var files = (string[])e.Data.GetFileNames();
                foreach (var filePath in files)
                {
                    var displayName = Path.GetFileName(filePath);
                    dragData.AddFile(filePath.Replace("\\", "/"), displayName);
                }
            }

            // Text
            if (e.Data.Contains(DataFormats.Text))
            {
                dragData.SetFragmentText(e.Data.GetText());
            }

            return dragData;
        }
    }
}
