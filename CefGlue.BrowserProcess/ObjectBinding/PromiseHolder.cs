using System;

namespace Xilium.CefGlue.BrowserProcess.ObjectBinding
{
    internal class PromiseHolder
    {
        public delegate void ResolveHandler(params CefV8Value[] args);
        public delegate void RejectHandler(params CefV8Value[] args);

        private readonly CefV8Value _resolve;
        private readonly CefV8Value _reject;

        public CefV8Value Promise { get; }

        public CefV8Context Context { get; }

        public PromiseHolder(CefV8Value promise, CefV8Value resolve, CefV8Value reject, CefV8Context context)
        {
            Promise = promise;
            Context = context;
            _resolve = resolve;
            _reject = reject;
        }

        public void ResolveOrReject(Action<ResolveHandler, RejectHandler> action)
        {
            if (Context.Enter())
            {
                try
                {
                    try
                    {
                        action(Resolve, Reject);
                    }
                    catch (Exception e)
                    {
                        Reject(CefV8Value.CreateString(e.Message));
                    }
                }
                finally
                {
                    Context.Exit();
                }
            }
            else
            {
                // TODO
            }
        }

        private void Resolve(params CefV8Value[] args)
        {
            _resolve.ExecuteFunction(null, args);
        }

        private void Reject(params CefV8Value[] args)
        {
            _reject.ExecuteFunction(null, args);
        }
    }
}
