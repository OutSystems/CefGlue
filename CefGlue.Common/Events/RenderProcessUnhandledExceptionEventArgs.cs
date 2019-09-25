using System;

namespace Xilium.CefGlue.Common.Events
{
    public class RenderProcessUnhandledExceptionEventArgs : EventArgs
    {
        public RenderProcessUnhandledExceptionEventArgs(string exceptionType, string message, string stackTrace)
        {
            ExceptionType = exceptionType;
            Message = message;
            StackTrace = stackTrace;
        }

        public string ExceptionType { get; }

        public string Message { get; }

        public string StackTrace { get; }
    }
}
