using System;
using Xilium.CefGlue.Common.ObjectBinding;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common
{
    internal class CommonCefRenderProcessHandler : CefRenderProcessHandler
    {
        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
           switch(message.Name)
            {
                case Messages.JsEvaluationRequest.Name:
                    HandleScriptEvaluationMessage(browser, message);
                    return true;

                case Messages.JsObjectRegistrationRequest.Name:
                    HandleJavascriptObjectRegistration(browser, message);
                    return true;
            }

            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        private void HandleScriptEvaluationMessage(CefBrowser browser, CefProcessMessage cefMessage)
        {
            var context = browser.GetMainFrame().V8Context;

            if (context.Enter()) {
                try
                {
                    var message = Messages.JsEvaluationRequest.FromCefMessage(cefMessage);

                    // send script to browser
                    var success = context.TryEval(message.Script, message.Url, message.Line, out var value, out var exception);

                    var response = new Messages.JsEvaluationResponse()
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

        private void HandleJavascriptObjectRegistration(CefBrowser browser, CefProcessMessage cefMessage)
        {
            var context = browser.GetMainFrame().V8Context;

            if (context.Enter())
            {
                try
                {
                    var message = Messages.JsObjectRegistrationRequest.FromCefMessage(cefMessage);

                    var global = context.GetGlobal();
                    var handler = new V8FunctionHandler(message.ObjectTrackId);
                    var attributes = CefV8PropertyAttribute.ReadOnly | CefV8PropertyAttribute.DontEnum | CefV8PropertyAttribute.DontDelete;

                    using (var v8Obj = CefV8Value.CreateObject())
                    {
                        var functionName = "testFunction";
                        using (var v8Function = CefV8Value.CreateFunction(functionName, handler))
                        {
                            v8Obj.SetValue(functionName, v8Function, attributes);
                        }
                        global.SetValue("test", v8Obj);
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
