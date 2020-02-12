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
        private readonly Control _contextMenuDummyTarget;

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
            var platformHandle = GetPlatformHandle();
            if (platformHandle is IMacOSTopLevelPlatformHandle macOSHandle)
            {
                return macOSHandle.GetNSViewRetained();
            }
            return platformHandle?.Handle;
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
            Dispatcher.UIThread.Post(() =>
            {
                SetContent(new ExtendedAvaloniaNativeControlHost(browserHandle));
            });
        }

        protected void SetContent(Control content)
        {
            ((Panel)_control.Content).Children.Add(content); 
        }

        public virtual void SetTooltip(string text) { }
    }
}
