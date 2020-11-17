using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Demo.WPF
{
    public partial class BrowserView : IDisposable
    {
        public BrowserView()
        {
            InitializeComponent();
            //browser.RegisterJavascriptObject(new BindingTestClass(), "boundBeforeLoadObject");
        }

        public event Action<string> TitleChanged;

        private void OnBrowserTitleChanged(object sender, string title)
        {
            TitleChanged?.Invoke(title);
        }

        private void OnBrowserLoadStart(object sender, LoadStartEventArgs e)
        {
            if (!e.Frame.IsMain)
            {
                return;
            }
            Application.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
                addressTextBox.Text = e.Frame.Url;
            }));
        }

        private void OnAddressTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                browser.Address = ((TextBox)sender).Text;
            }
        }

        public async void EvaluateJavascript()
        {
            //Console.WriteLine(await browser.EvaluateJavaScript<string>("\"Hello World!\""));

            //Console.WriteLine(await browser.EvaluateJavaScript<int>("1+1"));

            //Console.WriteLine(await browser.EvaluateJavaScript<bool>("false"));

            //Console.WriteLine(await browser.EvaluateJavaScript<double>("1.5+1.5"));

            //Console.WriteLine(await browser.EvaluateJavaScript<double>("3+1.5"));

            //Console.WriteLine(await browser.EvaluateJavaScript<DateTime>("new Date()"));

            //Console.WriteLine(string.Join(", ", await browser.EvaluateJavaScript<object[]>("[1, 2, 3]")));

            //Console.WriteLine(string.Join(", ", (await browser.EvaluateJavaScript<ExpandoObject>("(function() { return { a: 'valueA', b: 1, c: true } })()")).Select(p => p.Key + ":" + p.Value)));
        }

        public void BindJavascriptObject()
        {
            //const string TestObject = "dotNetObject";

            //var obj = new BindingTestClass();
            //browser.RegisterJavascriptObject(obj, "dotNetObject");

            //var methods = obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
            //                           .Where(m => m.GetParameters().Length == 0)
            //                           .Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1));

            //var script = "(function () {" +
            //    "let calls = [];" +
            //    //string.Join("", methods.Select(m => $"calls.push({{ name: '{m}', promise: {TestObject}.{m}() }});")) +
            //    $"calls.push({{ name: 'getObjectWithParams', promise: {TestObject}.getObjectWithParams(5, 'a string', {{ Name: 'obj name', Value: 10 }}, [ 1, 2 ]) }});" +
            //    "calls.forEach(c => c.promise.then(r => console.log(c.name + ': ' + JSON.stringify(r))).catch(e => console.log(e)));" +
            //    "})()";

            //browser.ExecuteJavaScript(script);
        }

        public void OpenDevTools()
        {
            browser.ShowDeveloperTools();
        }

        public void Dispose()
        {
            browser.Dispose();
        }
    }
}