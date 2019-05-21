using System;
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

        private async void OnEvaluateJavascriptMenuItemClick(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            object result = await browser.EvaluateJavaScript<string>("\"hi\"", "about:blank");
            Console.WriteLine(result);

            result = await browser.EvaluateJavaScript<int>("1+1", "about:blank");
            Console.WriteLine(result);

            result = await browser.EvaluateJavaScript<bool>("false", "about:blank");
            Console.WriteLine(result);

            result = await browser.EvaluateJavaScript<double>("1.5+1.5", "about:blank");
            Console.WriteLine(result);
        }

        private void OnOpenDevToolsMenuItemClick(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            browser.ShowDeveloperTools();
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
