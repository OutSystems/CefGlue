using System;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.JavascriptExecution
{
    internal static class JavascriptExecutionEngineRenderSide
    {
        public static void HandleScriptEvaluation(CefBrowser browser, CefProcessMessage cefMessage)
        {
            // TODO get the appropriate frame
            var context = browser.GetMainFrame().V8Context;

            if (context.Enter())
            {
                try
                {
                    var message = Messages.JsEvaluationRequest.FromCefMessage(cefMessage);

                    // send script to browser
                    var success = context.TryEval(message.Script, message.Url, message.Line, out var value, out var exception);

                    var response = new Messages.JsEvaluationResult()
                    {
                        TaskId = message.TaskId,
                        Success = success,
                        Exception = success ? null : BuildExceptionString(exception)
                    };

                    var cefResponseMessage = response.ToCefProcessMessage();

                    if (success)
                    {
                        V8Serialization.SerializeV8Object(value, cefResponseMessage.Arguments, 2);
                    }

                    browser.SendProcessMessage(CefProcessId.Browser, cefResponseMessage);
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

        private static string BuildExceptionString(CefV8Exception exception)
        {
            // TODO improve exception: shall we send all data in an object?
            var result = exception.Message + Environment.NewLine;
            if (!string.IsNullOrEmpty(exception.ScriptResourceName))
            {
                result += exception.ScriptResourceName;
            }
            result += ":" + exception.LineNumber + ":" + exception.StartColumn;
            return result;
        }
    }
}
