namespace Xilium.CefGlue.Common
{
    internal class CommonCefApp : CefApp
    {
        private readonly CefBrowserProcessHandler _browserProcessHandler;

        internal CommonCefApp(CefBrowserProcessHandler browserProcessHandler = null)
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
    }
}
