namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle events related to keyboard input. The
    /// methods of this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefKeyboardHandler
    {
        private int on_pre_key_event(cef_keyboard_handler_t* self, cef_browser_t* browser, cef_key_event_t* @event, IntPtr os_event, int* is_keyboard_shortcut)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefKeyboardHandler.OnPreKeyEvent
        }
        
        /// <summary>
        /// Called before a keyboard event is sent to the renderer. |event| contains
        /// information about the keyboard event. |os_event| is the operating system
        /// event message, if any. Return true if the event was handled or false
        /// otherwise. If the event will be handled in OnKeyEvent() as a keyboard
        /// shortcut set |is_keyboard_shortcut| to true and return false.
        /// </summary>
        // protected abstract int OnPreKeyEvent(cef_browser_t* browser, cef_key_event_t* @event, IntPtr os_event, int* is_keyboard_shortcut);
        
        private int on_key_event(cef_keyboard_handler_t* self, cef_browser_t* browser, cef_key_event_t* @event, IntPtr os_event)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefKeyboardHandler.OnKeyEvent
        }
        
        /// <summary>
        /// Called after the renderer and JavaScript in the page has had a chance to
        /// handle the event. |event| contains information about the keyboard event.
        /// |os_event| is the operating system event message, if any. Return true if
        /// the keyboard event was handled or false otherwise.
        /// </summary>
        // protected abstract int OnKeyEvent(cef_browser_t* browser, cef_key_event_t* @event, IntPtr os_event);
        
    }
}
