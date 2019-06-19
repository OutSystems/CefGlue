using System;
using Xilium.CefGlue.Common.JavascriptExecution;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        protected override void OnWebKitInitialized()
        {
            base.OnWebKitInitialized();
            PromiseFactory.Register();
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
           switch(message.Name)
            {
                case Messages.JsEvaluationRequest.Name:
                    JavascriptExecutionEngineRenderSide.HandleScriptEvaluation(browser, message);
                    return true;

                case Messages.NativeObjectRegistrationRequest.Name:
                    NativeObjectRegistryRenderSide.HandleNativeObjectRegistration(browser, message);
                    return true;

                case Messages.NativeObjectCallResult.Name:
                    NativeObjectMethodRunnerRenderSide.HandleNativeObjectCallResult(browser, message);
                    return true;
            }

            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }
    }
}
