namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle proxy resolution events.
    /// </summary>
    public abstract unsafe partial class CefProxyHandler
    {
        private void get_proxy_for_url(cef_proxy_handler_t* self, cef_string_t* url, cef_proxy_info_t* proxy_info)
        {
            CheckSelf(self);

            var m_url = cef_string_t.ToString(url);
            var m_proxy_info = new CefProxyInfo(proxy_info);

            GetProxyForUrl(m_url, m_proxy_info);

            m_proxy_info.Dispose();
        }

        /// <summary>
        /// Called to retrieve proxy information for the specified |url|.
        /// </summary>
        protected abstract void GetProxyForUrl(string url, CefProxyInfo proxyInfo);
    }
}
