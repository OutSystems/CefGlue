using System;
using System.IO;
using Avalonia;
using Xilium.CefGlue.Common;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {

        static int Main(string[] args)
        {
            // generate a unique cache path to avoid problems when launching more than one process
            // https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=19665
            var cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));
            
            AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
            
            AppBuilder.Configure<App>()
                      .UsePlatformDetect()
                      .With(new Win32PlatformOptions())
                      .AfterSetup(_ => CefRuntimeLoader.Initialize(new CefSettings() {
                          RootCachePath = cachePath,
#if WINDOWLESS
                          WindowlessRenderingEnabled = true
#else
                          WindowlessRenderingEnabled = false
#endif
                      },
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

        private static void Cleanup(string cachePath)
        {
            CefRuntime.Shutdown(); // must shutdown cef to free cache files (so that cleanup is able to delete files)

            try {
                var dirInfo = new DirectoryInfo(cachePath);
                if (dirInfo.Exists) {
                    dirInfo.Delete(true);
                }
            } catch (UnauthorizedAccessException) {
                // ignore
            } catch (IOException) {
                // ignore
            }
        }
    }
}
