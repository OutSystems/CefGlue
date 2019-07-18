using System;
using Xilium.CefGlue.WPF;

namespace Xilium.CefGlue.Demo.WPF
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            CefGlueLoader.Initialize(args);

            var app = new Xilium.CefGlue.Demo.WPF.App();
            app.InitializeComponent();
            app.Run();

            return 0;
        }
    }
}
