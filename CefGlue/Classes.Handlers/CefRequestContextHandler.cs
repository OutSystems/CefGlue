namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to provide handler implementations. The handler
    /// instance will not be released until all objects related to the context have
    /// been destroyed.
    /// </summary>
    public abstract unsafe partial class CefRequestContextHandler
    {
        private cef_cookie_manager_t* get_cookie_manager(cef_request_context_handler_t* self)
        {
            CheckSelf(self);

            var result = GetCookieManager();

            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Called on the IO thread to retrieve the cookie manager. If this method
        /// returns NULL the default cookie manager retrievable via
        /// CefRequestContext::GetDefaultCookieManager() will be used.
        /// </summary>
        protected abstract CefCookieManager GetCookieManager();
    }
}
