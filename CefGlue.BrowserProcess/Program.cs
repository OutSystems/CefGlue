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
                CefRuntime.EnableHighDpiSupport();

                var customSchemesArg = GetArgumentValue(args, CommandLineArgs.CustomScheme);
                var customSchemes = CustomScheme.FromCommandLineValue(customSchemesArg);

                // first argument is the path of the executable, but its ignored for now
                var mainArgs = new CefMainArgs(new[] { "BrowserProcess" }.Concat(args).ToArray());
                var exitCode = CefRuntime.ExecuteProcess(mainArgs, new BrowserCefApp(customSchemes), IntPtr.Zero);
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
            var arg = args.FirstOrDefault(a => a?.StartsWith(argName) == true);
            return arg?.Substring(argName.Length) ?? "";
        }
    }
}
