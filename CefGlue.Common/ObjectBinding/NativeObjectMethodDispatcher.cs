using System;
using System.Threading.Tasks.Dataflow;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodDispatcher : IDisposable
    {
        private class MethodExecutionContext
        {
            public readonly int CallId;
            public readonly string ObjectName;
            public readonly string MemberName;
            public readonly object[] Arguments;
            public readonly CefFrame Frame;

            public MethodExecutionContext(int callId, string objectName, string memberName, object[] arguments, CefFrame frame)
            {
                CallId = callId;
                ObjectName = objectName;
                MemberName = memberName;
                Arguments = arguments;
                Frame = frame;
            }
        }

        private readonly NativeObjectRegistry _objectRegistry;
        private readonly ActionBlock<MethodExecutionContext> _executionDispatcher;

        public NativeObjectMethodDispatcher(MessageDispatcher dispatcher, NativeObjectRegistry objectRegistry, int maxDegreeOfParallelism)
        {
            _objectRegistry = objectRegistry;

            _executionDispatcher = new ActionBlock<MethodExecutionContext>(
                DispatchNativeObjectCall, 
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

            _executionDispatcher.Post(new MethodExecutionContext(message.CallId, message.ObjectName, message.MemberName, message.ArgumentsOut, args.Frame));
        }

        private void DispatchNativeObjectCall(MethodExecutionContext context) 
        {
            using (CefObjectTracker.StartTracking())
            {
                var resultMessage = new Messages.NativeObjectCallResult()
                {
                    CallId = context.CallId,
                    Result = new CefValueHolder(),
                };

                try
                {
                    var targetObj = _objectRegistry.Get(context.ObjectName);
                    if (targetObj != null)
                    {
                        var nativeMethod = targetObj.GetNativeMethod(context.MemberName);
                        if (nativeMethod != null)
                        {
                            var result = NativeObjectMethodExecutor.ExecuteMethod(targetObj.Target, nativeMethod, context.Arguments, targetObj.MethodHandler);
                            CefValueSerialization.Serialize(result, resultMessage.Result);
                            resultMessage.Success = true;
                        }
                        else
                        {
                            resultMessage.Success = false;
                            resultMessage.Exception = $"Object does not have a {context.ObjectName} method.";
                        }
                    }
                    else
                    {
                        resultMessage.Success = false;
                        resultMessage.Exception = $"Object named {context.ObjectName} was not found. Make sure it was registered before.";
                    }
                }
                catch (Exception e)
                {
                    resultMessage.Success = false;
                    resultMessage.Exception = e.Message;
                }

                var cefMessage = resultMessage.ToCefProcessMessage();
                if (context.Frame.IsValid)
                {
                    context.Frame.SendProcessMessage(CefProcessId.Renderer, cefMessage);
                }
            }
        }

        public void Dispose()
        {
            _executionDispatcher.Complete();
        }
    }
}
