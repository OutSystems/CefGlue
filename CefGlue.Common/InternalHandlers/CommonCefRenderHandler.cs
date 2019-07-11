using System;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal sealed class CommonCefRenderHandler : CefRenderHandler
    {
        private readonly ICefBrowserHost _owner;
        private readonly ILogger _logger;

        public CommonCefRenderHandler(ICefBrowserHost owner, ILogger logger)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _owner = owner;
            _logger = logger;
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            return _owner.RenderHandler?.HandleGetAccessibilityHandler();
        }

        protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
        {
            _owner.GetViewRect(out rect);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            _owner.GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupShow(CefBrowser browser, bool show)
        {
            _owner.HandlePopupShow(show);
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
            _owner.HandlePopupSizeChange(rect);
        }

        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Debug("Type: {0} Buffer: {1:X8} Width: {2} Height: {3}", type, buffer, width, height);
                foreach (var rect in dirtyRects)
                {
                    _logger.Debug("   DirtyRect: X={0} Y={1} W={2} H={3}", rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
            _owner.HandleViewPaint(buffer, width, height, dirtyRects, type == CefPaintElementType.Popup);
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            _owner.HandleCursorChange(cursorHandle);
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            _owner.RenderHandler?.HandleScrollOffsetChanged(browser, x, y);
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
        }

        protected override bool StartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
        {
            return (_owner.RenderHandler?.HandleStartDragging(browser, dragData, allowedOps, x, y) ?? false) || base.StartDragging(browser, dragData, allowedOps, x, y);
        }

        protected override void UpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            _owner.RenderHandler?.HandleUpdateDragCursor(browser, operation);
        }
    }
}
