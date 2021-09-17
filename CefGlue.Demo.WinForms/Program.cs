namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal static class Program
    {
        // Chrome child processes want different COM apartment modes,
        // so here we doesn't specify it, and let chrome choose appropriate mode
        // to work. This especially important to run CEF debug builds.
        // [STAThread]
        private static int Main(string[] args)
        {
            using (var application = new DemoAppImpl())
            {
                return application.Run(args);
            }
        }
    }
}
