using System;

namespace Xilium.CefGlue.WindowsForms
{
	public class AddressChangedEventArgs : EventArgs
	{
		public AddressChangedEventArgs(CefFrame frame, string address)
		{
			Address = address;
			Frame = frame;
		}

		public string Address { get; private set; }

		public CefFrame Frame { get; private set; }
	}
}
