using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public class MainWindow : Window
    {
        private static bool IsChild = false;
        private static bool IsChildrenVisible;
        private static IList<MainWindow> ChildWindows = new List<MainWindow>();

        public MainWindow()
        {
            InitializeComponent();

#if WINDOWLESS
            Title += " - OSR mode";
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            CreateNewTab();

            var mainMenu = this.FindControl<Menu>("mainMenu");
            mainMenu.AttachedToVisualTree += MenuAttached;

            if (!IsChild)
            {
                IsChild = true;
                for (var i = 0; i < 2; i++)
                {
                    var w = new MainWindow();
                    ChildWindows.Add(w);
                }
            }

            var btn = this.Find<Button>("toggleWindows");
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (IsChildrenVisible)
            {
                foreach (var w in ChildWindows)
                {
                    w.Hide();
                }
            }
            else
            {
                foreach (var w in ChildWindows)
                {
                    w.Show(this);
                }
            }
            IsChildrenVisible = !IsChildrenVisible;
        }

        private void MenuAttached(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (NativeMenu.GetIsNativeMenuExported(this) && sender is Menu mainMenu)
            {
                mainMenu.IsVisible = false;
            }
        }

        private BrowserView ActiveBrowserView => (BrowserView)this.FindControl<TabControl>("tabControl").SelectedContent;

        private void CreateNewTab()
        {
            var tabItems = ((IList)this.FindControl<TabControl>("tabControl").Items);

            var view = new BrowserView();
            var tab = new TabItem();

            var headerPanel = new DockPanel();
            tab.Header = headerPanel;

            var closeButton = new Button()
            {
                Content = "X",
                Padding = new Thickness(2),
                Margin = new Thickness(5, 0, 0, 0)
            };
            closeButton.Click += delegate
            {
                view.Dispose();
                tabItems.Remove(tab);
            };
            DockPanel.SetDock(closeButton, Dock.Right);

            var tabTitle = new TextBlock()
            {
                Text = "New Tab"
            };
            headerPanel.Children.Add(tabTitle);
            headerPanel.Children.Add(closeButton);

            view.TitleChanged += title =>
            {
                Dispatcher.UIThread.Post((Action)(() =>
                {
                    tabTitle.Text = title;
                    ToolTip.SetTip(tab, title);
                }));
            };

            tab.Content = view;

            tabItems.Add(tab);
        }

        private void OnNewTabNativeMenuItemClick(object sender, EventArgs e)
        {
            CreateNewTab();
        }

        private void OnEvaluateJavascriptNativeMenuItemClick(object sender, EventArgs e)
        {
            ActiveBrowserView.EvaluateJavascript();
        }

        private void OnBindJavascriptObjectNativeMenuItemClick(object sender, EventArgs e)
        {
            ActiveBrowserView.BindJavascriptObject();
        }

        private void OnOpenDevToolsNativeMenuItemClick(object sender, EventArgs e)
        {
            ActiveBrowserView.OpenDevTools();
        }

        private void OnNewTabMenuItemClick(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        private void OnEvaluateJavascriptMenuItemClick(object sender, RoutedEventArgs e)
        {
            ActiveBrowserView.EvaluateJavascript();
        }

        private void OnBindJavascriptObjectMenuItemClick(object sender, RoutedEventArgs e)
        {
            ActiveBrowserView.BindJavascriptObject();
        }

        private void OnOpenDevToolsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ActiveBrowserView.OpenDevTools();
        }
    }
}
