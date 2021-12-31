using System;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodDispatcher
    {
        private readonly NativeObjectRegistry _objectRegistry;
        
        public NativeObjectMethodDispatcher(MessageDispatcher dispatcher, NativeObjectRegistry objectRegistry)
        {
            _objectRegistry = objectRegistry;
                
            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallRequest.Name, HandleNativeObjectCall);
        }

        private void HandleNativeObjectCall(MessageReceivedEventArgs args)
        {
            // message and arguments must be deserialized at this point because it will be disposed after
            var message = Messages.NativeObjectCallRequest.FromCefMessage(args.Message);
            var frame = args.Frame;
            var callId = message.CallId;
            
            var nativeObjectInfo = _objectRegistry.Get(message.ObjectName ?? "");
            if (nativeObjectInfo == null)
            {
                SendResult(callId, null, $"Object named {message.ObjectName} was not found. Make sure it was registered before.", frame);
                return;
            }
            
            var nativeMethodInfo = nativeObjectInfo.GetNativeMethod(message.MemberName ?? "");
            if (nativeMethodInfo == null)
            {
                SendResult(callId, null, $"Object does not have a {message.MemberName} method.", frame);
                return;
            }
            
            object result = null;
            Exception exception = null;
            try
            {
                NativeMethod actualNativeMethodCalled;
                var targetObject = nativeObjectInfo.Target;
                var methodArgs = message.ArgumentsOut;
                 
                if (nativeObjectInfo.MethodHandler != null)
                {
                    // execute the interceptor method and pass the call to the original method as argument
                    var methodCall = nativeMethodInfo.MakeDelegate(targetObject, methodArgs);
                    result = nativeObjectInfo.MethodHandler(methodCall);
                    actualNativeMethodCalled = nativeObjectInfo.MethodHandlerInfo;
                }
                else
                {
                    // execute the target method
                    result = nativeMethodInfo.Execute(targetObject, methodArgs);
                    actualNativeMethodCalled = nativeMethodInfo;
                }
                
                if (result != null && actualNativeMethodCalled.IsAsync)
                {
                    // if the method is async (ie returns a task), the following continuation
                    // will execute on another thread, and the current (single) thread will be freed
                    // for other calls
                    ((Task) result).ContinueWith(t =>
                    {
                        using (CefObjectTracker.StartTracking())
                        {
                            var taskResult = t.IsFaulted ? null : actualNativeMethodCalled.GetResult(t);
                            SendResult(callId, taskResult, t.Exception?.Message, frame);
                        }
                    });
                    return;
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            SendResult(callId, result, exception?.Message, frame);
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
    }
}
