using System;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal sealed class CommonCefDisplayHandler : CefDisplayHandler
    {
        private readonly ICefBrowserHost _owner;

        public CommonCefDisplayHandler(ICefBrowserHost owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            _owner.DisplayHandler?.HandleAddressChange(browser, frame, url);
            _owner.HandleAddressChange(browser, frame, url);
        }

        protected override void OnTitleChange(CefBrowser browser, string title)
        {
            _owner.DisplayHandler?.HandleTitleChange(browser, title);
            _owner.HandleTitleChange(browser, title);
        }

        protected override bool OnTooltip(CefBrowser browser, string text)
        {
            return (_owner.DisplayHandler?.HandleTooltip(browser, text) ?? false) || _owner.HandleTooltip(browser, text);
        }

        protected override void OnStatusMessage(CefBrowser browser, string value)
        {
            _owner.DisplayHandler?.HandleStatusMessage(browser, value);
            _owner.HandleStatusMessage(browser, value);
        }

        protected override bool OnConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            return (_owner.DisplayHandler?.HandleConsoleMessage(browser, level, message, source, line) ?? false) || _owner.HandleConsoleMessage(browser, level, message, source, line);
        }

        protected override bool OnAutoResize(CefBrowser browser, ref CefSize newSize)
        {
            return (_owner.DisplayHandler?.HandleAutoResize(browser, ref newSize) ?? false) || base.OnAutoResize(browser, ref newSize);
        }

        protected override void OnFaviconUrlChange(CefBrowser browser, string[] iconUrls)
        {
            _owner.DisplayHandler?.HandleFaviconUrlChange(browser, iconUrls);
        }

        protected override void OnFullscreenModeChange(CefBrowser browser, bool fullscreen)
        {
            _owner.DisplayHandler?.HandleFullscreenModeChange(browser, fullscreen);
        }

        protected override void OnLoadingProgressChange(CefBrowser browser, double progress)
        {
            _owner.DisplayHandler?.HandleLoadingProgressChange(browser, progress);
        }

        protected override bool OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            return _owner.HandleCursorChange(cursorHandle, type);
        }
    }
}
