using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Events;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.JavascriptExecution
{
    internal class JavascriptExecutionEngine
    {
        private static volatile int lastTaskId;

        private readonly ConcurrentDictionary<int, TaskCompletionSource<object>> _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<object>>();
        
        public JavascriptExecutionEngine(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterMessageHandler(Messages.JsEvaluationResult.Name, HandleScriptEvaluationResultMessage);
            dispatcher.RegisterMessageHandler(Messages.JsContextCreated.Name, HandleContextCreatedMessage);
            dispatcher.RegisterMessageHandler(Messages.JsContextReleased.Name, HandleContextReleasedMessage);
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

        private void HandleContextCreatedMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsContextCreated.FromCefMessage(args.Message);
            if (args.Frame.IsMain)
            {
                IsMainFrameContextInitialized = true;
            }
            ContextCreated?.Invoke(args.Frame);
        }

        private void HandleContextReleasedMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsContextReleased.FromCefMessage(args.Message);
            if (args.Frame.IsMain)
            {
                IsMainFrameContextInitialized = false;
            }
            ContextReleased?.Invoke(args.Frame);
        }

        private void HandleUncaughtExceptionMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsUncaughtException.FromCefMessage(args.Message);
            var stackFrames = message.StackFrames.Select(f => new JavascriptStackFrame(f.FunctionName, f.ScriptNameOrSourceUrl, f.Column, f.LineNumber));
            UncaughtException?.Invoke(new JavascriptUncaughtExceptionEventArgs(args.Frame, message.Message, stackFrames.ToArray()));
        }

        public async Task<T> Evaluate<T>(string script, string url, int line, CefFrame frame, TimeSpan? timeout = null)
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

                var tasks = new List<Task>(2)
                {
                    messageReceiveCompletionSource.Task
                };

                Task timeoutTask = null;
                if (timeout.HasValue)
                {
                    timeoutTask = Task.Delay(timeout.Value);
                    tasks.Add(timeoutTask);
                }

                var resultTask = await Task.WhenAny(tasks);
                if (resultTask == timeoutTask)
                {
                    // task evaluation timeout
                    throw new TaskCanceledException();
                }

                if (messageReceiveCompletionSource.Task.IsFaulted)
                {
                    throw messageReceiveCompletionSource.Task.Exception.InnerException;
                }

                return JavascriptToNativeTypeConverter.ConvertToNative<T>(messageReceiveCompletionSource.Task.Result);
            }
            catch
            {
                _pendingTasks.TryRemove(taskId, out var _);
                throw;
            }
        }
    }
}
