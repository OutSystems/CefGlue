using System.Diagnostics;
using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonBrowserProcessHandler : CefBrowserProcessHandler
    {
        private readonly CustomScheme[] _customSchemes;
        private readonly BrowserProcessHandler _handler;
        private readonly string _currentProcessId;

        public CommonBrowserProcessHandler(BrowserProcessHandler handler, CustomScheme[] customSchemes)
        {
            _customSchemes = customSchemes;
            _handler = handler;
            _currentProcessId = Process.GetCurrentProcess().Id.ToString();
        }

        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            _handler?.HandleBeforeChildProcessLaunch(commandLine);
            if (_customSchemes?.Length > 0)
            {
                commandLine.AppendSwitch(CommandLineArgs.CustomScheme, CustomScheme.ToCommandLineValue(_customSchemes));
            }

            commandLine.AppendSwitch(CommandLineArgs.ParentProcessId, _currentProcessId);
        }

        protected override void OnContextInitialized()
        {
            _handler?.HandleContextInitialized();
        }

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            _handler?.HandleScheduleMessagePumpWork(delayMs);
        }
    }
}
