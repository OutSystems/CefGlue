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

#if WINDOWLESS
            Title += " - OSR mode";
#endif
        }

        private BrowserView ActiveBrowserView => (BrowserView) tabControl.SelectedContent;

        private void CreateNewTab()
        {
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
                tabControl.Items.Remove(tab); 
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
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                   {
                       tabTitle.Text = title;
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
