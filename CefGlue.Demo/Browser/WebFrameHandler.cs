namespace Xilium.CefGlue.Demo.Browser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Xilium.CefGlue;

    internal sealed class WebFrameHandler : CefFrameHandler
    {
        private readonly WebBrowser _core;

        public WebFrameHandler(WebBrowser core)
        {
            _core = core;
        }

        protected override void OnFrameCreated(CefBrowser browser, CefFrame frame)
        {
            Debug.Print($"{nameof(WebFrameHandler)}::{nameof(OnFrameCreated)}: BID={browser.Identifier} FID={frame.Identifier}");
        }

        protected override void OnFrameAttached(CefBrowser browser, CefFrame frame, bool reattached)
        {
            Debug.Print($"{nameof(WebFrameHandler)}::{nameof(OnFrameAttached)}: BID={browser.Identifier} FID={frame.Identifier} Reattached={reattached}");
        }

        protected override void OnFrameDetached(CefBrowser browser, CefFrame frame)
        {
            Debug.Print($"{nameof(WebFrameHandler)}::{nameof(OnFrameDetached)}: BID={browser.Identifier} FID={frame.Identifier}");
        }

        protected override void OnMainFrameChanged(CefBrowser browser, CefFrame? oldFrame, CefFrame? newFrame)
        {
            Debug.Print($"{nameof(WebFrameHandler)}::{nameof(OnFrameDetached)}: BID={browser.Identifier} OFID={oldFrame?.Identifier} NFID={newFrame?.Identifier}");
        }
    }
}
