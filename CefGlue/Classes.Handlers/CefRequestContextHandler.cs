namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to provide handler implementations.
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
        /// Called on the IO thread to retrieve the cookie manager. The global cookie
        /// manager will be used if this method returns NULL.
        /// </summary>
        protected abstract CefCookieManager GetCookieManager();
    }
}
