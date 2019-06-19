using System;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        private JavascriptExecutionEngineRenderSide _javascriptExecutionEngine;
        private NativeObjectRegistryRenderSide _nativeObjectRegistry;
        private NativeObjectMethodRunnerRenderSide _nativeObjectMethodRunner;

        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        protected override void OnWebKitInitialized()
        {
            base.OnWebKitInitialized();
            _javascriptExecutionEngine = new JavascriptExecutionEngineRenderSide(_messageDispatcher);
            _nativeObjectRegistry = new NativeObjectRegistryRenderSide(_messageDispatcher);
            _nativeObjectMethodRunner = new NativeObjectMethodRunnerRenderSide(_messageDispatcher);
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            _messageDispatcher.DispatchMessage(browser, sourceProcess, message);
            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }
    }
}
