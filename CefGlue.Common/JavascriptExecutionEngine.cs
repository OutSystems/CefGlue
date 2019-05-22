using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common
{
    internal class JavascriptExecutionEngine
    {
        public static class MessageNames
        {
            public const string EvaluateJs = "EvaluateJs";
            public const string EvaluateJsResult = "EvaluateJsResult";
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
            var taskId = message.Arguments.GetInt(0);
            if (_pendingTasks.TryRemove(taskId, out var pendingTask))
            {
                var messageArgs = message.Arguments;
                var success = messageArgs.GetBool(1);

                if (success)
                {
                    pendingTask.SetResult(V8Serialization.DeserializeV8Object(messageArgs.GetValue(2)));
                }
                else
                {
                    pendingTask.SetException(new Exception(messageArgs.GetString(3)));
                }
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

            if (typeof(T) == typeof(double) && result is int)
            {
                // javascript numbers sometimes can lose the decimal part becoming integers, prevent that
                return (T) (object) Convert.ToDouble(result);
            }

            return (T)result;
        }
    }
}
