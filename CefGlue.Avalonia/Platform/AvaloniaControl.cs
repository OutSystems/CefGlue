using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Avalonia.Platform.MacOS;
using Xilium.CefGlue.Avalonia.Platform.Windows;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// The Avalonia control wrapper.
    /// </summary>
    internal class AvaloniaControl : Common.Platform.IControl
    {
        private IHost _hostView;
        private Func<WindowBase> _getHostingWindow;
        private readonly IAvaloniaList<IVisual> _controlVisualChildren;

        protected readonly Control _control;

        public event Action GotFocus;
        public event Action<CefSize> SizeChanged;

        public AvaloniaControl(Control control, IAvaloniaList<IVisual> visualChildren, Func<WindowBase> getHostingWindow)
        {
            _control = control;
            _controlVisualChildren = visualChildren;
            _getHostingWindow = getHostingWindow;

            _control.GotFocus += OnGotFocus;
            _control.LayoutUpdated += OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            SizeChanged?.Invoke(new CefSize((int)_control.Bounds.Width, (int)_control.Bounds.Height));
        }

        private void OnGotFocus(object sender, GotFocusEventArgs e)
        {
            GotFocus?.Invoke();
        }

        protected IPlatformHandle GetPlatformHandle()
        {
            return _getHostingWindow()?.PlatformImpl.Handle;
        }

        public virtual IntPtr? GetHostViewHandle(int initialWidth, int initialHeight)
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                if (_hostView == null)
                {
                    // create an host NSView to embed cef with current control dimensions
                    _hostView = new NSView(initialWidth, initialHeight);
                }
                return _hostView.Handle;
            }

            // return the window handle
            return GetPlatformHandle()?.Handle;
        }

        public void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    var menu = new ContextMenu();
                    var menuItems = new List<TemplatedControl>();

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

                    menu.Items = menuItems;

                    _control.ContextMenu = menu;

                    menu.PlacementAnchor = PopupAnchor.TopLeft;
                    menu.PlacementGravity = PopupGravity.BottomRight;
                    menu.PlacementMode = PlacementMode.AnchorAndGravity;
                    menu.PlacementRect = new Rect(x, y, 1, 1);
                    menu.Open(_control);
                },
                DispatcherPriority.Input);
        }

        public void CloseContextMenu()
        {
            Dispatcher.UIThread.Post(
               () =>
               {
                   _control.ContextMenu?.Close();
                   _control.ContextMenu = null;
               },
               DispatcherPriority.Input);
        }

        public void InitializeRender(IntPtr browserHandle)
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Windows)
            {
                // store cef window handle, to dispose later
                _hostView = new HostWindow(browserHandle);
            }

            Dispatcher.UIThread.Post(() => SetContent(new ExtendedAvaloniaNativeControlHost(_hostView.Handle)));
        }

        public void DestroyRender()
        {
            _hostView?.Dispose();
            _hostView = null;
        }

        protected void SetContent(Control content)
        {
            _controlVisualChildren.Add(content);
            _control.InvalidateArrange();
        }

        public virtual void SetTooltip(string text) { }
    }
}
