namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle context menu events. The methods of this
    /// class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefContextMenuHandler
    {
        private void on_before_context_menu(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, cef_menu_model_t* model)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.OnBeforeContextMenu
        }
        
        /// <summary>
        /// Called before a context menu is displayed. |params| provides information
        /// about the context menu state. |model| initially contains the default
        /// context menu. The |model| can be cleared to show no context menu or
        /// modified to show a custom menu. Do not keep references to |params| or
        /// |model| outside of this callback.
        /// </summary>
        // protected abstract void OnBeforeContextMenu(cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, cef_menu_model_t* model);
        
        private int run_context_menu(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, cef_menu_model_t* model, cef_run_context_menu_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.RunContextMenu
        }
        
        /// <summary>
        /// Called to allow custom display of the context menu. |params| provides
        /// information about the context menu state. |model| contains the context
        /// menu model resulting from OnBeforeContextMenu. For custom display return
        /// true and execute |callback| either synchronously or asynchronously with
        /// the selected command ID. For default display return false. Do not keep
        /// references to |params| or |model| outside of this callback.
        /// </summary>
        // protected abstract int RunContextMenu(cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, cef_menu_model_t* model, cef_run_context_menu_callback_t* callback);
        
        private int on_context_menu_command(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, int command_id, CefEventFlags event_flags)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.OnContextMenuCommand
        }
        
        /// <summary>
        /// Called to execute a command selected from the context menu. Return true if
        /// the command was handled or false for the default implementation. See
        /// cef_menu_id_t for the command ids that have default implementations. All
        /// user-defined command ids should be between MENU_ID_USER_FIRST and
        /// MENU_ID_USER_LAST. |params| will have the same values as what was passed
        /// to OnBeforeContextMenu(). Do not keep a reference to |params| outside of
        /// this callback.
        /// </summary>
        // protected abstract int OnContextMenuCommand(cef_browser_t* browser, cef_frame_t* frame, cef_context_menu_params_t* @params, int command_id, CefEventFlags event_flags);
        
        private void on_context_menu_dismissed(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.OnContextMenuDismissed
        }
        
        /// <summary>
        /// Called when the context menu is dismissed irregardless of whether the menu
        /// was canceled or a command was selected.
        /// </summary>
        // protected abstract void OnContextMenuDismissed(cef_browser_t* browser, cef_frame_t* frame);
        
        private int run_quick_menu(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_point_t* location, cef_size_t* size, CefQuickMenuEditStateFlags edit_state_flags, cef_run_quick_menu_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.RunQuickMenu
        }
        
        /// <summary>
        /// Called to allow custom display of the quick menu for a windowless browser.
        /// |location| is the top left corner of the selected region. |size| is the
        /// size of the selected region. |edit_state_flags| is a combination of flags
        /// that represent the state of the quick menu. Return true if the menu will
        /// be handled and execute |callback| either synchronously or asynchronously
        /// with the selected command ID. Return false to cancel the menu.
        /// </summary>
        // protected abstract int RunQuickMenu(cef_browser_t* browser, cef_frame_t* frame, cef_point_t* location, cef_size_t* size, CefQuickMenuEditStateFlags edit_state_flags, cef_run_quick_menu_callback_t* callback);
        
        private int on_quick_menu_command(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, int command_id, CefEventFlags event_flags)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.OnQuickMenuCommand
        }
        
        /// <summary>
        /// Called to execute a command selected from the quick menu for a windowless
        /// browser. Return true if the command was handled or false for the default
        /// implementation. See cef_menu_id_t for command IDs that have default
        /// implementations.
        /// </summary>
        // protected abstract int OnQuickMenuCommand(cef_browser_t* browser, cef_frame_t* frame, int command_id, CefEventFlags event_flags);
        
        private void on_quick_menu_dismissed(cef_context_menu_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefContextMenuHandler.OnQuickMenuDismissed
        }
        
        /// <summary>
        /// Called when the quick menu for a windowless browser is dismissed
        /// irregardless of whether the menu was canceled or a command was selected.
        /// </summary>
        // protected abstract void OnQuickMenuDismissed(cef_browser_t* browser, cef_frame_t* frame);
        
    }
}
