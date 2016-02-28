using System;

namespace Xilium.CefGlue.WindowsForms
{
	public class BeforePopupEventArgs : EventArgs
	{
		public BeforePopupEventArgs(
			CefFrame frame,
			string targetUrl,
			string targetFrameName,
			CefPopupFeatures popupFeatures,
			CefWindowInfo windowInfo,
			CefClient client,
			CefBrowserSettings settings,
			bool noJavascriptAccess)
		{
			Frame = frame;
			TargetUrl = targetUrl;
			TargetFrameName = targetFrameName;
			PopupFeatures = popupFeatures;
			WindowInfo = windowInfo;
			Client = client;
			Settings = settings;
			NoJavascriptAccess = noJavascriptAccess;
		}

		public bool NoJavascriptAccess { get; set; }

		public CefBrowserSettings Settings { get; private set; }

		public CefClient Client { get; set; }

		public CefWindowInfo WindowInfo { get; private set; }

		public CefPopupFeatures PopupFeatures { get; private set; }

		public string TargetFrameName { get; private set; }

		public string TargetUrl { get; private set; }

		public CefFrame Frame { get; private set; }

		public bool Handled { get; set; }
	}
}
