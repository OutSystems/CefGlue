using System;
using System.Diagnostics;
using NLog;

namespace Xilium.CefGlue.Common.Helpers.Logger
{
    internal interface ILogInitializer
    {
        NLog.Logger CreateLogger();
    }

    public class Logger : ILogger
    {
        private readonly object _syncObject = new object();
        private readonly ILogInitializer _logInitializer;
        private volatile NLog.Logger _log = null;

        private class CurrentLogInitilizer : ILogInitializer
        {
            public NLog.Logger CreateLogger()
            {
                return GetCurrentLogger();
            }
        }

        private class NamedLogInitilizer : ILogInitializer
        {
            private readonly string _loggerName;

            internal NamedLogInitilizer(string loggerName)
            {
                _loggerName = loggerName;
            }

            public NLog.Logger CreateLogger()
            {
                return LogManager.GetLogger(_loggerName);
            }
        }

        private class TypeLogInitilizer : ILogInitializer
        {
            private readonly Type _type;

            public TypeLogInitilizer(Type type)
            {
                _type = type;
            }

            public NLog.Logger CreateLogger()
            {
                return LogManager.GetCurrentClassLogger(_type);
            }
        }

        private NLog.Logger Log
        {
            get
            {
                lock (_syncObject)
                {
                    if (_log == null)
                    {
                        _log = _logInitializer.CreateLogger();
                    }

                    return _log;
                }
            }
        }

        private Logger(ILogInitializer logInitializer)
        {
            _logInitializer = logInitializer;
        }

        public Logger()
            : this(new CurrentLogInitilizer())
        {
        }

        private Logger(NLog.Logger log)
        {
            _log = log;
            _logInitializer = null;
        }

        public Logger(string loggerName)
            : this(new NamedLogInitilizer(loggerName))
        {
        }

        public Logger(Type type)
            : this(new TypeLogInitilizer(type))
        {
        }

        private static NLog.Logger GetCurrentLogger()
        {
            string loggerName;
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                var frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    loggerName = method.Name;
                    break;
                }

                framesToSkip++;
                loggerName = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return LogManager.GetLogger(loggerName);
        }

        public static ILogger GetLogger()
        {
            return new Logger(GetCurrentLogger());
        }

        public void TraceException(string message, Exception exception)
        {
            Log.Trace(exception, message);
        }

        public void Trace(string message)
        {
            Log.Trace(message);
        }

        public void DebugException(string message, Exception exception)
        {
            Log.Debug(exception, message);
        }

        public void Debug(string message)
        {
            Log.Debug(message);
        }

        public void ErrorException(string message, Exception exception)
        {
            Log.Error(exception, message);
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void FatalException(string message, Exception exception)
        {
            Log.Fatal(exception, message);
        }

        public void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public void InfoException(string message, Exception exception)
        {
            Log.Info(exception, message);
        }

        public void Info(string message)
        {
            Log.Info(message);
        }

        public void WarnException(string message, Exception exception)
        {
            Log.Warn(exception, message);
        }

        public void Warn(string message)
        {
            Log.Warn(message);
        }

        public bool IsTraceEnabled
        {
            get
            {
                return Log.IsTraceEnabled;
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return Log.IsDebugEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return Log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return Log.IsFatalEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return Log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return Log.IsWarnEnabled;
            }
        }
    }
}
