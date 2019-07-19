using System;

namespace Xilium.CefGlue.BrowserProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            CefRuntime.Load();

            var exitCode = CefRuntime.ExecuteProcess(new CefMainArgs(args), new BrowserCefApp(), IntPtr.Zero);
            if (exitCode != -1)
            {
                Environment.Exit(exitCode);
            }
        }
    }
}
