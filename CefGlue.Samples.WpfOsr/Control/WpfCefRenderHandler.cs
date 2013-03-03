namespace Xilium.CefGlue.Samples.WpfOsr
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WpfCefRenderHandler : CefRenderHandler
    {
        private readonly WpfCefBrowser _owner;

        public WpfCefRenderHandler(WpfCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return _owner.GetViewRect(ref rect);
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            return _owner.GetViewRect(ref rect);
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            _owner.GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
            return true;
        }

        protected override void OnPopupShow(CefBrowser browser, bool show)
        {
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            //Debug.WriteLine("Type: {0} Buffer: {1:X8} Width: {2} Height: {3}", type, buffer, width, height);
            //foreach (var rect in dirtyRects)
            //{
            //    Debug.WriteLine("   DirtyRect: X={0} Y={1} W={2} H={3}", rect.X, rect.Y, rect.Width, rect.Height);
            //}

            if (type == CefPaintElementType.View)
                _owner.HandleViewPaint(browser, type, dirtyRects, buffer, width, height);
            // 			else if (type == CefPaintElementType.Popup)
            // 				this.browser.HandleWidgetPaint(browser, type, dirtyRects, buffer, width, height);
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle)
        {
        }
    }
}
