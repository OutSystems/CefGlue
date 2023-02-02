using Xilium.CefGlue.BrowserProcess.ObjectBinding;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.JavascriptExecution
{
    internal class JavascriptExecutionEngineRenderSide
    {
        public JavascriptExecutionEngineRenderSide(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.JsEvaluationRequest.Name, HandleJavascriptEvaluation);
        }

        private static void HandleJavascriptEvaluation(MessageReceivedEventArgs args)
        {
            var frame = args.Frame;

            using (var context = frame.V8Context.EnterOrFail())
            {
                var message = Messages.JsEvaluationRequest.FromCefMessage(args.Message);

                // send script to browser
                var success = context.V8Context.TryEval(JavascriptHelper.WrapScriptForEvaluation(message.Script), message.Url, message.Line, out var value, out var exception);

                var response = new Messages.JsEvaluationResult()
                {
                    TaskId = message.TaskId,
                    Success = success,
                    Exception = success ? null : exception.Message,
                    ResultAsJson = value?.GetStringValue()
                };

                var cefResponseMessage = response.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Browser, cefResponseMessage);
            }
        }
    }
}