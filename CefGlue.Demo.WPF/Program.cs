using System;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.Demo.WPF
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            var settings = new CefSettings()
            {
#if WINDOWLESS
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
    }
}
