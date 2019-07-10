using System;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common
{
    public interface ICefBrowserHost
    {
        void GetViewRect(out CefRectangle rect);
        void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY);

        void HandlePopupShow(bool show);
        void HandlePopupSizeChange(CefRectangle rect);

        void HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup);
        
        void HandleCursorChange(IntPtr cursorHandle);

        void HandleBrowserCreated(CefBrowser browser);


        void HandleAddressChange(CefBrowser browser, CefFrame frame, string url);
        void HandleTitleChange(CefBrowser browser, string title);
        bool HandleTooltip(CefBrowser browser, string text);
        void HandleStatusMessage(CefBrowser browser, string value);
        bool HandleConsoleMessage(CefBrowser browser, CefLogSeverity level, string message, string source, int line);

        void HandleLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType);
        void HandleLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode);
        void HandleLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl);
        void HandleLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward);
    }
}
