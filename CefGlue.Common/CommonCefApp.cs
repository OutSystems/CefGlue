using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xilium.CefGlue.Common
{
    public class CommonCefApp : CefApp
    {
        private readonly CefBrowserProcessHandler _browserProcessHandler;
        private readonly CefRenderProcessHandler _renderProcessHandler = new CommonCefRenderProcessHandler();

        public CommonCefApp(CefBrowserProcessHandler browserProcessHandler = null)
        {
            _browserProcessHandler = browserProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (string.IsNullOrEmpty(processType))
            {
                commandLine.AppendSwitch("disable-gpu", "1");
                commandLine.AppendSwitch("disable-gpu-compositing", "1");
                commandLine.AppendSwitch("enable-begin-frame-scheduling", "1");
                commandLine.AppendSwitch("disable-smooth-scrolling", "1");
            }
        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return _browserProcessHandler;
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }

        public void Run(string[] args)
        {
            var mainArgs = new CefMainArgs(args);

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, this, IntPtr.Zero);
            if (exitCode != -1)
            {
                Environment.Exit(exitCode);
                return;
            }

            var cefSettings = new CefSettings
            {
                WindowlessRenderingEnabled = true,
            };

            switch (CefRuntime.Platform)
            {
                case CefRuntimePlatform.Windows:
                    var path = new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath;
                    path = Regex.Replace(path, ".dll$", ".exe");
                    cefSettings.BrowserSubprocessPath = path;
                    cefSettings.MultiThreadedMessageLoop = true;
                    break;

                case CefRuntimePlatform.MacOSX:
                    cefSettings.MultiThreadedMessageLoop = false;
                    cefSettings.ExternalMessagePump = true;
                    break;
            }

            CefRuntime.Initialize(mainArgs, cefSettings, this, IntPtr.Zero);

            // TODO call shutdown when on exit
        }
    }
}
