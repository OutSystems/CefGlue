using Avalonia.Controls;
using Avalonia.Threading;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;

namespace CefGlue.Tests
{
    public class TestBase
    {
        private AvaloniaCefBrowser browser;
        private Window window;

        protected AvaloniaCefBrowser Browser => browser;

        [SetUp]
        protected virtual async Task Setup()
        {
            await InternalSetup(() => new AvaloniaCefBrowser());

            await ExtraSetup();
        }

        protected async Task InternalSetup(Func<AvaloniaCefBrowser> avaloniaCefBrowserFactory)
        {
            var testName = TestContext.CurrentContext.Test.FullName; // capture test name outside the async part (otherwise wont work properly)
            await Run(async () =>
            {
                if (window == null)
                {
                    window = new Window();
                    window.Width = 1;
                    window.Height = 1;

                    window.Show();
                }

                window.Title = testName;

                var browserInitTaskCompletionSource = new TaskCompletionSource<bool>();
                browser = avaloniaCefBrowserFactory();
                browser.BrowserInitialized += delegate () { browserInitTaskCompletionSource.SetResult(true); };

                window.Content = browser;

                await browserInitTaskCompletionSource.Task;
            });
        }

        protected virtual Task ExtraSetup()
        {
            return Task.CompletedTask;
        }

        [TearDown] 
        protected void TearDown()
        {
            browser?.Dispose();
        }

        [OneTimeTearDown]
        protected async Task OneTimeTearDown()
        {
            await Run(() => {
                window?.Close();
                window = null;
            });
        }

        protected Task Run(Func<Task> func) => Dispatcher.UIThread.InvokeAsync(func, DispatcherPriority.Background);

        protected Task Run(Action action) => Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background).GetTask();

        protected Task<T> EvaluateJavascript<T>(string script, TimeSpan? timeout = null) => Browser.EvaluateJavaScript<T>(script, timeout: timeout);
    }
}
