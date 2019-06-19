using System.Threading.Tasks;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodRunner
    {
        private readonly NativeObjectRegistry _objectRegistry;

        public NativeObjectMethodRunner(MessageDispatcher dispatcher, NativeObjectRegistry objectRegistry)
        {
            _objectRegistry = objectRegistry;

            dispatcher.RegisterMessageHandler(Messages.NativeObjectCallRequest.Name, HandleNativeObjectCallRequest);
        }

        private void HandleNativeObjectCallRequest(MessageReceivedEventArgs args)
        {
            var message = Messages.NativeObjectCallRequest.FromCefMessage(args.Message);
            var targetObj = _objectRegistry.Get(message.ObjectName);

            if (targetObj != null)
            {
                Task.Run(() =>
                {
                    ExecuteMethod(args.Browser, message.CallId, targetObj, message.MemberName, new object[0]);
                });
            }
            else
            {
                // TODO send error when object is not found
            }
        }

        private void ExecuteMethod(CefBrowser browser, int callId, object targetObj, string methodName, object[] args)
        {

            // TODO :
            // find .net method
            // call .net method
            var result = CefValue.Create();
            result.SetString("Hello world!");

            var message = new Messages.NativeObjectCallResult()
            {
                CallId = callId,
                Success = true,
                Result = result
            };
            browser.SendProcessMessage(CefProcessId.Renderer, message.ToCefProcessMessage());
        }
    }
}
