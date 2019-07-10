using System;

namespace Xilium.CefGlue.Common.Events
{
    public class ConsoleMessageEventArgs : EventArgs
    {
		public ConsoleMessageEventArgs(CefLogSeverity level, string message, string source, int line)
		{
            Level = level;
            Message = message;
            Source = source;
            Line = line;
		}

        public CefLogSeverity Level { get; }

        public string Message { get; }

        public string Source { get; }

        public int Line { get; }

        public bool OutputToConsole { get; set; } = true;
    }
}
