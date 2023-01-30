using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.JavascriptExecution
{
    internal class JavascriptExecutionEngine : IDisposable
    {
        private static volatile int lastTaskId;

        private readonly ConcurrentDictionary<int, TaskCompletionSource<object>> _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<object>>();

        public JavascriptExecutionEngine(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.JsEvaluationResult.Name, HandleScriptEvaluationResultMessage);
            dispatcher.RegisterMessageHandler(Messages.JsUncaughtException.Name, HandleUncaughtExceptionMessage);
        }

        public bool IsMainFrameContextInitialized { get; private set; }

        public event Action<CefFrame> ContextCreated;
        public event Action<CefFrame> ContextReleased;
        public event Action<JavascriptUncaughtExceptionEventArgs> UncaughtException;

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

        private void HandleUncaughtExceptionMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsUncaughtException.FromCefMessage(args.Message);
            var stackFrames = message.StackFrames.Select(f => new JavascriptStackFrame(f.FunctionName, f.ScriptNameOrSourceUrl, f.Column, f.LineNumber));
            UncaughtException?.Invoke(new JavascriptUncaughtExceptionEventArgs(args.Frame, message.Message, stackFrames.ToArray()));
        }

        public void HandleFrameAttached(CefFrame frame)
        {
            if (frame.IsMain)
            {
                IsMainFrameContextInitialized = true;
            }
            ContextCreated?.Invoke(frame);
        }

        public void HandleFrameDetached(CefFrame frame)
        {
            if (frame.IsMain)
            {
                IsMainFrameContextInitialized = false;
            }
            ContextReleased?.Invoke(frame);
        }

        public Task<T> Evaluate<T>(string script, string url, int line, CefFrame frame, TimeSpan? timeout = null)
        {
            var taskId = lastTaskId++;
            var message = new Messages.JsEvaluationRequest()
            {
                TaskId = taskId,
                Script = script,
                Url = url,
                Line = line
            };

            var messageReceiveCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            _pendingTasks.TryAdd(taskId, messageReceiveCompletionSource);

            try
            {
                var cefMessage = message.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Renderer, cefMessage);

                var evaluationTask = messageReceiveCompletionSource.Task;

                if (timeout.HasValue)
                {
                    var tasks = Task.WhenAny(new[] {
                        evaluationTask,
                        Task.Delay(timeout.Value)
                    });

                    return tasks.ContinueWith(resultTask => ProcessResult<T>(evaluationTask, taskId, timedOut: resultTask.Result != evaluationTask));
                }
                else
                {
                    return evaluationTask.ContinueWith(task => ProcessResult<T>(task, taskId));
                }
            }
            catch
            {
                _pendingTasks.TryRemove(taskId, out var _);
                throw;
            }
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine($"###JavascriptExecutionEngine#Dispose", DateTime.Now.ToString("o"));
            ContextCreated = null;
            ContextReleased = null;
            UncaughtException = null;
            foreach (var task in _pendingTasks)
            {
                task.Value.TrySetCanceled();
            }
        }

        private T ProcessResult<T>(Task<object> task, int taskId, bool timedOut = false)
        {
            try
            {
                if (timedOut)
                {
                    // task evaluation timeout
                    throw new TaskCanceledException();
                }

                if (task.IsFaulted)
                {
                    throw task.Exception.InnerException;
                }

                return JavascriptToNativeTypeConverter.ConvertToNative<T>(task.Result);
            }
            catch (Exception e)
            {
                _pendingTasks.TryRemove(taskId, out var _);

                if (e is AggregateException && e.InnerException is TaskCanceledException)
                {
                    return default;
                }
                throw;
            }
        }
    }
}
