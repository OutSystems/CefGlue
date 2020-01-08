using System;
using Xilium.CefGlue.Common.Helpers;

namespace Xilium.CefGlue.Common.Platform
{
    internal class BrowserOSRSurface : BaseBrowserSurface
    {
        private readonly BuiltInRenderHandler _renderHandler;

        private Func<CefRectangle> _getViewRectOverride;
        private CefRectangle _viewRect;

        public BrowserOSRSurface(BuiltInRenderHandler renderHandler)
        {
            _renderHandler = renderHandler;
        }

        public override void SetBrowserHost(CefBrowserHost browserHost)
        {
            base.SetBrowserHost(browserHost);
            if (_viewRect.Width > 0 && _viewRect.Height > 0)
            {
                browserHost.WasResized();
            }
        }

        public override void Hide()
        {
            _browserHost.WasHidden(true);
        }

        public override bool MoveAndResize(int x, int y, int width, int height)
        {
            if (_viewRect.Width == width && _viewRect.Height == height)
            {
                return false;
            }

            // in osr x and y are always 0
            _viewRect = new CefRectangle(0, 0, width, height);

            _renderHandler.Resize(width, height);

            _browserHost?.WasResized();

            return true;
        }

        public override void Show()
        {
            _browserHost.WasHidden(false);

            // workaround cef OSR bug (https://bitbucket.org/chromiumembedded/cef/issues/2483/osr-invalidate-does-not-generate-frame)
            // we notify browser of a resize and return height+1px on next GetViewRect call
            // then restore the original size back again
            CefRuntime.PostTask(CefThreadId.UI, new ActionTask(() =>
            {
                _getViewRectOverride = () =>
                {
                    _getViewRectOverride = null;
                    _browserHost?.WasResized();
                    return new CefRectangle(0, 0, _viewRect.Width, _viewRect.Height + 1);
                };

                _browserHost.WasResized();
            }));
        }

        public override CefRectangle GetViewRect() => _getViewRectOverride?.Invoke() ?? _viewRect;
    }
}
