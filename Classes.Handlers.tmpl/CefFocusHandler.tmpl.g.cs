namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle events related to focus. The methods of
    /// this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefFocusHandler
    {
        private void on_take_focus(cef_focus_handler_t* self, cef_browser_t* browser, int next)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefFocusHandler.OnTakeFocus
        }
        
        /// <summary>
        /// Called when the browser component is about to loose focus. For instance,
        /// if focus was on the last HTML element and the user pressed the TAB key.
        /// |next| will be true if the browser is giving focus to the next component
        /// and false if the browser is giving focus to the previous component.
        /// </summary>
        // protected abstract void OnTakeFocus(cef_browser_t* browser, int next);
        
        private int on_set_focus(cef_focus_handler_t* self, cef_browser_t* browser, CefFocusSource source)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefFocusHandler.OnSetFocus
        }
        
        /// <summary>
        /// Called when the browser component is requesting focus. |source| indicates
        /// where the focus request is originating from. Return false to allow the
        /// focus to be set or true to cancel setting the focus.
        /// </summary>
        // protected abstract int OnSetFocus(cef_browser_t* browser, CefFocusSource source);
        
        private void on_got_focus(cef_focus_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefFocusHandler.OnGotFocus
        }
        
        /// <summary>
        /// Called when the browser component has received focus.
        /// </summary>
        // protected abstract void OnGotFocus(cef_browser_t* browser);
        
    }
}
