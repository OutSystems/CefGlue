namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal struct PromiseHolder
    {
        public CefV8Value Promise;
        public CefV8Value Resolve;
        public CefV8Value Reject;
        public CefV8Context Context;
    }
}
