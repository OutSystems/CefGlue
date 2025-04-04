namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface for CefBrowserHost::GetNavigationEntries. The methods of
    /// this class will be called on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefNavigationEntryVisitor
    {
        private int visit(cef_navigation_entry_visitor_t* self, cef_navigation_entry_t* entry, int current, int index, int total)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefNavigationEntryVisitor.Visit
        }
        
        /// <summary>
        /// Method that will be executed. Do not keep a reference to |entry| outside
        /// of this callback. Return true to continue visiting entries or false to
        /// stop. |current| is true if this entry is the currently loaded navigation
        /// entry. |index| is the 0-based index of this entry and |total| is the total
        /// number of entries.
        /// </summary>
        // protected abstract int Visit(cef_navigation_entry_t* entry, int current, int index, int total);
        
    }
}
