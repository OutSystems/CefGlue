namespace Xilium.CefGlue.WindowsForms
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    internal sealed class CefWebDisplayHandler : CefDisplayHandler
    {
        private readonly CefWebBrowser _core;

        public CefWebDisplayHandler(CefWebBrowser core)
        {
            _core = core;
        }

        protected override void  OnTitleChange(CefBrowser browser, string title)
        {
            _core.OnTitleChanged(title);
        }

        protected override void OnAddressChange(CefBrowser browser, CefFrame frame, string url)
        {
            if (frame.IsMain)
            {
                _core.OnAddressChanged(url);
            }
        }

        protected override void OnStatusMessage(CefBrowser browser, string value)
        {
            _core.OnStatusMessage(value);
        }
    }
}
