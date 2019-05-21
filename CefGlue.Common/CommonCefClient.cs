using System;
using Xilium.CefGlue.Common.Helpers.Logger;

namespace Xilium.CefGlue.Common
{
    internal sealed class CommonCefClient : CefClient
    {
        private readonly CommonCefLifeSpanHandler _lifeSpanHandler;
        private readonly CommonCefDisplayHandler _displayHandler;
        private readonly CommonCefRenderHandler _renderHandler;
        private readonly CommonCefLoadHandler _loadHandler;
        private readonly CommonCefJSDialogHandler _jsDialogHandler;

        public CommonCefClient(ICefBrowserHost owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            _lifeSpanHandler = new CommonCefLifeSpanHandler(owner);
            _displayHandler = new CommonCefDisplayHandler(owner);
            _renderHandler = new CommonCefRenderHandler(owner, new NLogLogger("CefRenderHandler"));
            _loadHandler = new CommonCefLoadHandler(owner);
            _jsDialogHandler = new CommonCefJSDialogHandler();
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override CefJSDialogHandler GetJSDialogHandler()
        {
            return _jsDialogHandler;
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs() {
                Browser = browser,
                ProcessId = sourceProcess,
                Message = message
            });

            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }
    }
}
