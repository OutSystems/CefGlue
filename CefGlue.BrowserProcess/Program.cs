using System;
using System.Linq;
using System.Runtime.InteropServices;
using Xilium.CefGlue.BrowserProcess.Helpers;
using Xilium.CefGlue.Common.Shared;

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
                NativeLibsLoader.Install();

                var parentProcessId = GetArgumentValue(args, CommandLineArgs.ParentProcessId);
                if (parentProcessId != null && int.TryParse(parentProcessId, out var parentProcessIdAsInt))
                {
                    ParentProcessMonitor.StartMonitoring(parentProcessIdAsInt);
                }

                CefRuntime.Load();

                var customSchemesArg = GetArgumentValue(args, CommandLineArgs.CustomScheme);
                var customSchemes = CustomScheme.FromCommandLineValue(customSchemesArg);
                // first argument is the path of the executable, but its ignored for now
                var mainArgs = new CefMainArgs(new[] { "BrowserProcess" }.Concat(args).ToArray());
                var exitCode = CefRuntime.ExecuteProcess(mainArgs, new RendererCefApp(customSchemes), IntPtr.Zero);
                if (exitCode != -1)
                {
                    Environment.Exit(exitCode);
                }
#if DEBUG
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    System.Diagnostics.Debugger.Launch();
                }
                throw;
            }
#endif
        }

        private static string GetArgumentValue(string[] args, string argName)
        {
            var arg = args.FirstOrDefault(a => a?.StartsWith(argName + "=") == true);
            return arg?.Substring(argName.Length + 1) ?? "";
        }
    }
}
