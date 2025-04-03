namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to filter cookies that may be sent or received from
    /// resource requests. The methods of this class will be called on the IO thread
    /// unless otherwise indicated.
    /// </summary>
    public abstract unsafe partial class CefCookieAccessFilter
    {
        private int can_send_cookie(cef_cookie_access_filter_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_cookie_t* cookie)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefCookieAccessFilter.CanSendCookie
        }
        
        /// <summary>
        /// Called on the IO thread before a resource request is sent. The |browser|
        /// and |frame| values represent the source of the request, and may be NULL
        /// for requests originating from service workers or CefURLRequest. |request|
        /// cannot be modified in this callback. Return true if the specified cookie
        /// can be sent with the request or false otherwise.
        /// </summary>
        // protected abstract int CanSendCookie(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_cookie_t* cookie);
        
        private int can_save_cookie(cef_cookie_access_filter_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, cef_cookie_t* cookie)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefCookieAccessFilter.CanSaveCookie
        }
        
        /// <summary>
        /// Called on the IO thread after a resource response is received. The
        /// |browser| and |frame| values represent the source of the request, and may
        /// be NULL for requests originating from service workers or CefURLRequest.
        /// |request| cannot be modified in this callback. Return true if the
        /// specified cookie returned with the response can be saved or false
        /// otherwise.
        /// </summary>
        // protected abstract int CanSaveCookie(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, cef_cookie_t* cookie);
        
    }
}
