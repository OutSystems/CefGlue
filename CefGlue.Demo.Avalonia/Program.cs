using System.Runtime.InteropServices;
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
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() {
#if WINDOWLESS
                          WindowlessRenderingEnabled = true
#else
                          WindowlessRenderingEnabled = false
#endif

                      }))
                      .StartWithClassicDesktopLifetime(args);
                      
            return 0;
        }
    }
}
