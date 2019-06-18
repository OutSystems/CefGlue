using Xilium.CefGlue.Common.RendererProcessCommunication;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class V8FunctionHandler : CefV8Handler
    {
        private long _objTrackId;

        public V8FunctionHandler(long objTrackId)
        {
            _objTrackId = objTrackId;
        }

        protected override bool Execute(string name, CefV8Value obj, CefV8Value[] arguments, out CefV8Value returnValue, out string exception)
        {
            //CefProcessMessage.Create(MessageNames.JsObjectDispatchCall);
            returnValue = CefV8Value.CreateNull();
            exception = null;
            return true;
        }
    }
}
