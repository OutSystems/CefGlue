using System;
using System.Collections.Generic;
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

        private readonly Dictionary<string, EventHandler<MessageReceivedEventArgs>> _messageHandlers = new Dictionary<string, EventHandler<MessageReceivedEventArgs>>();

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
            if (_messageHandlers.TryGetValue(message.Name, out var existingHandler))
            {
                existingHandler(this, new MessageReceivedEventArgs()
                {
                    Browser = browser,
                    ProcessId = sourceProcess,
                    Message = message
                });
            }

            return base.OnProcessMessageReceived(browser, sourceProcess, message);
        }

        public void RegisterMessageHandler(string messageName, EventHandler<MessageReceivedEventArgs> handler)
        {
            _messageHandlers.TryGetValue(messageName, out var existingHandler);
            _messageHandlers[messageName] = existingHandler + handler;
        }
    }
}
