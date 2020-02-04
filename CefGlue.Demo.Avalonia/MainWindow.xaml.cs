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

        private BrowserView ActiveBrowserView => (BrowserView) this.FindControl<TabControl>("tabControl").SelectedContent;

        private void CreateNewTab()
        {
            var tabItems = ((IList)this.FindControl<TabControl>("tabControl").Items);

            var tab = new TabItem();
            tab.Header = "New Tab";

            var view = new BrowserView();
            view.TitleChanged += title => Dispatcher.UIThread.Post(() =>
            {
                tab.Header = title;
                ToolTip.SetTip(tab, title);
            });

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
