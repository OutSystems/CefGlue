using System;
using System.Linq;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

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
            var message = new Messages.NativeObjectCallRequest()
            {
                ObjectName = _objectName,
                MemberName = name,
                ArgumentsJson = arguments.Any() ? arguments[0].GetStringValue() : string.Empty
            };

            var promiseHolder = _functionCallHandler(message);

            if (promiseHolder != null)
            {
                returnValue = promiseHolder.Promise;
                exception = null;
            }
            else
            {
                returnValue = null;
                exception = "Failed to create promise";
            }

            return true;
        }
    }
}
