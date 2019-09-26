using System;
using Xilium.CefGlue.BrowserProcess.Serialization;
using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class V8FunctionHandler : CefV8Handler
    {
        private readonly string _objectName;
        private readonly Func<Messages.NativeObjectCallRequest, PromiseHolder> _functionCallHandler;

        public V8FunctionHandler(string objectName, Func<Messages.NativeObjectCallRequest, PromiseHolder> functionCallHandler)
        {
            _objectName = objectName;
            _functionCallHandler = functionCallHandler;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            var argsCopy = V8ValueSerialization.SerializeV8Object(arguments); // create a copy of the args to pass to the browser process
            var message = new Messages.NativeObjectCallRequest()
            {
                ObjectName = _objectName,
                MemberName = name,
                Arguments = argsCopy
            };

            var promiseHolder = _functionCallHandler(message);

            returnValue = promiseHolder.Promise;
            exception = null;
            return true;
        }
    }
}
