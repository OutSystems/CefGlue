using Xilium.CefGlue.Common.Handlers;
using Xilium.CefGlue.Common.Shared;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonBrowserProcessHandler : CefBrowserProcessHandler
    {
        private readonly CustomScheme[] _customSchemes;
        private readonly BrowserProcessHandler _handler;

        public CommonBrowserProcessHandler(BrowserProcessHandler handler, CustomScheme[] customSchemes)
        {
            _customSchemes = customSchemes;
            _handler = handler;
        }

        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            _handler?.HandleBeforeChildProcessLaunch(commandLine);

            if (_customSchemes?.Length > 0)
            {
                commandLine.AppendArgument(CommandLineArgs.CustomScheme + CustomScheme.ToCommandLineValue(_customSchemes));
            }
        }

        protected override CefPrintHandler GetPrintHandler()
        {
            return _handler?.HandleGetPrintHandler();
        }

        protected override void OnContextInitialized()
        {
            _handler?.HandleContextInitialized();
        }

        protected override void OnRenderProcessThreadCreated(CefListValue extraInfo)
        {
            _handler?.HandleRenderProcessThreadCreated(extraInfo);
        }

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            _handler?.HandleScheduleMessagePumpWork(delayMs);
        }
    }
}
