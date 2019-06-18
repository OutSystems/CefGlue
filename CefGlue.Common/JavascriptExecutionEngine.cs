using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common
{
    internal class JavascriptExecutionEngine
    {

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
                case Messages.JsEvaluationResponse.Name:
                    HandleScriptEvaluationResultMessage(e.Message);
                    break;
            }
        }

        private void HandleScriptEvaluationResultMessage(CefProcessMessage cefMessage)
        {
            var message = Messages.JsEvaluationResponse.FromCefMessage(cefMessage);

            if (_pendingTasks.TryRemove(message.TaskId, out var pendingTask))
            {
                if (message.Success)
                {
                    pendingTask.SetResult(V8Serialization.DeserializeV8Object(message.Result));
                }
                else
                {
                    pendingTask.SetException(new Exception(message.Exception));
                }
            }
        }

        public async Task<T> Evaluate<T>(string script, string url, int line)
        {
            var taskId = lastTaskId++;
            var message = new Messages.JsEvaluationRequest()
            {
                TaskId = taskId,
                Script = script,
                Url = url,
                Line = line
            };

            var messageReceiveCompletionSource = new TaskCompletionSource<object>();

            _pendingTasks.TryAdd(taskId, messageReceiveCompletionSource);

            try
            {
                _browser.SendProcessMessage(CefProcessId.Renderer, message.ToCefProcessMessage());

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
