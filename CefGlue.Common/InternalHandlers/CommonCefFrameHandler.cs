using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonCefFrameHandler : CefFrameHandler
    {
        private readonly ICefBrowserHost _owner;

        public CommonCefFrameHandler(ICefBrowserHost owner)
        {
            _owner = owner;
        }

        protected override void OnFrameAttached(CefBrowser browser, CefFrame frame, bool reattached)
        {
            base.OnFrameAttached(browser, frame, reattached);
            _owner.HandleFrameAttached(browser, frame, reattached);
        }

        protected override void OnFrameDetached(CefBrowser browser, CefFrame frame)
        {
            base.OnFrameDetached(browser, frame);
            _owner.HandleFrameDetached(browser, frame);
        }
    }
}
