namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to commands. The methods
    /// of this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefCommandHandler
    {
        private int on_chrome_command(cef_command_handler_t* self, cef_browser_t* browser, int command_id, CefWindowOpenDisposition disposition)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            return OnChromeCommand(m_browser, command_id, disposition) ? 1 : 0;
        }

        /// <summary>
        /// Called to execute a Chrome command triggered via menu selection or
        /// keyboard shortcut. Values for |command_id| can be found in the
        /// cef_command_ids.h file. |disposition| provides information about the
        /// intended command target. Return true if the command was handled or false
        /// for the default implementation. For context menu commands this will be
        /// called after CefContextMenuHandler::OnContextMenuCommand. Only used with
        /// the Chrome runtime.
        /// </summary>
        protected abstract bool OnChromeCommand(CefBrowser browser, int commandId, CefWindowOpenDisposition disposition);

        private int is_chrome_app_menu_item_visible(cef_command_handler_t* self, cef_browser_t* browser, int command_id)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var result = IsChromeAppMenuItemVisible(m_browser, command_id);
            return result ? 1 : 0;
        }

        /// <summary>
        /// Called to check if a Chrome app menu item should be visible. Values for
        /// |command_id| can be found in the cef_command_ids.h file. Only called for
        /// menu items that would be visible by default. Only used with the Chrome
        /// runtime.
        /// </summary>
        protected virtual bool IsChromeAppMenuItemVisible(CefBrowser browser, int commandId)
            => true;

        private int is_chrome_app_menu_item_enabled(cef_command_handler_t* self, cef_browser_t* browser, int command_id)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var result = IsChromeAppMenuItemEnabled(m_browser, command_id);
            return result ? 1 : 0;
        }

        /// <summary>
        /// Called to check if a Chrome app menu item should be enabled. Values for
        /// |command_id| can be found in the cef_command_ids.h file. Only called for
        /// menu items that would be enabled by default. Only used with the Chrome
        /// runtime.
        /// </summary>
        protected virtual bool IsChromeAppMenuItemEnabled(CefBrowser browser, int commandId)
            => true;

        private int is_chrome_page_action_icon_visible(cef_command_handler_t* self, CefChromePageActionIconType icon_type)
        {
            CheckSelf(self);

            var result = IsChromePageActionIconVisible(icon_type);
            return result ? 1 : 0;
        }

        /// <summary>
        /// Called during browser creation to check if a Chrome page action icon
        /// should be visible. Only called for icons that would be visible by default.
        /// Only used with the Chrome runtime.
        /// </summary>
        protected virtual bool IsChromePageActionIconVisible(CefChromePageActionIconType iconType)
            => true;

        private int is_chrome_toolbar_button_visible(cef_command_handler_t* self, CefChromeToolbarButtonType button_type)
        {
            CheckSelf(self);

            var result = IsChromeToolbarButtonVisible(button_type);
            return result ? 1 : 0;
        }

        /// <summary>
        /// Called during browser creation to check if a Chrome toolbar button
        /// should be visible. Only called for buttons that would be visible by
        /// default. Only used with the Chrome runtime.
        /// </summary>
        protected virtual bool IsChromeToolbarButtonVisible(CefChromeToolbarButtonType buttonType)
            => true;
    }
}
