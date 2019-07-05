using System;

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

        bool HandleTooltip(string text);

        void HandleLoadStart(LoadStartEventArgs args);
        void HandleLoadEnd(LoadEndEventArgs args);
        void HandleLoadError(LoadErrorEventArgs args);
        void HandleLoadingStateChange(LoadingStateChangeEventArgs args);
    }
}
