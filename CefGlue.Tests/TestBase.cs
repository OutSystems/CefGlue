using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CefGlue.Tests.CustomSchemes;
using CefGlue.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xilium.CefGlue.Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace CefGlue.Tests
{
    public class TestBase
    {
        private static object initLock = new object();
        private static bool initialized = false;

        private AvaloniaCefBrowser browser;
        private Window window;

        protected AvaloniaCefBrowser Browser => browser;

        [OneTimeSetUp]
        protected async Task SetUp()
        {
            if (initialized)
            {
                return;
            }

            var initializationTaskCompletionSource = new TaskCompletionSource<bool>();

            CefRuntimeLoader.Initialize(customSchemes: new[] { 
                new CustomScheme()
                {
                    SchemeName = CustomSchemeHandlerFactory.SchemeName,
                    SchemeHandlerFactory = new CustomSchemeHandlerFactory()
                }
            });

            lock (initLock)
            {
                if (initialized)
                {
                    return;
                }

                var uiThread = new Thread(() =>
                {
                    AppBuilder.Configure<App>().UsePlatformDetect().SetupWithoutStarting();

                    Dispatcher.UIThread.Post(() =>
                    {
                        initialized = true;
                        initializationTaskCompletionSource.SetResult(true);
                    });
                    Dispatcher.UIThread.MainLoop(CancellationToken.None);
                });
                uiThread.IsBackground = true;
                uiThread.Start();
            }

            await initializationTaskCompletionSource.Task;
        }

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
