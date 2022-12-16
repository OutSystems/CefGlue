using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

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
            var frame = args.Frame;

            using (var context = frame.V8Context.EnterOrFail())
            {
                var message = Messages.JsEvaluationRequest.FromCefMessage(args.Message);

                // TODO - bcs - move method to JavaScriptHelper
                // send script to browser
                var success = context.V8Context.TryEval($"cefglue.evaluateScript(() => ({message.Script}))", message.Url, message.Line, out var value, out var exception);

                var response = new Messages.JsEvaluationResult()
                {
                    TaskId = message.TaskId,
                    Success = success,
                    Exception = success ? null : exception.Message,
                    ResultAsJson = value?.GetStringValue()
                };

                //if (value != null)
                //{
                //    System.Diagnostics.Debugger.Launch();
                //    V8ValueSerialization.SerializeV8ObjectToCefValue(value, response.ResultAsJson);
                //    //response.ResultAsJson = CefGlue.Common.Shared.Serialization.CefValueSerialization.DeserializeFromJson<object>(value.GetStringValue());
                //}

                var cefResponseMessage = response.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Browser, cefResponseMessage);
            }
        }
    }
}
