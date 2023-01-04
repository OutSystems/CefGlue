using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.Avalonia
{
    public class MainWindow : Window
    {
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
        }

        private void MenuAttached(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (NativeMenu.GetIsNativeMenuExported(this) && sender is Menu mainMenu)
            {
                mainMenu.IsVisible = false;
            }
        }

        private BrowserView ActiveBrowserView =>
            (BrowserView)this.FindControl<TabControl>("tabControl").SelectedContent;

        private void CreateNewTab()
        {
            var tabItems = ((IList)this.FindControl<TabControl>("tabControl").Items);

            var view = new BrowserView();
            var tab = new TabItem();

            var headerPanel = new DockPanel();
            tab.Header = headerPanel;

            var closeButton = new Button()
            {
                Content = "X", Padding = new Thickness(2), Margin = new Thickness(5, 0, 0, 0)
            };
            closeButton.Click += delegate
            {
                view.Dispose();
                tabItems.Remove(tab);
            };
            DockPanel.SetDock(closeButton, Dock.Right);

            var tabTitle = new TextBlock() { Text = "New Tab" };
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

        private void OnTestScenarioNativeMenuItemClick(object sender, EventArgs e)
        {
            ActiveBrowserView.TestScenario();
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

        private void OnTestScenarioMenuItemClick(object sender, RoutedEventArgs e)
        {
            ActiveBrowserView.TestScenario();
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