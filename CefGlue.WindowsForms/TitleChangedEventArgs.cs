using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xilium.CefGlue.WindowsForms
{
	public class TitleChangedEventArgs : EventArgs
	{
		public TitleChangedEventArgs(string title)
		{
			Title = title;
		}

		public string Title { get; private set; }
	}
}
