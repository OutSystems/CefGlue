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
            const int ResponseValueIndex = 1;

            var context = browser.GetMainFrame().V8Context;
            var taskId = message.Arguments.GetInt(0);
            var script = message.Arguments.GetString(1);
            var url = message.Arguments.GetString(2);
            var line = message.Arguments.GetInt(3);

            context.TryEval(script, url, line, out var value, out var exception);

            var response = CefProcessMessage.Create(JavascriptExecutionEngine.MessageNames.EvaluateJsResult);
            var responseArgs = response.Arguments;

            responseArgs.SetInt(0, taskId);

            if (exception != null)
            {
                // TODO
            }
            else if (value.IsArray)
            {
                // TODO
            }
            else if (value.IsBool)
            {
                responseArgs.SetBool(ResponseValueIndex, value.GetBoolValue());
            }
            else if (value.IsDate)
            {
                // TODO
            }
            else if (value.IsDouble)
            {
                responseArgs.SetDouble(ResponseValueIndex, value.GetDoubleValue());
            }
            else if (value.IsInt)
            {
                responseArgs.SetInt(ResponseValueIndex, value.GetIntValue());
            }
            else if (value.IsNull)
            {
                responseArgs.SetNull(ResponseValueIndex);
            }
            else if (value.IsObject)
            {
                // TODO
            }
            else if (value.IsString)
            {
                responseArgs.SetString(ResponseValueIndex, value.GetStringValue());
            }
            else if (value.IsUInt)
            {
                responseArgs.SetInt(ResponseValueIndex, (int)value.GetUIntValue());
            }

            browser.SendProcessMessage(CefProcessId.Browser, response);
        }
    }
}
