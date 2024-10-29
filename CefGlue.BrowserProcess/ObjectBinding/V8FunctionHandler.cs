using System;
using System.Linq;
using Xilium.CefGlue.Common.Shared.RendererProcessCommunication;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    delegate bool CallHandler(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception);

    internal class V8FunctionHandler : CefV8Handler
    {
        private readonly CallHandler _functionCallHandler;

        public V8FunctionHandler(CallHandler functionCallHandler)
        {
            _functionCallHandler = functionCallHandler;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            return _functionCallHandler(name, obj, arguments, out returnValue, out exception);
        }
    }
}
