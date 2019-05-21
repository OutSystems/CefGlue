using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common
{
    internal class JavascriptExecutionEngine
    {
        public static class MessageNames
        {
            public const string EvaluateJs = "evaluateJs";
            public const string EvaluateJsResult = "evaluateJsResult";
        }

        private static volatile int lastTaskId;

        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<object>> _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<object>>();

        public JavascriptExecutionEngine(CefBrowser browser, CommonCefClient cefClient) 
        {
            _browser = browser;

            cefClient.MessageReceived += OnCefClientMessageReceived;
        }

        private void OnCefClientMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch(e.Message.Name)
            {
                case MessageNames.EvaluateJsResult:
                    HandleScriptEvaluationResultMessage(e.Message);
                    break;
            }
        }

        private void HandleScriptEvaluationResultMessage(CefProcessMessage message)
        {
            const int ResponseValueIndex = 1;

            var taskId = message.Arguments.GetInt(0);
            if (_pendingTasks.TryRemove(taskId, out var pendingTask))
            {
                object clrResult = null;

                var messageArgs = message.Arguments;
                var result = messageArgs.GetValue(1);
                
                switch(messageArgs.GetValueType(1))
                {
                    case CefValueType.Bool:
                        clrResult = messageArgs.GetBool(ResponseValueIndex);
                        break;

                    case CefValueType.Dictionary:
                        // TODO clrResult = messageArgs.GetBool(ResponseValue);
                        break;

                    case CefValueType.Double:
                        clrResult = messageArgs.GetDouble(ResponseValueIndex);
                        break;

                    case CefValueType.List:
                        //  TODO
                        break;

                    case CefValueType.Int:
                        clrResult = messageArgs.GetInt(ResponseValueIndex);
                        break;

                    case CefValueType.String:
                        clrResult = messageArgs.GetString(ResponseValueIndex);
                        break;

                    case CefValueType.Null:
                        break;
                }
                pendingTask.SetResult(clrResult);
            }
        }

        public async Task<T> Evaluate<T>(string script, string url, int line)
        {
            var taskId = lastTaskId++;
            var message = CefProcessMessage.Create(MessageNames.EvaluateJs);
            message.Arguments.SetInt(0, taskId);
            message.Arguments.SetString(1, script);
            message.Arguments.SetString(2, url);
            message.Arguments.SetInt(3, line);

            var messageReceiveCompletionSource = new TaskCompletionSource<object>();

            _pendingTasks.TryAdd(taskId, messageReceiveCompletionSource);

            try
            {
                _browser.SendProcessMessage(CefProcessId.Renderer, message);

                await messageReceiveCompletionSource.Task;
            }
            catch
            {
                _pendingTasks.TryRemove(taskId, out var dummy);
                throw;
            }

            var result = messageReceiveCompletionSource.Task.Result;
            return (T) result;
        }
    }
}
