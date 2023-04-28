using System;
using Xilium.CefGlue.Common.Helpers;
using Xilium.CefGlue.Common.Helpers.Logger;
using Xilium.CefGlue.Common.InternalHandlers;
using Xilium.CefGlue.Common.Shared.Helpers;

namespace Xilium.CefGlue.Common
{
    internal sealed class CommonCefClient : CefClient
    {
        private readonly CefLifeSpanHandler _lifeSpanHandler;
        private readonly CefDisplayHandler _displayHandler;
        private readonly CefRenderHandler _renderHandler;
        private readonly CefLoadHandler _loadHandler;
        private readonly CefFrameHandler _frameHandler;
        private readonly CefContextMenuHandler _contextMenuHandler;
        private readonly ICefBrowserHost _owner;

        private readonly MessageDispatcher _messageDispatcher = new MessageDispatcher();

        public CommonCefClient(ICefBrowserHost owner, CefRenderHandler renderHandler, ILogger logger)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            _owner = owner;
            _renderHandler = renderHandler;
            _lifeSpanHandler = new CommonCefLifeSpanHandler(owner);
            _displayHandler = new CommonCefDisplayHandler(owner);
            _loadHandler = new CommonCefLoadHandler(owner);
            _frameHandler = new CommonCefFrameHandler(owner);

            _contextMenuHandler = new CommonCefContextMenuHandler(owner);
        }

        protected override CefContextMenuHandler GetContextMenuHandler()
        {
            return _contextMenuHandler;
        }

        protected override CefDialogHandler GetDialogHandler()
        {
            return _owner.DialogHandler;
        }

        protected override CefDownloadHandler GetDownloadHandler()
        {
            return _owner.DownloadHandler;
        }

        protected override CefDragHandler GetDragHandler()
        {
            return _owner.DragHandler;
        }

        protected override CefFindHandler GetFindHandler()
        {
            return _owner.FindHandler;
        }

        protected override CefFocusHandler GetFocusHandler()
        {
            return _owner.FocusHandler;
        }

        protected override CefKeyboardHandler GetKeyboardHandler()
        {
            return _owner.KeyboardHandler;
        }

        protected override CefRequestHandler GetRequestHandler()
        {
            return _owner.RequestHandler;
        }

        protected override CefJSDialogHandler GetJSDialogHandler()
        {
            return _owner.JSDialogHandler;
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

        protected override CefFrameHandler GetFrameHandler()
        {
            return _frameHandler;
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            try
            {
                using (message)
                _messageDispatcher.DispatchMessage(browser, frame, sourceProcess, message);
                return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
            }
            catch (Exception e)
            {
                _owner.HandleException(e);
                return false;
            }
        }

        public MessageDispatcher Dispatcher => _messageDispatcher;
    }
}
