using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

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
                      },
                      flags: new[] { new KeyValuePair<string, string>("disable-features", "NetworkService,VizDisplayCompositor") },
                      customSchemes: new[] {
                        new CustomScheme()
                        {
                            SchemeName = "test",
                            SchemeHandlerFactory = new CustomSchemeHandler()
                        }
                      }))
                      .StartWithClassicDesktopLifetime(args);
                      
            return 0;
        }
    }
}
