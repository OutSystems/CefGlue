using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;
using AvaloniaPoint = Avalonia.Point;
using Point = Xilium.CefGlue.Common.Platform.Point;

namespace Xilium.CefGlue.Avalonia.Platform
{
    internal class AvaloniaControl : UIControl
    {
        // TODO avalonia: get value from OS
        private const int MouseWheelDelta = 100;

        private readonly Control _control;

        public AvaloniaControl(Control control, RenderHandler renderHandler) : base(renderHandler)
        {
            _control = control;

            _control.GotFocus += delegate { TriggerGotFocus(); };
            _control.LostFocus += delegate { TriggerLostFocus(); };

            _control.PointerMoved += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            _control.PointerLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));
            _control.PointerPressed += (sender, arg) => TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.MouseButton.AsCefMouseButtonType(), arg.ClickCount);
            _control.PointerReleased += (sender, arg) => TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), arg.MouseButton.AsCefMouseButtonType());
            _control.PointerWheelChanged += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), (int)arg.Delta.X * MouseWheelDelta, (int)arg.Delta.Y * MouseWheelDelta);

            _control.KeyDown += (sender, arg) =>
            {
                bool handled;
                TriggerKeyDown(arg.AsCefKeyEvent(false), out handled);
                arg.Handled = handled;
            };
            _control.KeyUp += (sender, arg) =>
            {
                bool handled;
                TriggerKeyUp(arg.AsCefKeyEvent(true), out handled);
                arg.Handled = handled;
            };

            _control.TextInput += (sender, arg) =>
            {
                bool handled;
                TriggerTextInput(arg.Text, out handled);
                arg.Handled = handled;
            };
        }

        protected virtual IVisual MousePositionReferential => _control;

        public override Point PointToScreen(Point point)
        {
            var screenCoordinates = _control.PointToScreen(new AvaloniaPoint(point.X, point.Y));

            var result = new Point(0, 0);
            result.X = (int) screenCoordinates.X;
            result.Y = (int) screenCoordinates.Y;
            return result;
        }

        public override IntPtr? GetHostWindowHandle()
        {
            var parentWnd = _control.GetVisualRoot() as Window;
            if (parentWnd != null)
            {
                return (IntPtr?)parentWnd.PlatformImpl.Handle.Handle;
            }

            return null;
        }

        public override void SetCursor(IntPtr cursorHandle)
        {
            var cursor = CursorsProvider.GetCursorFromHandle(cursorHandle);
            Dispatcher.UIThread.Post(() => _control.Cursor = cursor);
        }

        public override void SetTooltip(string text)
        {
            // TODO BUG: sometimes the tooltips are left hanging when the user moves the cursor over the tooltip but meanwhile
            // the tooltip is destroyed
            Dispatcher.UIThread.Post(
                () =>
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        ToolTip.SetIsOpen(_control, false);
                    }
                    else
                    {
                        ToolTip.SetTip(_control, text);
                        ToolTip.SetShowDelay(_control, 0);
                        ToolTip.SetIsOpen(_control, true);
                    }
                }, DispatcherPriority.Input);
        }

        public override void Focus()
        {
            _control.Focus();
        }

        public override bool IsFocused => _control.IsFocused;
    }
}
