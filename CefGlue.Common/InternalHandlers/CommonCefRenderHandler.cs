using System;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal sealed class CommonCefRenderHandler : CefRenderHandler
    {
        private readonly IOffscreenCefBrowserHost _owner;
        private readonly ILogger _logger;

        public CommonCefRenderHandler(IOffscreenCefBrowserHost owner, ILogger logger)
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
            _owner.GetScreenInfo(screenInfo);
            return true;
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
                _logger.Debug($"Type: {type} Buffer: {buffer.ToInt64()} Width: {width} Height: {height}");
                foreach (var rect in dirtyRects)
                {
                    _logger.Debug($"   DirtyRect: X={rect.X} Y={rect.Y} W={rect.Width} H={rect.Height}");
                }
            }
            _owner.HandleViewPaint(buffer, width, height, dirtyRects, type == CefPaintElementType.Popup);
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
            _owner.HandleStartDragging(browser, dragData, allowedOps, x, y);
            return true;
        }

        protected override void UpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
        {
            _owner.HandleUpdateDragCursor(browser, operation);
        }
    }
}
