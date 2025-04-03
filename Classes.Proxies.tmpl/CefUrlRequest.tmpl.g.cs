namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to make a URL request. URL requests are not associated with a
    /// browser instance so no CefClient callbacks will be executed. URL requests
    /// can be created on any valid CEF thread in either the browser or render
    /// process. Once created the methods of the URL request object must be accessed
    /// on the same thread that created it.
    /// </summary>
    public sealed unsafe partial class CefUrlRequest
    {
        /// <summary>
        /// Create a new URL request that is not associated with a specific browser or
        /// frame. Use CefFrame::CreateURLRequest instead if you want the request to
        /// have this association, in which case it may be handled differently (see
        /// documentation on that method). A request created with this method may only
        /// originate from the browser process, and will behave as follows:
        /// - It may be intercepted by the client via CefResourceRequestHandler or
        /// CefSchemeHandlerFactory.
        /// - POST data may only contain only a single element of type PDE_TYPE_FILE
        /// or PDE_TYPE_BYTES.
        /// - If |request_context| is empty the global request context will be used.
        /// The |request| object will be marked as read-only after calling this
        /// method.
        /// </summary>
        public static cef_urlrequest_t* Create(cef_request_t* request, cef_urlrequest_client_t* client, cef_request_context_t* request_context)
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.Create
        }
        
        /// <summary>
        /// Returns the request object used to create this URL request. The returned
        /// object is read-only and should not be modified.
        /// </summary>
        public cef_request_t* GetRequest()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.GetRequest
        }
        
        /// <summary>
        /// Returns the client.
        /// </summary>
        public cef_urlrequest_client_t* GetClient()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.GetClient
        }
        
        /// <summary>
        /// Returns the request status.
        /// </summary>
        public CefUrlRequestStatus GetRequestStatus()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.GetRequestStatus
        }
        
        /// <summary>
        /// Returns the request error if status is UR_CANCELED or UR_FAILED, or 0
        /// otherwise.
        /// </summary>
        public CefErrorCode GetRequestError()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.GetRequestError
        }
        
        /// <summary>
        /// Returns the response, or NULL if no response information is available.
        /// Response information will only be available after the upload has
        /// completed. The returned object is read-only and should not be modified.
        /// </summary>
        public cef_response_t* GetResponse()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.GetResponse
        }
        
        /// <summary>
        /// Returns true if the response body was served from the cache. This includes
        /// responses for which revalidation was required.
        /// </summary>
        public int ResponseWasCached()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.ResponseWasCached
        }
        
        /// <summary>
        /// Cancel the request.
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException(); // TODO: CefUrlRequest.Cancel
        }
        
    }
}
