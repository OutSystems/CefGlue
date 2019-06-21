using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.JavascriptExecution
{
    internal class JavascriptExecutionEngine
    {
        private static volatile int lastTaskId;

        private readonly CefBrowser _browser;
        private readonly ConcurrentDictionary<int, TaskCompletionSource<object>> _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<object>>();

        public JavascriptExecutionEngine(CefBrowser browser, MessageDispatcher dispatcher) 
        {
            _browser = browser;

            dispatcher.RegisterMessageHandler(Messages.JsEvaluationResult.Name, HandleScriptEvaluationResultMessage);
        }

        private void HandleScriptEvaluationResultMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsEvaluationResult.FromCefMessage(args.Message);

            if (_pendingTasks.TryRemove(message.TaskId, out var pendingTask))
            {
                if (message.Success)
                {
                    pendingTask.SetResult(CefValueSerialization.DeserializeCefValue(message.Result));
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
