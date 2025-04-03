namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implemented by the client to observe preference changes and registered via
    /// CefPreferenceManager::AddPreferenceObserver. The methods of this class will
    /// be called on the browser process UI thread.
    /// </summary>
    public sealed unsafe partial class CefPreferenceObserver
    {
        /// <summary>
        /// Called when a preference has changed. The new value can be retrieved using
        /// CefPreferenceManager::GetPreference.
        /// </summary>
        public void OnPreferenceChanged(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefPreferenceObserver.OnPreferenceChanged
        }
        
    }
}
