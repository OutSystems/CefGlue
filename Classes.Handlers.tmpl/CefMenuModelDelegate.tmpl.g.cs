namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle menu model events. The methods of this
    /// class will be called on the browser process UI thread unless otherwise
    /// indicated.
    /// </summary>
    public abstract unsafe partial class CefMenuModelDelegate
    {
        private void execute_command(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model, int command_id, CefEventFlags event_flags)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.ExecuteCommand
        }
        
        /// <summary>
        /// Perform the action associated with the specified |command_id| and
        /// optional |event_flags|.
        /// </summary>
        // protected abstract void ExecuteCommand(cef_menu_model_t* menu_model, int command_id, CefEventFlags event_flags);
        
        private void mouse_outside_menu(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model, cef_point_t* screen_point)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.MouseOutsideMenu
        }
        
        /// <summary>
        /// Called when the user moves the mouse outside the menu and over the owning
        /// window.
        /// </summary>
        // protected abstract void MouseOutsideMenu(cef_menu_model_t* menu_model, cef_point_t* screen_point);
        
        private void unhandled_open_submenu(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model, int is_rtl)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.UnhandledOpenSubmenu
        }
        
        /// <summary>
        /// Called on unhandled open submenu keyboard commands. |is_rtl| will be true
        /// if the menu is displaying a right-to-left language.
        /// </summary>
        // protected abstract void UnhandledOpenSubmenu(cef_menu_model_t* menu_model, int is_rtl);
        
        private void unhandled_close_submenu(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model, int is_rtl)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.UnhandledCloseSubmenu
        }
        
        /// <summary>
        /// Called on unhandled close submenu keyboard commands. |is_rtl| will be true
        /// if the menu is displaying a right-to-left language.
        /// </summary>
        // protected abstract void UnhandledCloseSubmenu(cef_menu_model_t* menu_model, int is_rtl);
        
        private void menu_will_show(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.MenuWillShow
        }
        
        /// <summary>
        /// The menu is about to show.
        /// </summary>
        // protected abstract void MenuWillShow(cef_menu_model_t* menu_model);
        
        private void menu_closed(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.MenuClosed
        }
        
        /// <summary>
        /// The menu has closed.
        /// </summary>
        // protected abstract void MenuClosed(cef_menu_model_t* menu_model);
        
        private int format_label(cef_menu_model_delegate_t* self, cef_menu_model_t* menu_model, cef_string_t* label)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMenuModelDelegate.FormatLabel
        }
        
        /// <summary>
        /// Optionally modify a menu item label. Return true if |label| was modified.
        /// </summary>
        // protected abstract int FormatLabel(cef_menu_model_t* menu_model, cef_string_t* label);
        
    }
}
