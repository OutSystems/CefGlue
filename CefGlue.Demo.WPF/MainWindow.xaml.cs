using System;
using System.Windows;
using System.Windows.Controls;

namespace Xilium.CefGlue.Demo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateNewTab();
        }

        private BrowserView ActiveBrowserView => (BrowserView) tabControl.SelectedContent;

        private void CreateNewTab()
        {
            var tab = new TabItem();
            tab.Header = "New Tab";

            var view = new BrowserView();
            view.TitleChanged += title =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                   {
                       tab.Header = title;
                       tab.ToolTip = title;
                   }));
            };

            tab.Content = view;

            tabControl.Items.Add(tab);
        }

        private void OnNewTabMenuItemClick(object sender, RoutedEventArgs e)
        {
            CreateNewTab();
        }

        private void OnCloseTabMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex >= 0)
            {
                var browser = ActiveBrowserView;
                tabControl.Items.RemoveAt(tabControl.SelectedIndex);
                browser.Dispose();
            }
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
