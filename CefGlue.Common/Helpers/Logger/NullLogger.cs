﻿using System;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Helpers.Log
{
    public sealed class NullLogger : ILogger
    {
        public NullLogger() { }

        public NullLogger(string loggerName) { }

        public bool IsTraceEnabled => false;

        public bool IsDebugEnabled => false;

        public bool IsErrorEnabled => false;

        public bool IsFatalEnabled => false;

        public bool IsInfoEnabled => false;

        public bool IsWarnEnabled => false;

        public void Debug(string message, params object[] args) { }
        public void Trace(string message) { }
        public void DebugException(string message, Exception exception) { }
        public void Debug(string message) { }
        public void Error(string message, params object[] args) { }
        public void ErrorException(string message, Exception exception) { }
        public void Error(string message) { }
        public void Fatal(string message, params object[] args) { }
        public void FatalException(string message, Exception exception) { }
        public void Fatal(string message) { }
        public void Info(string message, params object[] args) { }
        public void InfoException(string message, Exception exception) { }
        public void Info(string message) { }
        public void Trace(string message, params object[] args) { }
        public void TraceException(string message, Exception exception) { }
        public void Warn(string message) { }
        public void Warn(string message, params object[] args) { }
        public void WarnException(string message, Exception exception) { }
    }
}
