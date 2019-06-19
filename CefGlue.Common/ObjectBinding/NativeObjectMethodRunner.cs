using System.Threading.Tasks;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObjectMethodRunner
    {
        private readonly CefBrowser _browser;
        private readonly NativeObjectRegistry _objectRegistry;

        public NativeObjectMethodRunner(CefBrowser browser, CommonCefClient cefClient, NativeObjectRegistry objectRegistry)
        {
            _browser = browser;
            _objectRegistry = objectRegistry;

            cefClient.RegisterMessageHandler(Messages.NativeObjectCallRequest.Name, (o, e) => HandleNativeObjectCallDispatch(e.Message));
        }

        private void HandleNativeObjectCallDispatch(CefProcessMessage cefMessage)
        {
            var message = Messages.NativeObjectCallRequest.FromCefMessage(cefMessage);
            var targetObj = _objectRegistry.Get(message.ObjectName);

            if (targetObj != null)
            {
                Task.Run(() =>
                {
                    ExecuteMethod(targetObj, message.MemberName, new object[0]);
                });
            }
            else
            {
                // TODO send error when object is not found
            }
        }

        private void ExecuteMethod(object targetObj, string methodName, object[] args)
        {

            // TODO :
            // find .net method
            // call .net method
            var message = new Messages.NativeObjectCallResult()
            {
            };
            _browser.SendProcessMessage(CefProcessId.Renderer, message.ToCefProcessMessage());
        }
    }
}
