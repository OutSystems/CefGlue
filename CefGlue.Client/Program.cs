namespace Xilium.CefGlue.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using Xilium.CefGlue;

    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                CefRuntime.Load();
            }
            catch (DllNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }
            catch (CefRuntimeException ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 3;
            }

            var mainArgs = new CefMainArgs(args);
            var app = new DemoApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, app);
            if (exitCode != -1)
                return exitCode;

            var settings = new CefSettings
                {
                    // BrowserSubprocessPath = @"D:\fddima\Projects\Xilium\Xilium.CefGlue\CefGlue.Demo\bin\Release\Xilium.CefGlue.Demo.exe",
                    SingleProcess = false,
                    MultiThreadedMessageLoop = true,
                    LogSeverity = CefLogSeverity.Disable,
                    LogFile = "CefGlue.log",
                };

            CefRuntime.Initialize(mainArgs, settings, app);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!settings.MultiThreadedMessageLoop)
            {
                Application.Idle += (sender, e) => { CefRuntime.DoMessageLoopWork(); };
            }

            Application.Run(new MainForm());

            CefRuntime.Shutdown();
            return 0;
        }
    }
}
