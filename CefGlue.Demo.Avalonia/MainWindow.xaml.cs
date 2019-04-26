using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Xilium.CefGlue.Avalonia;

namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class MainWindow : Window
    {
        private AvaloniaCefBrowser browser;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var grid = new DockPanel();

            browser = new AvaloniaCefBrowser();
            browser.StartUrl = "http://www.google.com";

            var button = new Button();
            button.Content = "Open Dev Tools";
            //button.Click += Button_Click;
            DockPanel.SetDock(button, Dock.Bottom);

            grid.Children.Add(button);
            grid.Children.Add(browser);

            Content = grid;
        }
    }
}
