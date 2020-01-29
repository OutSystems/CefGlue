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
                      /*.With(new AvaloniaNativePlatformOptions() {
                          UseDeferredRendering = false,
                          UseGpu = false // fixes rendering quircks on mac OS
                      })*/
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() { WindowlessRenderingEnabled = false }))
                      .StartWithClassicDesktopLifetime(args);
                      
            return 0;
        }
    }
}
