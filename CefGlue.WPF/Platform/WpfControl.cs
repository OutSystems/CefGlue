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
    internal class WpfControl : UIControl
    {
        private readonly FrameworkElement _control;

        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        private Point browserScreenLocation;

        public WpfControl(FrameworkElement control, BuiltInRenderHandler renderHandler) : base(renderHandler)
        {
            _control = control;

            _control.GotFocus += delegate { TriggerGotFocus(); };
            _control.LostFocus += delegate { TriggerLostFocus(); };

            _control.MouseMove += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            _control.MouseLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));
            _control.MouseDown += (sender, arg) =>
            {
                Mouse.Capture(_control); // allow captuing mouse mouse when outside the webview (eg: grabbing scrollbar)
                TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType(), arg.ClickCount);
            };
            _control.MouseUp += (sender, arg) =>
            {
                Mouse.Capture(null);
                TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType());
            };
            _control.MouseWheel += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), 0, (int)arg.Delta);

            _control.DragEnter += (sender, arg) => TriggerDragEnter(arg.AsCefMouseEvent(MousePositionReferential), arg.GetDragData(), arg.AllowedEffects.AsCefDragOperationsMask());
            _control.DragOver += (sender, arg) => TriggerDragOver(arg.AsCefMouseEvent(MousePositionReferential), arg.AllowedEffects.AsCefDragOperationsMask());
            _control.DragLeave += (sender, arg) => TriggerDragLeave();
            _control.Drop += (sender, arg) => TriggerDrop(arg.AsCefMouseEvent(MousePositionReferential), arg.AllowedEffects.AsCefDragOperationsMask());

            _control.Loaded += OnLoaded;
            _control.Unloaded += OnUnloaded;

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

            _control.IsVisibleChanged += delegate { TriggerVisibilityChanged(_control.IsVisible); };

            _tooltip = new ToolTip();
            _tooltip.StaysOpen = true;
            _tooltip.Visibility = Visibility.Collapsed;
            _tooltip.Closed += OnTooltipClosed;

            _tooltipTimer = new DispatcherTimer();
            _tooltipTimer.Interval = TimeSpan.FromSeconds(0.5);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            PresentationSource.AddSourceChangedHandler(_control, OnPresentationSourceChanged);
            var source = PresentationSource.FromVisual(_control);
            UpdatePresentationSource(source, source); // same parameters passed on purpose, to make sure the events don't get registered more than once to the same source
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            PresentationSource.RemoveSourceChangedHandler(_control, OnPresentationSourceChanged);
            _tooltip.IsOpen = false;
            _tooltipTimer.Stop();
        }

        private void OnPresentationSourceChanged(object sender, SourceChangedEventArgs e)
        {
            UpdatePresentationSource(e.OldSource, e.NewSource);
        }

        private void UpdatePresentationSource(PresentationSource oldSource, PresentationSource newSource)
        {
            if (oldSource?.RootVisual is Window oldWindow)
            {
                oldWindow.StateChanged -= OnHostWindowStateChanged;
                oldWindow.LocationChanged -= OnHostWindowLocationChanged;
            }

            if (newSource != null)
            {
                browserScreenLocation = GetBrowserScreenLocation();

                var matrix = newSource.CompositionTarget.TransformToDevice;
                TriggerScreenInfoChanged((float)matrix.M11);

                if (newSource.RootVisual is Window window)
                {
                    window.StateChanged += OnHostWindowStateChanged;
                    window.LocationChanged += OnHostWindowLocationChanged;
                }
            }
        }

        protected virtual IInputElement MousePositionReferential => _control;

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
            // calculate the point based on the browser stored location, 
            // since PointToScreen needs to be executed on the dispatcher thread
            // but calling Invoke at this stage can lead to dead locks
            var deviceScaleFactor = RenderHandler.DeviceScaleFactor;
            return new Point((int) (browserScreenLocation.X + point.X * deviceScaleFactor), (int) (browserScreenLocation.Y + point.Y * deviceScaleFactor));
        }

        public override void SetCursor(IntPtr cursorHandle)
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    var cursor = CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false));
                    _control.Cursor = cursor;
                }));
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
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
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

        private void OnHostWindowLocationChanged(object sender, EventArgs e)
        {
            browserScreenLocation = GetBrowserScreenLocation();
        }

        private void OnHostWindowStateChanged(object sender, EventArgs e)
        {
            var window = (Window)sender;

            switch (window.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Maximized:
                    TriggerVisibilityChanged(_control.IsVisible);
                    break;

                case WindowState.Minimized:
                    TriggerVisibilityChanged(false);
                    break;
            }
        }

        private Point GetBrowserScreenLocation()
        {
            if (PresentationSource.FromVisual(_control) != null)
            {
                var wpfPoint = _control.PointToScreen(new WpfPoint());
                return new Point((int) wpfPoint.X, (int) wpfPoint.Y);
            }
            return new Point(0, 0);
        }
    }
}
