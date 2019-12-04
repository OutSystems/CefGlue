using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common;
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

        private Action<Image> _setContent;

        public AvaloniaControl(Control control, Action<Image> setContent)
        {
            _setContent = setContent;

            _isVisible = control.IsEffectivelyVisible;
            control.GetPropertyChangedObservable(Control.TransformedBoundsProperty).Subscribe(OnTransformedBoundsChanged);
            control.AttachedToVisualTree += OnAttachedToVisualTree;
            control.DetachedFromVisualTree += OnDetachedFromVisualTree;

            _control = control;
        }

        private void AttachInputEvents(Control control)
        {
            control.GotFocus += delegate { TriggerGotFocus(); };
            control.LostFocus += delegate { TriggerLostFocus(); };

            control.PointerMoved += (sender, arg) => TriggerMouseMoved(arg.AsCefMouseEvent(MousePositionReferential));
            control.PointerLeave += (sender, arg) => TriggerMouseLeave(arg.AsCefMouseEvent(MousePositionReferential));

            // TODO
            //control.DoubleTapped += (sender, arg) =>
            //{
            //    TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), arg.AsCefMouseButtonType(), 2);
            //    if (arg.MouseButton == MouseButton.Left)
            //    {
            //        arg.Device.Capture(control);
            //    }
            //};
            control.PointerPressed += (sender, arg) =>
            {
                var button = arg.AsCefMouseButtonType();
                TriggerMouseButtonPressed(this, arg.AsCefMouseEvent(MousePositionReferential), button, 1);
                if (button == CefMouseButtonType.Left)
                {
                    arg.Pointer.Capture(control);
                }
            };
            control.PointerReleased += (sender, arg) =>
            {
                var button = arg.AsCefMouseButtonType();
                TriggerMouseButtonReleased(arg.AsCefMouseEvent(MousePositionReferential), button);
                if (button == CefMouseButtonType.Left)
                {
                    arg.Pointer.Capture(null);
                }
            };
            control.PointerWheelChanged += (sender, arg) => TriggerMouseWheelChanged(arg.AsCefMouseEvent(MousePositionReferential), (int)arg.Delta.X * MouseWheelDelta, (int)arg.Delta.Y * MouseWheelDelta);

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
        }

        private void OnTransformedBoundsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            // the only way we can be notified of the control visibility changes is through the transformed bounds property changes
            //var isVisible = _control.IsEffectivelyVisible;
            //if (isVisible != _isVisible)
            //{
            //    _isVisible = isVisible;
            //    TriggerVisibilityChanged(isVisible);
            //}
        }

        private void OnDetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            _windowStateChangedObservable?.Dispose();
            _isVisible = false;
            TriggerVisibilityChanged(false);
        }

        private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            TriggerVisibilityChanged(true);
            _isVisible = true;
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

        public override Point PointToScreen(Point point, float deviceScaleFactor)
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

        public override void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    var menu = new ContextMenu();
                    var menuItems = new List<TemplatedControl>();
                    menu.Items = menuItems;

                    foreach (var menuEntry in menuEntries)
                    {
                        if (menuEntry.IsSeparator)
                        {
                            menuItems.Add(new Separator());
                        }
                        else
                        {
                            var menuItem = new MenuItem()
                            {
                                Header = menuEntry.Label.Replace("&", "_"),
                                IsEnabled = menuEntry.IsEnabled,
                                // TODO
                                //IsChecked = menuEntry.IsChecked ?? false,
                                //IsCheckable = menuEntry.IsChecked != null,
                            };
                            var commandId = menuEntry.CommandId;
                            menuItem.Click += delegate { callback.Continue(commandId, CefEventFlags.None); };
                            menuItems.Add(menuItem);
                        }
                    }

                    menu.MenuClosed += delegate
                    {
                        callback.Cancel();
                        _control.ContextMenu = null;
                    };

                    _control.ContextMenu = menu;
                },
                DispatcherPriority.Input);
        }

        public override void CloseContextMenu()
        {
            // TODO this is being raised when it shouldn't
            //Dispatcher.UIThread.Post(
            //   () =>
            //   {
            //       _control.ContextMenu = null;
            //   },
            //   DispatcherPriority.Input);
        }

        public override BuiltInRenderHandler CreateRenderHandler()
        {
            var image = CreateImage();
            AttachInputEvents(_control);
            _setContent(image);
            return new AvaloniaRenderHandler(image);
        }

        /// <summary>
        /// Create an image that is used to render the browser frame and popups
        /// </summary>
        /// <returns></returns>
        private static Image CreateImage()
        {
            return new Image()
            {
                Focusable = false,
                Stretch = Stretch.None,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Top,
            };
        }
    }
}
