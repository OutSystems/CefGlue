using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        private JavascriptExecutionEngineRenderSide _javascriptExecutionEngine;
        private NativeObjectRegistryRenderSide _nativeObjectRegistry;

        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        protected override void OnWebKitInitialized()
        {
            base.OnWebKitInitialized();
            _javascriptExecutionEngine = new JavascriptExecutionEngineRenderSide(_messageDispatcher);
            _nativeObjectRegistry = new NativeObjectRegistryRenderSide(_messageDispatcher);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            _messageDispatcher.DispatchMessage(browser, sourceProcess, message);
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _nativeObjectRegistry.HandleContextReleased(context);
            base.OnContextReleased(browser, frame, context);
        }
    }
}
