using System;

namespace Xilium.CefGlue.WindowsForms
{
	public class PluginCrashedEventArgs : EventArgs
	{
		public PluginCrashedEventArgs(string pluginPath)
		{
			PluginPath = pluginPath;
		}

		public string PluginPath { get; private set; }
	}
}
