using System;

namespace Xilium.CefGlue.Common.Helpers.Logger
{
    public interface ILogger
    {
        void TraceException(string message, Exception exception);

        void Trace(string message);

        void DebugException(string message, Exception exception);

        void Debug(string message);

        void ErrorException(string message, Exception exception);

        void Error(string message);

        void FatalException(string message, Exception exception);

        void Fatal(string message);

        void InfoException(string message, Exception exception);

        void Info(string message);

        void WarnException(string message, Exception exception);

        void Warn(string message);

        bool IsTraceEnabled { get; }

        bool IsDebugEnabled { get; }

        bool IsErrorEnabled { get; }

        bool IsFatalEnabled { get; }

        bool IsInfoEnabled { get; }

        bool IsWarnEnabled { get; }
    }
}
