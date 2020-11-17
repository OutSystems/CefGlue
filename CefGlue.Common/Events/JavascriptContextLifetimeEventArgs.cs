using System;

namespace Xilium.CefGlue.Common.Events
{
    public class JavascriptContextLifetimeEventArgs : EventArgs
	{
		public JavascriptContextLifetimeEventArgs(CefFrame frame)
		{
			Frame = frame;
		}

		public CefFrame Frame { get; }
	}
}
