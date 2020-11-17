using System;

namespace Xilium.CefGlue.BrowserProcess
{
    internal static class ContextExtensions
    {
        public static ContextWrapper EnterOrFail(this CefV8Context context, bool shallDispose = true)
        {
            if (!context.Enter())
            {
                throw new InvalidOperationException("Could not enter context");
            }
            return new ContextWrapper(context, shallDispose);
        }
    }
}
