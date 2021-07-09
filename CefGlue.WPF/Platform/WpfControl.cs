using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Threading;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Platform;

namespace Xilium.CefGlue.WPF.Platform
{
    /// <summary>
    /// The WPF control wrapper.
    /// </summary>
    internal class WpfControl : IControl
    {
        protected readonly FrameworkElement _control;
        private ExtendedWpfNativeControlHost _nativeControl;
        
        public event Action GotFocus;
        public event Action<CefSize> SizeChanged;

        public WpfControl(FrameworkElement control)
        {
            _control = control;

            control.GotKeyboardFocus += OnGotFocus;
            control.LayoutUpdated += OnLayoutUpdated;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            GotFocus?.Invoke();
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (_control.IsLoaded || (_control.RenderSize.Width > 0 && _control.RenderSize.Height > 0))
            {
                // fire as soon as the control becomes loaded or the render size is not empty
                SizeChanged?.Invoke(new CefSize((int)_control.RenderSize.Width, (int)_control.RenderSize.Height));
            }
        }

        public IntPtr? GetHostViewHandle(int initialWidth, int initialHeight)
        {
            var window = Window.GetWindow(_control);
            if (window != null)
            {
                return new WindowInteropHelper(window).Handle;
            }

            return IntPtr.Zero;
        }

        public virtual void SetTooltip(string text)
        {
        }

        public void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
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
                })
            );
        }

        public void CloseContextMenu()
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() =>
                {
                    _control.ContextMenu = null;
                })
            );
        }

        public virtual bool SetCursor(IntPtr cursorHandle, CefCursorType cursorType)
        {
            return false;
        }

        public void InitializeRender(IntPtr browserHandle)
        {
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Input,
                new Action(() =>
                {
                    _nativeControl = new ExtendedWpfNativeControlHost(browserHandle);
                    ((ContentControl) _control).Content = _nativeControl;
                })
            );
        }

        public void DestroyRender()
        {
            _nativeControl.DestroyWindow(); // must be destroyed on current thread
            _control.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    // dispose remaining resources
                    _nativeControl.Dispose();
                })
            );
        }
    }
}
