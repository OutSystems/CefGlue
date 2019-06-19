using System.Collections.Concurrent;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class V8FunctionHandler : CefV8Handler
    {
        private readonly string _objectName;
        
        public V8FunctionHandler(string objectName)
        {
            _objectName = objectName;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var promiseHolder = PromiseFactory.CreatePromise();

            var argsCopy = V8Serialization.SerializeV8Object(arguments); // create a copy of the args to pass to the browser process
            var message = new Messages.NativeObjectCallRequest()
            {
                ObjectName = _objectName,
                MemberName = name,
                Arguments = argsCopy
            };
            
            var browser = CefV8Context.GetCurrentContext().GetBrowser();
            browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());

            returnValue = promiseHolder.Promise;
            exception = null;
            return true;
        }

        private void ResolvePendingExecution()
        {

        }
    }
}
