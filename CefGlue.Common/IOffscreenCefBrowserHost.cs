using System;

namespace Xilium.CefGlue.Common
{
    internal interface IOffscreenCefBrowserHost : ICefBrowserHost
    {
        void GetViewRect(out CefRectangle rect);
        void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY);
        void GetScreenInfo(CefScreenInfo screenInfo);

        void HandlePopupShow(bool show);
        void HandlePopupSizeChange(CefRectangle rect);

        void HandleViewPaint(IntPtr buffer, int width, int height, CefRectangle[] dirtyRects, bool isPopup);

        void HandleStartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y);
        void HandleUpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation);
    }
}
