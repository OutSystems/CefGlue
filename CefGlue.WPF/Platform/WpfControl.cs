using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32.SafeHandles;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;
using Point = Xilium.CefGlue.Common.Platform.Point;
using WpfPoint = System.Windows.Point;

namespace Xilium.CefGlue.WPF.Platform
{
    /// <summary>
    /// The WPF control wrapper.
    /// </summary>
    internal class WpfControl : UIControl, IDisposable
    {
        private readonly FrameworkElement _control;

        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        public WpfControl(FrameworkElement control, BuiltInRenderHandler renderHandler) : base(renderHandler)
        {
            _control = control;

            _control.GotFocus += delegate { TriggerGotFocus(); };
            _control.LostFocus += delegate { TriggerLostFocus(); };

            _control.MouseMove += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            _control.MouseLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));
            _control.MouseDown += (sender, arg) => TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType(), arg.ClickCount);
            _control.MouseUp += (sender, arg) => TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType());
            _control.MouseWheel += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), 0, (int)arg.Delta);

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

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += OnTooltipClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);
        }

        protected virtual IInputElement MousePositionReferential => _control;

        public override bool IsFocused => _control.Dispatcher.Invoke(() => _control.IsFocused);

        public override void Focus()
        {
            _control.Focus();
        }

        public override IntPtr? GetHostWindowHandle()
        {
            var window = Window.GetWindow(_control);
            if (window != null)
            {
                return new WindowInteropHelper(window).Handle;
            }

            return null;
        }

        public override Point PointToScreen(Point point)
        {
            return _control.Dispatcher.Invoke(() =>
            {
                var result = new Point(0, 0);
                if (PresentationSource.FromVisual(_control) != null)
                {
                    var screenCoordinates = _control.PointToScreen(new WpfPoint(point.X, point.Y));
                    result.X = (int)screenCoordinates.X;
                    result.Y = (int)screenCoordinates.Y;
                }
                return result;
            });
        }

        public override void SetCursor(IntPtr cursorHandle)
        {
            _control.Dispatcher.Invoke(() =>
            {
                var cursor = CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false));
                _control.Cursor = cursor;
            });
        }

        public override void SetTooltip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                UpdateTooltip(null);
            }
            else
            {
                _tooltipTimer.Tick += (sender, args) => UpdateTooltip(text);
                _tooltipTimer.Start();
            }
        }

        private void UpdateTooltip(string text)
        {
            _control.Dispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(
                    () =>
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            _tooltip.IsOpen = false;
                        }
                        else
                        {
                            _tooltip.Placement = PlacementMode.Mouse;
                            _tooltip.Content = text;
                            _tooltip.IsOpen = true;
                            _tooltip.Visibility = Visibility.Visible;
                        }
                    }));

            _tooltipTimer.Stop();
        }

        private void OnTooltipClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Placement = PlacementMode.Absolute;
        }

        public void Dispose()
        {
            _tooltipTimer.Stop();
        }
    }
}
