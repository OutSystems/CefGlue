using System;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
           switch(message.Name)
            {
                case JavascriptExecutionEngine.MessageNames.EvaluateJs:
                    HandleScriptEvaluationMessage(browser, message);
                    return true;
            }

            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        private void HandleScriptEvaluationMessage(CefBrowser browser, CefProcessMessage message)
        {
            var context = browser.GetMainFrame().V8Context;

            var response = CefProcessMessage.Create(JavascriptExecutionEngine.MessageNames.EvaluateJsResult);
            var responseArgs = response.Arguments;

            if (context.Enter()) {
                try
                {
                    var taskId = message.Arguments.GetInt(0);
                    var script = message.Arguments.GetString(1);
                    var url = message.Arguments.GetString(2);
                    var line = message.Arguments.GetInt(3);

                    var success = context.TryEval(script, url, line, out var value, out var exception);

                    responseArgs.SetInt(0, taskId);
                    responseArgs.SetBool(1, success);

                    if (success)
                    {
                        V8Serialization.SerializeV8Object(value, responseArgs, 2);
                    }
                    else
                    {
                        responseArgs.SetString(3, BuildExceptionString(exception));
                    }

                    browser.SendProcessMessage(CefProcessId.Browser, response);
                }
                finally
                {
                    context.Exit();
                }
            } else
            {
                // TODO
            }
        }

        private string BuildExceptionString(CefV8Exception exception)
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
