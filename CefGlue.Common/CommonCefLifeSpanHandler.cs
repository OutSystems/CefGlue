using System;

namespace Xilium.CefGlue.Common
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
    }
}
