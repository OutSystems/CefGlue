namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class that creates CefResourceHandler instances for handling scheme requests.
    /// The methods of this class will always be called on the IO thread.
    /// </summary>
    public abstract unsafe partial class CefSchemeHandlerFactory
    {
        private cef_resource_handler_t* create(cef_scheme_handler_factory_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* scheme_name, cef_request_t* request)
        {
            CheckSelf(self);

            var m_browser = browser != null ? CefBrowser.FromNative(browser) : null;
            var m_frame = frame != null ? CefFrame.FromNative(frame) : null;
            var m_schemeName = cef_string_t.ToString(scheme_name);
            var m_request = CefRequest.FromNative(request);

            var handler = Create(m_browser, m_frame, m_schemeName, m_request);

            return handler.ToNative();
        }

        /// <summary>
        /// Return a new resource handler instance to handle the request. |browser|
        /// will be the browser window that initiated the request. If the request was
        /// initiated using the CefWebURLRequest API |browser| will be NULL. The
        /// |request| object passed to this method will not contain cookie data.
        /// </summary>
        protected abstract CefResourceHandler Create(CefBrowser browser, CefFrame frame, string schemeName, CefRequest request);
    }
}
