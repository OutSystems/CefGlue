namespace Xilium.CefGlue.Common.Handlers
{
    public class BrowserProcessHandler : CefBrowserProcessHandler
    {
        internal void HandleBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            OnBeforeChildProcessLaunch(commandLine);
        }

        internal void HandleContextInitialized()
        {
            OnContextInitialized();
        }

        internal void HandleScheduleMessagePumpWork(long delayMs)
        {
            OnScheduleMessagePumpWork(delayMs);
        }
    }

    public class ContextMenuHandler : CefContextMenuHandler
    {

        internal void HandleBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            OnBeforeContextMenu(browser, frame, state, model);
        }

        internal bool HandleContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags)
        {
            return OnContextMenuCommand(browser, frame, state, commandId, eventFlags);
        }

        internal void HandleContextMenuDismissed(CefBrowser browser, CefFrame frame)
        {
            OnContextMenuDismissed(browser, frame);
        }

        internal bool HandleRunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
        {
            return RunContextMenu(browser, frame, parameters, model, callback);
        }
    }

    public class DialogHandler : CefDialogHandler { }

    public class DisplayHandler : CefDisplayHandler
    {
        internal void HandleAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            OnAddressChange(browser, frame, url);
        }

        internal void HandleTitleChange(CefBrowser browser, string title)
        {
            OnTitleChange(browser, title);
        }

        internal bool HandleTooltip(CefBrowser browser, string text)
        {
            return OnTooltip(browser, text); 
        }

        internal void HandleStatusMessage(CefBrowser browser, string value)
        {
            OnStatusMessage(browser, value);
        }

        internal bool HandleConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line)
        {
            return OnConsoleMessage(browser, level, message, source, line);
        }

        internal bool HandleAutoResize(CefBrowser browser, ref CefSize newSize)
        {
            return OnAutoResize(browser, ref newSize);
        }

        internal void HandleFaviconUrlChange(CefBrowser browser, string[] iconUrls)
        {
            OnFaviconUrlChange(browser, iconUrls); 
        }

        internal void HandleFullscreenModeChange(CefBrowser browser, bool fullscreen)
        {
            OnFullscreenModeChange(browser, fullscreen);
        }

        internal void HandleLoadingProgressChange(CefBrowser browser, double progress)
        {
            OnLoadingProgressChange(browser, progress);
        }
    }

    public class DownloadHandler : CefDownloadHandler { }

    public abstract class DragHandler : CefDragHandler { }

    public abstract class FindHandler : CefFindHandler { }

    public class FocusHandler : CefFocusHandler { }

    public abstract class JSDialogHandler : CefJSDialogHandler { }

    public class KeyboardHandler : CefKeyboardHandler { }

    public class LifeSpanHandler : CefLifeSpanHandler
    {

        internal bool HandleDoClose(CefBrowser browser)
        {
            return DoClose(browser);
        }

        internal void HandleBeforeClose(CefBrowser browser)
        {
            OnBeforeClose(browser);
        }

        internal bool HandleBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
        {
            return OnBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref extraInfo, ref noJavascriptAccess);
        }

        internal void HandleAfterCreated(CefBrowser browser)
        {
            OnAfterCreated(browser);
        }

        internal void HandleBeforeDevToolsPopup(CefBrowser browser, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool useDefaultWindow)
        {
            OnBeforeDevToolsPopup(browser, windowInfo, ref client, settings, ref extraInfo, ref useDefaultWindow);
        }
    }

    public abstract class RenderHandler : CefRenderHandler
    {
        internal CefAccessibilityHandler HandleGetAccessibilityHandler()
        {
            return GetAccessibilityHandler();
        }

        internal void HandleScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            OnScrollOffsetChanged(browser, x, y);
        }
    }

    public class RequestHandler : CefRequestHandler
    {
        protected override CefResourceRequestHandler GetResourceRequestHandler(CefBrowser browser, CefFrame frame, CefRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return null;
        }
    }
}
