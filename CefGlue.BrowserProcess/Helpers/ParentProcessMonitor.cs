using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.Helpers
{
    /// <summary>
    /// Prevents current process from staying alive after parent process dies.
    /// </summary>
    internal static class ParentProcessMonitor
    {
        public static void StartMonitoring(int parentProcessId)
        {
            Task.Factory.StartNew(() => AwaitParentProcessExit(parentProcessId), TaskCreationOptions.LongRunning);
        }

        private static async void AwaitParentProcessExit(int parentProcessId)
        {
            try
            {
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.WaitForExit();
            }
            catch
            {
                //main process probably died already
            }

            await Task.Delay(TimeSpan.FromSeconds(10)); // wait a bit before exiting

            Environment.Exit(0);
        }
    }
}
