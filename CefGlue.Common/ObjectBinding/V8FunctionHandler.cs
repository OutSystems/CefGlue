using System.Collections.Generic;
using Xilium.CefGlue.Common.RendererProcessCommunication;
using static Xilium.CefGlue.Common.ObjectBinding.PromiseFactory;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class V8FunctionHandler : CefV8Handler
    {
        private static volatile int lastCallId;

        private readonly string _objectName;
        private readonly IDictionary<int, PromiseHolder> _promisesStorage;

        public V8FunctionHandler(string objectName, IDictionary<int, PromiseHolder> promisesStorage)
        {
            _objectName = objectName;
            _promisesStorage = promisesStorage;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var promiseHolder = PromiseFactory.CreatePromise();

            var argsCopy = V8Serialization.SerializeV8Object(arguments); // create a copy of the args to pass to the browser process
            var message = new Messages.NativeObjectCallRequest()
            {
                CallId = lastCallId++,
                ObjectName = _objectName,
                MemberName = name,
                Arguments = argsCopy
            };

            _promisesStorage.Add(message.CallId, promiseHolder);

            var browser = CefV8Context.GetCurrentContext().GetBrowser();
            browser.SendProcessMessage(CefProcessId.Browser, message.ToCefProcessMessage());

            returnValue = promiseHolder.Promise;
            exception = null;
            return true;
        }
    }
}
