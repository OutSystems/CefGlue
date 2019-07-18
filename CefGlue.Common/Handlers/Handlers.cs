namespace Xilium.CefGlue.Common.Handlers
{
    public class ContextMenuHandler : CefContextMenuHandler { }

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

    public class LifeSpanHandler : CefLifeSpanHandler {

        internal bool HandleDoClose(CefBrowser browser)
        {
            return DoClose(browser);
        }

        internal void HandleBeforeClose(CefBrowser browser)
        {
            OnBeforeClose(browser);
        }

        internal bool HandleBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref bool noJavascriptAccess)
        {
            return OnBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref noJavascriptAccess);
        }

        internal void HandleAfterCreated(CefBrowser browser)
        {
            OnAfterCreated(browser);
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
            HandleScrollOffsetChanged(browser, x, y);
        }

        internal bool? HandleStartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            return StartDragging(browser, dragData, allowedOps, x, y);
        }

        internal void HandleUpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            UpdateDragCursor(browser, operation);
        }
    }

    public class RenderProcessHandler : CefRenderProcessHandler { }

    public class RequestHandler : CefRequestHandler { }
}