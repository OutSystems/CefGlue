using System;

namespace Xilium.CefGlue.WindowsForms
{
	public class RenderProcessTerminatedEventArgs : EventArgs
	{
		public RenderProcessTerminatedEventArgs(CefTerminationStatus status)
		{
			Status = status;
		}

		public CefTerminationStatus Status { get; private set; }
	}
}
