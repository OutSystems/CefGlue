namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implemented by the client to observe content and website setting changes and
    /// registered via CefRequestContext::AddSettingObserver. The methods of this
    /// class will be called on the browser process UI thread.
    /// </summary>
    public sealed unsafe partial class CefSettingObserver
    {
        /// <summary>
        /// Called when a content or website setting has changed. The new value can be
        /// retrieved using CefRequestContext::GetContentSetting or
        /// CefRequestContext::GetWebsiteSetting.
        /// </summary>
        public void OnSettingChanged(cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type)
        {
            throw new NotImplementedException(); // TODO: CefSettingObserver.OnSettingChanged
        }
        
    }
}
