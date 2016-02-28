using System;

namespace Xilium.CefGlue.WindowsForms
{
	public class LoadStartEventArgs : EventArgs
	{
		public LoadStartEventArgs(CefFrame frame)
		{
			Frame = frame;
		}

		public CefFrame Frame { get; private set; }
	}
}
