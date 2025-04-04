namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to receive accessibility notification when
    /// accessibility events have been registered. The methods of this class will
    /// be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefAccessibilityHandler
    {
        private void on_accessibility_tree_change(cef_accessibility_handler_t* self, cef_value_t* value)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefAccessibilityHandler.OnAccessibilityTreeChange
        }
        
        /// <summary>
        /// Called after renderer process sends accessibility tree changes to the
        /// browser process.
        /// </summary>
        // protected abstract void OnAccessibilityTreeChange(cef_value_t* value);
        
        private void on_accessibility_location_change(cef_accessibility_handler_t* self, cef_value_t* value)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefAccessibilityHandler.OnAccessibilityLocationChange
        }
        
        /// <summary>
        /// Called after renderer process sends accessibility location changes to the
        /// browser process.
        /// </summary>
        // protected abstract void OnAccessibilityLocationChange(cef_value_t* value);
        
    }
}
