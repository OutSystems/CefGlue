using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        private JavascriptExecutionEngineRenderSide _javascriptExecutionEngine;
        private JavascriptToNativeDispatcherRenderSide _javascriptToNativeDispatcher;

        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        protected override void OnWebKitInitialized()
        {
            base.OnWebKitInitialized();
            _javascriptExecutionEngine = new JavascriptExecutionEngineRenderSide(_messageDispatcher);
            _javascriptToNativeDispatcher = new JavascriptToNativeDispatcherRenderSide(_messageDispatcher);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            _messageDispatcher.DispatchMessage(browser, sourceProcess, message);
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            // TODO make this available
            _javascriptToNativeDispatcher.HandleContextReleased(context);
            base.OnContextReleased(browser, frame, context);
        }

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            // TODO make this available
            base.OnContextCreated(browser, frame, context);
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            // TODO make this available
            base.OnUncaughtException(browser, frame, context, exception, stackTrace);
        }
    }
}
