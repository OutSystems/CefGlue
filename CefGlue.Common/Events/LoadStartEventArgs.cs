using System;

namespace Xilium.CefGlue.Common.Events
{
    public class LoadStartEventArgs : EventArgs
    {
		public LoadStartEventArgs(CefFrame frame)
		{
			Frame = frame;
		}

		public CefFrame Frame { get; }
    }
}
