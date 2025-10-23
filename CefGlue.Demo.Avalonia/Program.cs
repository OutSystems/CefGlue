using System;
using System.IO;
using System.Threading.Tasks;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.Avalonia
{
    class Program
    {
        static int Main(string[] args)
        {
            // https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=19665
            var cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));
            CefRuntimeLoader.InitializeSync(new CefSettings { RootCachePath = cachePath });
            AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
            
            // To check if it's working
            //CefBrowserWindow.Create();
            
            // Hold for 20 seconds to register NetworkService crashes
            Task.Delay(20000).Wait();
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
