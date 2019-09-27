using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;
using AvaloniaPoint = Avalonia.Point;
using Point = Xilium.CefGlue.Common.Platform.Point;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// The Avalonia control wrapper.
    /// </summary>
    internal class AvaloniaControl : UIControl
    {
        // TODO avalonia: get value from OS
        private const int MouseWheelDelta = 100;

        private readonly Control _control;

        private IDisposable _windowStateChangedObservable;
        private bool _isVisible;

        public AvaloniaControl(Control control, BuiltInRenderHandler renderHandler) : base(renderHandler)
        {
            _control = control;

            _control.GotFocus += delegate { TriggerGotFocus(); };
            _control.LostFocus += delegate { TriggerLostFocus(); };

            _control.PointerMoved += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            _control.PointerLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));
            _control.PointerPressed += (sender, arg) =>
            {
                arg.Device.Capture(_control);
                TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.MouseButton.AsCefMouseButtonType(), arg.ClickCount);
            };
            _control.PointerReleased += (sender, arg) =>
            {
                arg.Device.Capture(null);
                TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), arg.MouseButton.AsCefMouseButtonType());
            };
            _control.PointerWheelChanged += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), (int)arg.Delta.X * MouseWheelDelta, (int)arg.Delta.Y * MouseWheelDelta);

            _control.KeyDown += (sender, arg) =>
            {
                bool handled;
                TriggerKeyDown(arg.AsCefKeyEvent(false), out handled);

                var key = arg.Key;
                if (key == Key.Tab  // Avoid tabbing out the web browser control
                    || key == Key.Home || key == Key.End // Prevent keyboard navigation using home and end keys
                    || key == Key.Up || key == Key.Down || key == Key.Left || key == Key.Right // Prevent keyboard navigation using arrows
                )
                {
                    handled = true;
                }

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

            _isVisible = _control.IsEffectivelyVisible;
            _control.GetPropertyChangedObservable(Control.TransformedBoundsProperty).Subscribe(OnTransformedBoundsChanged);
            _control.AttachedToVisualTree += OnAttachedToVisualTree;
            _control.DetachedFromVisualTree += OnDetachedFromVisualTree;
        }

        private void OnTransformedBoundsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // the only way we can be notified of the control visibility changes is through the transformed bounds property changes
            var isVisible = _control.IsEffectivelyVisible;
            if (isVisible != _isVisible)
            {
                _isVisible = isVisible;
                TriggerVisibilityChanged(isVisible);
            }
        }

        private void OnDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _windowStateChangedObservable?.Dispose();
        }

        private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (e.Root is Window newWindow)
            {
                _windowStateChangedObservable = newWindow.GetPropertyChangedObservable(Window.WindowStateProperty).Subscribe(OnHostWindowStateChanged);
            }
        }

        private void OnHostWindowStateChanged(AvaloniaPropertyChangedEventArgs e)
        {
            switch ((WindowState)e.NewValue)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    TriggerVisibilityChanged(_control.IsEffectivelyVisible);
                    break;

                case WindowState.Minimized:
                    TriggerVisibilityChanged(false);
                    break;
            }
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
    }
}
