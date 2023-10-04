using System;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class PromiseHolder : IDisposable
    {
        public delegate void ResolveHandler(CefV8Value arg);
        public delegate void RejectHandler(string errorMessage);

        public CefV8Value Promise { get; }

        public CefV8Context Context { get; }

        public PromiseHolder(CefV8Value promise, CefV8Context context)
        {
            Promise = promise;
            Context = context;
        }

        public void ResolveOrReject(Action<ResolveHandler, RejectHandler> action)
        {
            try
            {
                action(Resolve, Reject);
            }
            catch (Exception e)
            {
                Reject(e.Message);
            }
        }

        private void Resolve(CefV8Value arg)
        {
            Promise.ResolvePromise(arg);
        }

        private void Reject(string errorMessage)
        {
            Promise.RejectPromise(errorMessage);
        }

        public void Dispose()
        {
            Promise.Dispose();
        }
    }
}
