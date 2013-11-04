using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xilium.CefGlue.WPF
{
    public class WpfCefLoadHandler : CefLoadHandler
    {
        private WpfCefBrowser _owner;

        public WpfCefLoadHandler(WpfCefBrowser owner)
        {
            this._owner = owner;
        }

        protected override void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
        {
            this._owner.OnLoadingStateChange(isLoading, canGoBack, canGoForward);
        }
    }
}
