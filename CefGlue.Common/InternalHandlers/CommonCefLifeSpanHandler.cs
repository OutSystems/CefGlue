using System;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal sealed class CommonCefLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly ICefBrowserHost _owner;

        public CommonCefLifeSpanHandler(ICefBrowserHost owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            _owner = owner;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            _owner.HandleBrowserCreated(browser);
        }

        protected override bool DoClose(CefBrowser browser)
        {
            return base.DoClose(browser);
        }

        protected override void OnBeforeClose(CefBrowser browser)
        {
            base.OnBeforeClose(browser);
        }

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref bool noJavascriptAccess)
        {
            return base.OnBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref noJavascriptAccess);
        }
    }
}
