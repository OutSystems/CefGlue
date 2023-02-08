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

        protected override void OnFrameDetached(CefBrowser browser, CefFrame frame)
        {
            base.OnFrameDetached(browser, frame);
            _owner.HandleFrameDetached(browser, frame);
        }
    }
}
