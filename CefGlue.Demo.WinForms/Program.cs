namespace Xilium.CefGlue.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal static class Program
    {
        // TODO: Chrome child processes want different COM apartment modes,
        // so it require native wrapper (to host dotnet runtime in single
        // binary) or use separate dotnet child processes (STA/MTA) and spawn
        // them depending on process type.
        // This especially important to run CEF debug builds, but release builds
        // must cover this case as well.
        [STAThread]
        private static int Main(string[] args)
        {
            using (var application = new DemoAppImpl())
            {
                return application.Run(args);
            }
        }
    }
}
