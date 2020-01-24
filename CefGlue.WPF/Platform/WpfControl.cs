using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Xilium.CefGlue.Common;
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

        public WpfControl(FrameworkElement control)
        {
            _control = control;

            _control.IsVisibleChanged += delegate { TriggerVisibilityChanged(_control.IsVisible); };
        }

        private void AttachInputEvents(FrameworkElement control)
        {
            control.AllowDrop = true;
            control.GotFocus += delegate { TriggerGotFocus(); };
            control.LostFocus += delegate { TriggerLostFocus(); };

            control.MouseMove += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            control.MouseLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));
            control.MouseDown += (sender, arg) =>
            {
                TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType(), arg.ClickCount);
                if (arg.ChangedButton == MouseButton.Left)
                {
                    Mouse.Capture(control); // allow capturing mouse mouse when outside the webview (eg: grabbing scrollbar)
                }
            };
            control.MouseUp += (sender, arg) =>
            {
                TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), arg.ChangedButton.AsCefMouseButtonType());
                if (arg.ChangedButton == MouseButton.Left)
                {
                    Mouse.Capture(null);
                }
            };
            control.MouseWheel += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), 0, (int)arg.Delta);

            control.DragEnter += (sender, arg) => TriggerDragEnter(arg.AsCefMouseEvent(MousePositionReferential), arg.GetDragData(), arg.AllowedEffects.AsCefDragOperationsMask());
            control.DragOver += (sender, arg) => TriggerDragOver(arg.AsCefMouseEvent(MousePositionReferential), arg.AllowedEffects.AsCefDragOperationsMask());
            control.DragLeave += (sender, arg) => TriggerDragLeave();
            control.Drop += (sender, arg) => TriggerDrop(arg.AsCefMouseEvent(MousePositionReferential), arg.AllowedEffects.AsCefDragOperationsMask());

            control.Loaded += OnLoaded;
            control.Unloaded += OnUnloaded;

            control.KeyDown += (sender, arg) =>
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
            control.KeyUp += (sender, arg) =>
            {
                bool handled;
                TriggerKeyUp(arg.AsCefKeyEvent(true), out handled);
                arg.Handled = handled;
            };

            control.TextInput += (sender, arg) =>
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            PresentationSource.AddSourceChangedHandler(element, OnPresentationSourceChanged);
            var source = PresentationSource.FromVisual(element);
            UpdatePresentationSource(source, source); // same parameters passed on purpose, to make sure the events don't get registered more than once to the same source
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            PresentationSource.RemoveSourceChangedHandler(element, OnPresentationSourceChanged);
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

        public override Point PointToScreen(Point point, float deviceScaleFactor)
        {
            // calculate the point based on the browser stored location, 
            // since PointToScreen needs to be executed on the dispatcher thread
            // but calling Invoke at this stage can lead to dead locks
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

        public override void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() =>
                {
                    var menu = new ContextMenu();

                    foreach (var menuEntry in menuEntries)
                    {
                        if (menuEntry.IsSeparator)
                        {
                            menu.Items.Add(new Separator());
                        }
                        else
                        {
                            var menuItem = new MenuItem()
                            {
                                Header = menuEntry.Label.Replace("&", "_"),
                                IsEnabled = menuEntry.IsEnabled,
                                IsChecked = menuEntry.IsChecked ?? false,
                                IsCheckable = menuEntry.IsChecked != null,
                            };
                            var commandId = menuEntry.CommandId;
                            menuItem.Click += delegate { callback.Continue(commandId, CefEventFlags.None); };
                            menu.Items.Add(menuItem);
                        }
                    }

                    menu.Closed += delegate {
                        callback.Cancel();
                        _control.ContextMenu = null;
                    };

                    _control.ContextMenu = menu;
                    menu.HorizontalOffset = x;
                    menu.VerticalOffset = y;
                    menu.Placement = PlacementMode.Relative;
                    menu.PlacementTarget = _control;
                    menu.IsOpen = true;
                }
            ));
        }

        public override void CloseContextMenu()
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() =>
                {
                    _control.ContextMenu = null;
                }));
        }

        public override async Task<CefDragOperationsMask> StartDragging(CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            var dataObject = new DataObject();

            dataObject.SetText(dragData.FragmentText, TextDataFormat.Text);
            dataObject.SetText(dragData.FragmentText, TextDataFormat.UnicodeText);
            dataObject.SetText(dragData.FragmentHtml, TextDataFormat.Html);

            var result = DragDropEffects.None;

            await _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    result = DragDrop.DoDragDrop(_control, dataObject, allowedOps.AsDragDropEffects());
                })
            );

            return result.AsCefDragOperationsMask();
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

        public override BuiltInRenderHandler CreateRenderHandler()
        {
            var image = CreateImage();
            AttachInputEvents(_control);
            SetContent(image);
            return new WpfRenderHandler(image);
        }

        protected virtual void SetContent(Image image)
        {
            ((ContentControl) _control).Content = image;
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            var image = new Image()
            {
                Stretch = Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);

            return image;
        }
    }
}
