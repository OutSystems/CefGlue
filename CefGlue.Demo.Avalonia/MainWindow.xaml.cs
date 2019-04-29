using Avalonia.Controls;
using Avalonia.Input;
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

            var browserWrapper = this.FindControl<Decorator>("browserWrapper");

            browser = new AvaloniaCefBrowser();
            browser.StartUrl = "http://www.google.com";

            browserWrapper.Child = browser;
        }

        private void OnUrlTextBoxKeyDown(object sender, global::Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                browser.NavigateTo(((TextBox)sender).Text);
            }
        }
    }
}
