using System;

namespace Xilium.CefGlue.Common.Platform
{
    internal class BrowserMacOSSurface : BaseBrowserSurface
    {
        private CefRectangle _viewRect;

        public override void Hide()
        {
        }

        public override bool MoveAndResize(int x, int y, int width, int height)
        {
            _viewRect = new CefRectangle(x, y, width, height);
            return true;
        }

        public override void Show()
        {
        }

        public override void SetBrowserHost(CefBrowserHost browserHost)
        {
            base.SetBrowserHost(browserHost);
        }

        public override CefRectangle GetViewRect() => _viewRect;
    }
}
