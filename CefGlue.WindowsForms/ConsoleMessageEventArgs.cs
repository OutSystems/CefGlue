using System;

namespace Xilium.CefGlue.WindowsForms
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

        public CefLogSeverity Level { get; private set; }
		public string Message { get; private set; }
		public string Source { get; private set; }
		public int Line { get; private set; }
		public bool Handled { get; set; }
	}
}
