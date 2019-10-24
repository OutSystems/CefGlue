using Xilium.CefGlue.Common.Handlers;

namespace Xilium.CefGlue.Common.InternalHandlers
{
    internal class CommonCefContextMenuHandler : ContextMenuHandler
    {
        private readonly ICefBrowserHost _owner;

        public CommonCefContextMenuHandler(ICefBrowserHost owner)
        {
            _owner = owner;
        }

        protected override void OnBeforeContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams state, CefMenuModel model)
        {
            _owner.ContextMenuHandler?.HandleBeforeContextMenu(browser, frame, state, model);
            base.OnBeforeContextMenu(browser, frame, state, model);
        }

        protected override bool OnContextMenuCommand(CefBrowser browser, CefFrame frame, CefContextMenuParams state, int commandId, CefEventFlags eventFlags)
        {
            return _owner.ContextMenuHandler?.HandleContextMenuCommand(browser, frame, state, commandId, eventFlags) ??
                base.OnContextMenuCommand(browser, frame, state, commandId, eventFlags);
        }

        protected override void OnContextMenuDismissed(CefBrowser browser, CefFrame frame)
        {
            _owner.ContextMenuHandler?.HandleContextMenuDismissed(browser, frame);
            _owner.HandleCloseContextMenu();
        }

        protected override bool RunContextMenu(CefBrowser browser, CefFrame frame, CefContextMenuParams parameters, CefMenuModel model, CefRunContextMenuCallback callback)
        {
            var result = _owner.ContextMenuHandler?.HandleRunContextMenu(browser, frame, parameters, model, callback);
            if (result != null)
            {
                return result.Value;
            }

            _owner.HandleOpenContextMenu(parameters, model, callback);
            return true;
        }
    }
}
