using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            dispatcher.RegisterMessageHandler(Messages.JsContextCreated.Name, HandleContextCreatedMessage);
            dispatcher.RegisterMessageHandler(Messages.JsContextReleased.Name, HandleContextReleasedMessage);
            dispatcher.RegisterMessageHandler(Messages.JsUncaughtException.Name, HandleUncaughtExceptionMessage);
        }

        public bool IsMainFrameContextInitialized { get; private set; }

        public event Action<CefFrame> ContextCreated;
        public event Action<CefFrame> ContextReleased;
        public event Action<JavascriptUncaughtExceptionEventArgs> UncaughtException;

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Thread[{Thread.CurrentThread.ManagedThreadId}]-{DateTime.Now.ToString("mm':'ss.ffff")}####{message}");
        }

        private void HandleScriptEvaluationResultMessage(MessageReceivedEventArgs args)
        {
            var message = Messages.JsEvaluationResult.FromCefMessage(args.Message);

            LogMessage($"JavascriptExecutionEngine#HandleScriptEvaluationResultMessage#TaskId{message.TaskId}");

            if (_pendingTasks.TryRemove(message.TaskId, out var pendingTask))
            {
                if (message.Success)
                {
                    pendingTask.SetResult(CefValueSerialization.DeserializeCefValue(message.Result));
                    LogMessage($"JavascriptExecutionEngine#HandleScriptEvaluationResultMessage#TaskId{message.TaskId}#GotPendingTask#Success#SettingResult[Ended]");
                }
                else
                {
                    LogMessage($"JavascriptExecutionEngine#HandleScriptEvaluationResultMessage#TaskId{message.TaskId}#GotPendingTask#Failed");
                    pendingTask.SetException(new Exception(message.Exception));
                }
            }
        }

        private void HandleContextCreatedMessage(MessageReceivedEventArgs args)
        {
            //LogMessage("JavascriptExecutionEngine#HandleContextCreatedMessage");
            var message = Messages.JsContextCreated.FromCefMessage(args.Message);
            if (args.Frame.IsMain)
            {
                IsMainFrameContextInitialized = true;
            }
            ContextCreated?.Invoke(args.Frame);
        }

        private void HandleContextReleasedMessage(MessageReceivedEventArgs args)
        {
            //LogMessage("JavascriptExecutionEngine#HandleContextReleasedMessage");
            var message = Messages.JsContextReleased.FromCefMessage(args.Message);
            if (args.Frame.IsMain)
            {
                IsMainFrameContextInitialized = false;
            }
            ContextReleased?.Invoke(args.Frame);
        }

        private void HandleUncaughtExceptionMessage(MessageReceivedEventArgs args)
        {
            //LogMessage("JavascriptExecutionEngine#HandleUncaughtExceptionMessage");
            var message = Messages.JsUncaughtException.FromCefMessage(args.Message);
            var stackFrames = message.StackFrames.Select(f => new JavascriptStackFrame(f.FunctionName, f.ScriptNameOrSourceUrl, f.Column, f.LineNumber));
            UncaughtException?.Invoke(new JavascriptUncaughtExceptionEventArgs(args.Frame, message.Message, stackFrames.ToArray()));
        }

        public async Task<T> Evaluate<T>(string script, string url, int line, CefFrame frame, TimeSpan? timeout = null, bool forceExecute = false)
        {
            var taskId = lastTaskId++;
            var message = new Messages.JsEvaluationRequest()
            {
                TaskId = taskId,
                Script = script,
                Url = url,
                Line = line
            };
            var c = forceExecute ? TaskCreationOptions.None : TaskCreationOptions.RunContinuationsAsynchronously;
            var messageReceiveCompletionSource = new TaskCompletionSource<object>(c);

            _pendingTasks.TryAdd(taskId, messageReceiveCompletionSource);

            LogMessage($"JavascriptExecutionEngine#Evaluate#TaskId[{taskId}]#code[{script}]");

            try
            {
                var cefMessage = message.ToCefProcessMessage();
                frame.SendProcessMessage(CefProcessId.Renderer, cefMessage);

                LogMessage($"JavascriptExecutionEngine#Evaluate#TaskId[{taskId}]#MessageSent");

                if (timeout.HasValue)
                {
                    var tasks = new Task[]
                    {
                        messageReceiveCompletionSource.Task,
                        Task.Delay(timeout.Value)
                    };

                    var resultTask = await Task.WhenAny(tasks).ConfigureAwait(false);
                    if (resultTask != messageReceiveCompletionSource.Task)
                    {
                        // task evaluation timeout
                        throw new TaskCanceledException();
                    }
                }
                else
                {
                    LogMessage($"JavascriptExecutionEngine#Evaluate#TaskId[{taskId}]#MessageSent#AwaitingResult");
                    await messageReceiveCompletionSource.Task;
                    LogMessage($"JavascriptExecutionEngine#Evaluate#TaskId[{taskId}]#MessageSent#Continued...");
                }

                if (messageReceiveCompletionSource.Task.IsFaulted)
                {
                    throw messageReceiveCompletionSource.Task.Exception.InnerException;
                }
                
                return JavascriptToNativeTypeConverter.ConvertToNative<T>(messageReceiveCompletionSource.Task.Result);
            }
            catch
            {
                //System.Diagnostics.Debugger.Launch();
                _pendingTasks.TryRemove(taskId, out var _);
                throw;
            }
        }

        public void Dispose()
        {
            ContextCreated = null;
            ContextReleased = null;
            UncaughtException = null;
            foreach (var task in _pendingTasks)
            {
                task.Value.TrySetCanceled();
            }
        }
    }
}
