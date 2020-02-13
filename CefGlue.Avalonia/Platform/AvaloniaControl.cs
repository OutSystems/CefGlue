using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Avalonia.Platform
{
    /// <summary>
    /// The Avalonia control wrapper.
    /// </summary>
    internal class AvaloniaControl : Common.Platform.IControl
    {
        private static IntPtr? _dummyHostView;

        private readonly Control _contextMenuDummyTarget;
        private IntPtr? _browserHandle;

        protected readonly ContentControl _control;

        public event Action<CefSize> SizeChanged;

        public AvaloniaControl(ContentControl control)
        {
            _control = control;

            _contextMenuDummyTarget = new Control();
            _contextMenuDummyTarget.Width = 1;
            _contextMenuDummyTarget.Height = 1;
            _contextMenuDummyTarget.HorizontalAlignment = HorizontalAlignment.Left;
            _contextMenuDummyTarget.VerticalAlignment = VerticalAlignment.Top;

            var panel = new Panel();
            panel.Children.Add(_contextMenuDummyTarget);
            _control.Content = panel;

            _control.LayoutUpdated += OnLayoutUpdated;
        }

        public void Dispose()
        {
            if (_browserHandle != null)
            {
                NativeExtensions.objc_release(_browserHandle.Value);
                _browserHandle = null;
            }
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            SizeChanged?.Invoke(new CefSize((int)_control.Bounds.Width, (int)_control.Bounds.Height));
        }

        protected IPlatformHandle GetPlatformHandle()
        {
            return (_control.GetVisualRoot() as Window)?.PlatformImpl.Handle;
        }

        public virtual IntPtr? GetHostViewHandle()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                if (_dummyHostView == null)
                {
                    // create a dummy nsview to host all browsers
                    var nsViewClass = NativeExtensions.objc_getClass("NSView");
                    var nsViewType = NativeExtensions.objc_msgSend(nsViewClass, NativeExtensions.sel_registerName("alloc"));
                    _dummyHostView = NativeExtensions.objc_msgSend(nsViewType, NativeExtensions.sel_registerName("init"));
                }
                return _dummyHostView.Value;
            }
            return GetPlatformHandle()?.Handle;
        }

        public void OpenContextMenu(IEnumerable<MenuEntry> menuEntries, int x, int y, CefRunContextMenuCallback callback)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    var menu = new ExtendedAvaloniaContextMenu();
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
                    menu.Open(_contextMenuDummyTarget, new Point(x, y));
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
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                // must retain the browser handle, as long as the browser lives, otherwise seg faults might occur
                NativeExtensions.objc_retain(browserHandle);
            }
            Dispatcher.UIThread.Post(() =>
            {
                var nativeHost = new ExtendedAvaloniaNativeControlHost(browserHandle);

                if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
                {
                    // workaround, otherwise on osx the browser starts with screen size
                    var width = _control.Bounds.Width;
                    nativeHost.Width = width + 1;
                    DispatcherTimer.RunOnce(() => nativeHost.Width = double.NaN, TimeSpan.FromMilliseconds(1));
                }

                SetContent(nativeHost);
            });
        }

        protected void SetContent(Control content)
        {
            ((Panel)_control.Content).Children.Add(content); 
        }

        public virtual void SetTooltip(string text) { }
    }
}
