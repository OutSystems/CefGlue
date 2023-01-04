using System.Collections.Generic;
using System.Runtime.InteropServices;
using Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Demo.Avalonia
{
    internal class SchemeHandlerFactory : CefSchemeHandlerFactory {

        protected override CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request) {
            return null;
        }
    }
    class Program
    {

        static int Main(string[] args)
        {
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .With(new Win32PlatformOptions
                      {
                          UseWindowsUIComposition = false
                      })
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() {
#if WINDOWLESS
                          WindowlessRenderingEnabled = true
#else
                          WindowlessRenderingEnabled = false
#endif
                      },
                    flags: new []{
                        // enable experimental feature flags
                        new KeyValuePair<string, string>("enable-experimental-web-platform-features", null)
                      },
                      customSchemes: new[] {
                          new CustomScheme()
                          {
                              SchemeName = "local", SchemeHandlerFactory = new SchemeHandlerFactory()
                          },
                        new CustomScheme()
                        {
                            SchemeName = "test",
                            SchemeHandlerFactory = new CustomSchemeHandler()
                        },
                      }))
                      .StartWithClassicDesktopLifetime(args);
                      
            return 0;
        }
    }
}
