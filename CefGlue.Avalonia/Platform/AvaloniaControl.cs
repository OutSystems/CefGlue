using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
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

        private IntPtr? _browserHandle;
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

            if (NeedsRootWindowStylesFix)
            {
                _control.AttachedToLogicalTree += OnAttachedToLogicalTree;
            }
        }

        protected virtual bool NeedsRootWindowStylesFix => CefRuntime.Platform == CefRuntimePlatform.Windows;

        private void OnAttachedToLogicalTree(object sender, LogicalTreeAttachmentEventArgs e)
        {
            if (e.Root is PopupRoot root)
            {
                // FIX avalonia popups dont apply the CLIPCHILDREN style, so we must force it
                var rootHandle = root.PlatformImpl.Handle.Handle;
                NativeExtensions.Windows.SetWindowLong(rootHandle, NativeExtensions.Windows.GWL.STYLE, NativeExtensions.Windows.WS.CLIPCHILDREN);
            }
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

        public virtual IntPtr? GetHostViewHandle()
        {
            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                if (_dummyHostView == null)
                {
                    // create a dummy nsview to host all browsers
                    var nsViewClass = NativeExtensions.OSX.objc_getClass("NSView");
                    var nsViewType = NativeExtensions.OSX.objc_msgSend(nsViewClass, NativeExtensions.OSX.sel_registerName("alloc"));
                    _dummyHostView = NativeExtensions.OSX.objc_msgSend(nsViewType, NativeExtensions.OSX.sel_registerName("init"));
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
                    menu.Open(_control, new Point(x, y));
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
            _browserHandle = browserHandle;

            if (CefRuntime.Platform == CefRuntimePlatform.MacOSX)
            {
                // must retain the browser handle, as long as the browser lives, otherwise seg faults might occur
                NativeExtensions.OSX.objc_retain(browserHandle);
            }

            Dispatcher.UIThread.Post(() => SetContent(new ExtendedAvaloniaNativeControlHost(browserHandle)));
        }

        public void DestroyRender()
        {
            if (_browserHandle == null)
            {
                return;
            }

            switch (CefRuntime.Platform) {
                case CefRuntimePlatform.Windows:
                    NativeExtensions.Windows.DestroyWindow(_browserHandle.Value);
                    break;

                case CefRuntimePlatform.MacOSX:
                    NativeExtensions.OSX.objc_release(_browserHandle.Value);
                    break;   
            }

            _browserHandle = null;
        }

        protected void SetContent(Control content)
        {
            _controlVisualChildren.Add(content);
            _control.InvalidateArrange();
        }

        public virtual void SetTooltip(string text) { }
    }
}
