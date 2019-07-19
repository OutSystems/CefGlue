using Xilium.CefGlue.BrowserProcess.JavascriptExecution;
using Xilium.CefGlue.BrowserProcess.ObjectBinding;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.Handlers
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

        protected override void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            base.OnContextCreated(browser, frame, context);

            var message = new Messages.JsContextCreated();
            message.FrameId = frame.Name;
            browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
        }

        protected override void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
            _javascriptToNativeDispatcher.HandleContextReleased(context);
            base.OnContextReleased(browser, frame, context);

            var message = new Messages.JsContextReleased();
            message.FrameId = frame.Name;
            browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());
        }

        protected override void OnUncaughtException(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
        {
            // TODO
            base.OnUncaughtException(browser, frame, context, exception, stackTrace);
        }
    }
}
