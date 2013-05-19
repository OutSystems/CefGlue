namespace Xilium.CefGlue.WindowsForms
{
	sealed class CefWebLoadHandler : CefLoadHandler
	{
		private readonly CefWebBrowser _core;

		public CefWebLoadHandler(CefWebBrowser core)
		{
			_core = core;
		}

		protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
		{
			_core.InvokeIfRequired(() => _core.OnLoadEnd(new LoadEndEventArgs(frame, httpStatusCode)));
		}

		protected override void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
		{
			_core.InvokeIfRequired(() => _core.OnLoadError(new LoadErrorEventArgs(frame, errorCode, errorText, failedUrl)));
		}

		protected override void OnLoadStart(CefBrowser browser, CefFrame frame)
		{
			_core.InvokeIfRequired(() => _core.OnLoadStart(new LoadStartEventArgs(frame)));
		}

		protected override void OnPluginCrashed(CefBrowser browser, string pluginPath)
		{
			_core.InvokeIfRequired(() => _core.OnPluginCrashed(new PluginCrashedEventArgs(pluginPath)));
		}

		protected override void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
		{
			_core.InvokeIfRequired(() => _core.OnRenderProcessTerminated(new RenderProcessTerminatedEventArgs(status)));
		}
	}
}