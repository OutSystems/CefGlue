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
            _owner.LifeSpanHandler?.HandleAfterCreated(browser);
            _owner.HandleBrowserCreated(browser);
        }

        protected override bool DoClose(CefBrowser browser)
        {
            return (_owner.LifeSpanHandler?.HandleDoClose(browser) ?? false) || 
                _owner.HandleBrowserClose(browser);
        }

        protected override void OnBeforeClose(CefBrowser browser)
        {
            _owner.LifeSpanHandler?.HandleBeforeClose(browser);
            _owner.HandleBrowserDestroyed(browser);
        }


        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
        {
            return (_owner.LifeSpanHandler?.HandleBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref extraInfo, ref noJavascriptAccess) ?? false) ||
                base.OnBeforePopup(browser, frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, ref client, settings, ref extraInfo, ref noJavascriptAccess);
        }

        protected override void OnBeforeDevToolsPopup(CefBrowser browser, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool useDefaultWindow)
        {
            _owner.LifeSpanHandler?.HandleBeforeDevToolsPopup(browser, windowInfo, ref client, settings, ref extraInfo, ref useDefaultWindow);
            base.OnBeforeDevToolsPopup(browser, windowInfo, ref client, settings, ref extraInfo, ref useDefaultWindow);
        }
    }
}
