using System;

namespace Xilium.CefGlue.Common.Events
{
    public class LoadEndEventArgs : EventArgs
	{
		public LoadEndEventArgs(CefFrame frame, int httpStatusCode)
		{
			Frame = frame;
			HttpStatusCode = httpStatusCode;
		}

		public int HttpStatusCode { get; }

		public CefFrame Frame { get; }
	}
}
