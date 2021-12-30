using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodDispatcher : IDisposable
    {
        private class AsyncMethodExecutionResult
        {
            public readonly int CallId;
            public readonly CefFrame Frame;
            
            private readonly Func<object> _awaiter;

            public AsyncMethodExecutionResult(int callId, Func<object> awaiter, CefFrame frame)
            {
                CallId = callId;
                _awaiter = awaiter;
                Frame = frame;
            }

            public object GetResult() => _awaiter();
        }

        private readonly NativeObjectRegistry _objectRegistry;
        private readonly ActionBlock<AsyncMethodExecutionResult> _executionDispatcher;

        public NativeObjectMethodDispatcher(MessageDispatcher dispatcher, NativeObjectRegistry objectRegistry, int maxDegreeOfParallelism)
        {
            _objectRegistry = objectRegistry;

            _executionDispatcher = new ActionBlock<AsyncMethodExecutionResult>(
                DispatchAsyncMethodCall, 
                new ExecutionDataflowBlockOptions() { 
                    MaxDegreeOfParallelism = maxDegreeOfParallelism,
                    EnsureOrdered = true
                });

            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallRequest.Name, HandleNativeObjectCallRequest);
        }

        private void HandleNativeObjectCallRequest(MessageReceivedEventArgs args)
        {
            // message and arguments must be deserialized at this point because it will be disposed after
            var message = Messages.NativeObjectCallRequest.FromCefMessage(args.Message);
            var callId = message.CallId;
            
            var nativeObject = _objectRegistry.Get(message.ObjectName ?? "");
            if (nativeObject == null)
            {
                SendResult(callId, null, $"Object named {message.ObjectName} was not found. Make sure it was registered before.", args.Frame);
                return;
            }
            
            var nativeMethod = nativeObject.GetNativeMethod(message.MemberName ?? "");
            if (nativeMethod == null)
            {
                SendResult(callId, null, $"Object does not have a {message.MemberName} method.", args.Frame);
                return;
            }

            object result = null;
            Exception exception = null;
            try
            {
                result = nativeMethod.Execute(nativeObject.Target, message.ArgumentsOut, nativeObject.MethodHandler);
                if (nativeMethod.IsAsync)
                {
                    var awaiter = nativeMethod.GetResultWaiter(result);
                    _executionDispatcher.Post(new AsyncMethodExecutionResult(callId, awaiter, args.Frame));
                    return;
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            SendResult(callId, result, exception?.Message, args.Frame);
        }

        private static void DispatchAsyncMethodCall(AsyncMethodExecutionResult context)
        {
            using (CefObjectTracker.StartTracking())
            {
                object result = null;
                Exception exception = null;
                try
                {
                    result = context.GetResult();
                }
                catch (Exception e)
                {
                    exception = e;
                }
                
                SendResult(context.CallId, result, exception?.Message, context.Frame);
            }
        }
        
        private static void SendResult(int callId, object result, string exceptionMessage, CefFrame frame) 
        {
            var resultMessage = new Messages.NativeObjectCallResult()
            {
                CallId = callId,
                Result = new CefValueHolder(),
            };

            if (exceptionMessage != null)
            {
                resultMessage.Exception = exceptionMessage;
            }
            else
            {
                try
                {
                    CefValueSerialization.Serialize(result, resultMessage.Result);
                }
                catch (Exception e)
                {
                    resultMessage.Exception = e.Message;
                }
            }

            resultMessage.Success = resultMessage.Exception == null;
            
            var cefMessage = resultMessage.ToCefProcessMessage();
            if (frame.IsValid)
            {
                frame.SendProcessMessage(CefProcessId.Renderer, cefMessage);
            }
        }

        public void Dispose()
        {
            _executionDispatcher.Complete();
        }
    }
}
