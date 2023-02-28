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

            var nativeObject = _objectRegistry.Get(message.ObjectName ?? "");
            if (nativeObject == null)
            {
                SendResult(callId, null, $"Object named {message.ObjectName} was not found. Make sure it was registered before.", frame);
                return;
            }

            nativeObject.ExecuteMethod(message.MemberName, message.ArgumentsAsJson, (result, exception) =>
            {
                using (CefObjectTracker.StartTracking())
                {
                    SendResult(callId, result, exception?.Message, frame);
                }
            });
        }

        private static void SendResult(int callId, object result, string exceptionMessage, CefFrame frame)
        {
            var resultMessage = new Messages.NativeObjectCallResult()
            {
                CallId = callId,
            };

            if (exceptionMessage != null)
            {
                resultMessage.Exception = exceptionMessage;
            }
            else if (result is Task)
            {
                resultMessage.Exception = "Unexpected Task type result";
            }
            else
            {
                try
                {
                    resultMessage.ResultAsJson = Serializer.Serialize(result);
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
