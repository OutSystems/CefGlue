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
                if (result != null && nativeMethod.IsAsync)
                {
                    // if the method is async (ie returns a task), the following continuation
                    // will execute on another thread, and the current (single) thread will be freed
                    // for other calls
                    ((Task) result).ContinueWith(t =>
                    {
                        using (CefObjectTracker.StartTracking())
                        {
                            var taskResult = t.IsFaulted ? null : nativeMethod.GetResult(t);
                            SendResult(callId, taskResult, t.Exception?.Message, args.Frame);
                        }
                    });
                    return;
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            SendResult(callId, result, exception?.Message, args.Frame);
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
