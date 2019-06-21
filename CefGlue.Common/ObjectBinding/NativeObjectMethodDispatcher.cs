using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using Xilium.CefGlue.Common.Serialization;

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
            // message must be deserialized at this point because it will be disposed after
            var message = Messages.NativeObjectCallRequest.FromCefMessage(args.Message);
            
            Task.Run(() =>
            {
                try
                {
                    var targetObj = _objectRegistry.Get(message.ObjectName);
                    if (targetObj != null)
                    {
                        var nativeMethodName = targetObj.GetNativeMethodName(message.MemberName);
                        var arguments = CefValueSerialization.DeserializeCefList<object>(message.Arguments);
                        
                        ExecuteMethod(args.Browser, message.CallId, targetObj.Target, nativeMethodName, arguments);
                    }
                    else
                    {
                        // TODO send error when object is not found
                    }
                }
                catch
                {
                    // TODO handle error
                }
            });
        }

        private void ExecuteMethod(CefBrowser browser, int callId, object targetObj, string methodName, object[] args)
        {
            // TODO :
            // serialize arguments properly based on method param types

            var method = targetObj.GetType().GetMethod(methodName);
            var result = method.Invoke(targetObj, args);

            var cefValue = CefValueSerialization.Serialize(result);

            var message = new Messages.NativeObjectCallResult()
            {
                CallId = callId,
                Success = true,
                Result = cefValue
            };
            browser.SendProcessMessage(CefProcessId.Renderer, message.ToCefProcessMessage());
        }
    }
}
