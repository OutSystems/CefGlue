using System;

namespace Xilium.CefGlue.Common.Events
{
    public class RenderProcessUnhandledException : Exception
    {
        private readonly string _stackTrace;

        public RenderProcessUnhandledException(string exceptionType, string message, string stackTrace) : base(message)
        {
            ExceptionType = exceptionType;
            _stackTrace = stackTrace;
        }

        public string ExceptionType { get; }

        public override string StackTrace => _stackTrace;
    }
}
