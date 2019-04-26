namespace Xilium.CefGlue.Common
{
    public class CommonCefApp : CefApp
    {
        private readonly CefBrowserProcessHandler _browserProcessHandler;

        public CommonCefApp(CefBrowserProcessHandler browserProcessHandler = null)
        {
            _browserProcessHandler = browserProcessHandler;
        }

        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (string.IsNullOrEmpty(processType))
            {
                commandLine.AppendSwitch("disable-gpu");
                commandLine.AppendSwitch("disable-gpu-compositing");
                commandLine.AppendSwitch("enable-begin-frame-scheduling");
                commandLine.AppendSwitch("disable-smooth-scrolling");
            }
        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            return _browserProcessHandler;
        }
    }
}
