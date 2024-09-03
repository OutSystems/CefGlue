using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;
using CefGlue.Tests.CustomSchemes;
using CefGlue.Tests.Helpers;
using NUnit.Framework;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace CefGlue.Tests;

[SetUpFixture]
public class GlobalSetup
{

    [OneTimeSetUp]
    protected async Task SetUp()
    {
        var initializationTaskCompletionSource = new TaskCompletionSource<bool>();

        CefRuntimeLoader.Initialize(customSchemes: new[]
        {
            new CustomScheme()
            {
                SchemeName = CustomSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new CustomSchemeHandlerFactory()
            }
        });

        var uiThread = new Thread(() =>
        {
            AppBuilder.Configure<App>().UsePlatformDetect().SetupWithoutStarting();

            Dispatcher.UIThread.Post(() =>
            {
                initializationTaskCompletionSource.SetResult(true);
            });
            Dispatcher.UIThread.MainLoop(CancellationToken.None);
        });
        uiThread.IsBackground = true;
        uiThread.Start();

        await initializationTaskCompletionSource.Task;
    }
}