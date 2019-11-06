namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal sealed class CefWebLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly CefWebBrowser _core;

        public CefWebLifeSpanHandler(CefWebBrowser core)
        {
            _core = core;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);

            _core.InvokeIfRequired(() => _core.OnBrowserAfterCreated(browser));
        }

        protected override bool DoClose(CefBrowser browser)
        {
            // TODO: ... dispose core
            return false;
        }

        protected override void OnBeforeClose(CefBrowser browser)
        {
            if (_core.InvokeRequired)
                _core.BeginInvoke((Action)_core.OnBeforeClose);
            else
                _core.OnBeforeClose();
        }

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
        {
            var e = new BeforePopupEventArgs(frame, targetUrl, targetFrameName, popupFeatures, windowInfo, client, settings,
                                 noJavascriptAccess);

            _core.InvokeIfRequired(() => _core.OnBeforePopup(e));

            client = e.Client;
            noJavascriptAccess = e.NoJavascriptAccess;

            return e.Handled;
        }
    }
}
