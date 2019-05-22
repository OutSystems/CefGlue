using System;
using System.Dynamic;
using System.Linq;
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
            Console.WriteLine(await browser.EvaluateJavaScript<string>("\"Hello World!\""));

            Console.WriteLine(await browser.EvaluateJavaScript<int>("1+1"));

            Console.WriteLine(await browser.EvaluateJavaScript<bool>("false"));

            Console.WriteLine(await browser.EvaluateJavaScript<double>("1.5+1.5"));

            Console.WriteLine(await browser.EvaluateJavaScript<double>("3+1.5"));

            Console.WriteLine(string.Join(", ", await browser.EvaluateJavaScript<object[]>("[1, 2, 3]")));

            Console.WriteLine(string.Join(", ", (await browser.EvaluateJavaScript<ExpandoObject>("(function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));
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
