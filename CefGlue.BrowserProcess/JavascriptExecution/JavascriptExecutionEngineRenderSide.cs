using Xilium.CefGlue.BrowserProcess.Serialization;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using Xilium.CefGlue.Common.Serialization;

namespace Xilium.CefGlue.BrowserProcess.JavascriptExecution
{
    internal class JavascriptExecutionEngineRenderSide
    {
        public JavascriptExecutionEngineRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.JsEvaluationRequest.Name, HandleScriptEvaluation);
        }

        private void HandleScriptEvaluation(MessageReceivedEventArgs args)
        {
            var message = Messages.JsEvaluationRequest.FromCefMessage(args.Message);

            var browser = args.Browser;
            var context = browser.GetFrame(message.FrameId ?? "")?.V8Context;

            if (context != null && context.Enter())
            {
                try
                {
                    // send script to browser
                    var success = context.TryEval(message.Script, message.Url, message.Line, out var value, out var exception);
                    
                    var response = new Messages.JsEvaluationResult()
                    {
                        TaskId = message.TaskId,
                        Success = success,
                        Exception = success ? null : exception.Message,
                        Result = new CefValueHolder()
                    };

                    if (value != null)
                    {
                        V8ValueSerialization.SerializeV8Object(value, response.Result);
                    }

                    using (var cefResponseMessage = response.ToCefProcessMessage())
                    {
                        browser.SendProcessMessage(CefProcessId.Browser, cefResponseMessage);
                    }
                }
                finally
                {
                    context.Exit();
                }
            }
            else
            {
                // TODO
            }
        }
    }
}
