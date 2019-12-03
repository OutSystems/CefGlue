using Avalonia;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {
        static int Main(string[] args)
        {
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .UseSkia()
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() { WindowlessRenderingEnabled = true }))
                      .Start<MainWindow>();
            return 0;
        }
    }
}
