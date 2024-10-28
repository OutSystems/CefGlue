using System;
using System.IO;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.WPF
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            // generate a unique cache path to avoid problems when launching more than one process
            // https://www.magpcss.org/ceforum/viewtopic.php?f=6&t=19665
            var cachePath = Path.Combine(Path.GetTempPath(), "CefGlue_" + Guid.NewGuid().ToString().Replace("-", null));
            
            AppDomain.CurrentDomain.ProcessExit += delegate { Cleanup(cachePath); };
            
            var settings = new CefSettings()
            {
                RootCachePath = cachePath,
#if WINDOWLESS
                // its recommended to leave this off (false), since its less performant and can cause more issues
                WindowlessRenderingEnabled = true
#else
                WindowlessRenderingEnabled = false
#endif
            };
            CefRuntimeLoader.Initialize(settings);

            var app = new Xilium.CefGlue.Demo.WPF.App();
            app.InitializeComponent();
            app.Run();

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
