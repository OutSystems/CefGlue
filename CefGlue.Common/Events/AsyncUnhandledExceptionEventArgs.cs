using System;

namespace Xilium.CefGlue.Common.Events
{
    public class AsyncUnhandledExceptionEventArgs : EventArgs
    {
        internal AsyncUnhandledExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
