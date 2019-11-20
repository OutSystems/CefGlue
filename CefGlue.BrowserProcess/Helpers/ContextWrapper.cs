using System;

namespace Xilium.CefGlue.BrowserProcess
{
    internal class ContextWrapper : IDisposable
    {
        private readonly bool _shallDispose;

        internal ContextWrapper(CefV8Context context, bool shallDispose)
        {
            V8Context = context;
            _shallDispose = shallDispose;
        }

        public CefV8Context V8Context { get; }

        public void Dispose()
        {
            V8Context.Exit();
            if (_shallDispose)
            {
                V8Context.Dispose();
            }
        }
    }
}
