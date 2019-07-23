using System;
using System.Linq;
using Xilium.CefGlue.Common;

namespace Xilium.CefGlue.BrowserProcess
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            try
            {
#endif
                CefRuntime.Load();

                var customSchemesArg = GetArgumentValue(args, CommandLineArgs.CustomScheme);
                var customSchemes = CustomScheme.FromCommandLineValue(customSchemesArg);

                var exitCode = CefRuntime.ExecuteProcess(new CefMainArgs(args), new BrowserCefApp(customSchemes), IntPtr.Zero);
                if (exitCode != -1)
                {
                    Environment.Exit(exitCode);
                }
#if DEBUG
            }
            catch (Exception)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif
        }

        private static string GetArgumentValue(string[] args, string argName)
        {
            var arg = args.FirstOrDefault(a => a.StartsWith(argName));
            return arg?.Substring(argName.Length) ?? "";
        }
    }
}
