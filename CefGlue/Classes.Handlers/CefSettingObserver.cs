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
    public unsafe partial class CefSettingObserver
    {
        private void on_setting_changed(cef_setting_observer_t* self, cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type)
        {
            CheckSelf(self);

            var m_requesting_url = cef_string_t.ToString(requesting_url);
            var m_top_level_url = cef_string_t.ToString(top_level_url);
            OnSettingChanged(m_requesting_url, m_top_level_url, content_type);
        }

        /// <summary>
        /// Called when a content or website setting has changed. The new value can be
        /// retrieved using CefRequestContext::GetContentSetting or
        /// CefRequestContext::GetWebsiteSetting.
        /// </summary>
        internal virtual void OnSettingChanged(string requestingUrl, string topLeveUrl, CefContentSettingType contentType)
        {
        }
    }
}
