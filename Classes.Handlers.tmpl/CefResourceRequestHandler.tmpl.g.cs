namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle events related to browser requests. The
    /// methods of this class will be called on the IO thread unless otherwise
    /// indicated.
    /// </summary>
    public abstract unsafe partial class CefResourceRequestHandler
    {
        private cef_cookie_access_filter_t* get_cookie_access_filter(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.GetCookieAccessFilter
        }
        
        /// <summary>
        /// Called on the IO thread before a resource request is loaded. The |browser|
        /// and |frame| values represent the source of the request, and may be NULL
        /// for requests originating from service workers or CefURLRequest. To
        /// optionally filter cookies for the request return a CefCookieAccessFilter
        /// object. The |request| object cannot not be modified in this callback.
        /// </summary>
        // protected abstract cef_cookie_access_filter_t* GetCookieAccessFilter(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request);
        
        private CefReturnValue on_before_resource_load(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.OnBeforeResourceLoad
        }
        
        /// <summary>
        /// Called on the IO thread before a resource request is loaded. The |browser|
        /// and |frame| values represent the source of the request, and may be NULL
        /// for requests originating from service workers or CefURLRequest. To
        /// redirect or change the resource load optionally modify |request|.
        /// Modification of the request URL will be treated as a redirect. Return
        /// RV_CONTINUE to continue the request immediately. Return RV_CONTINUE_ASYNC
        /// and call CefCallback methods at a later time to continue or cancel the
        /// request asynchronously. Return RV_CANCEL to cancel the request
        /// immediately.
        /// </summary>
        // protected abstract CefReturnValue OnBeforeResourceLoad(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_callback_t* callback);
        
        private cef_resource_handler_t* get_resource_handler(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.GetResourceHandler
        }
        
        /// <summary>
        /// Called on the IO thread before a resource is loaded. The |browser| and
        /// |frame| values represent the source of the request, and may be NULL for
        /// requests originating from service workers or CefURLRequest. To allow the
        /// resource to load using the default network loader return NULL. To specify
        /// a handler for the resource return a CefResourceHandler object. The
        /// |request| object cannot not be modified in this callback.
        /// </summary>
        // protected abstract cef_resource_handler_t* GetResourceHandler(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request);
        
        private void on_resource_redirect(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, cef_string_t* new_url)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.OnResourceRedirect
        }
        
        /// <summary>
        /// Called on the IO thread when a resource load is redirected. The |browser|
        /// and |frame| values represent the source of the request, and may be NULL
        /// for requests originating from service workers or CefURLRequest. The
        /// |request| parameter will contain the old URL and other request-related
        /// information. The |response| parameter will contain the response that
        /// resulted in the redirect. The |new_url| parameter will contain the new URL
        /// and can be changed if desired. The |request| and |response| objects cannot
        /// be modified in this callback.
        /// </summary>
        // protected abstract void OnResourceRedirect(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, cef_string_t* new_url);
        
        private int on_resource_response(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.OnResourceResponse
        }
        
        /// <summary>
        /// Called on the IO thread when a resource response is received. The
        /// |browser| and |frame| values represent the source of the request, and may
        /// be NULL for requests originating from service workers or CefURLRequest. To
        /// allow the resource load to proceed without modification return false. To
        /// redirect or retry the resource load optionally modify |request| and return
        /// true. Modification of the request URL will be treated as a redirect.
        /// Requests handled using the default network loader cannot be redirected in
        /// this callback. The |response| object cannot be modified in this callback.
        /// WARNING: Redirecting using this method is deprecated. Use
        /// OnBeforeResourceLoad or GetResourceHandler to perform redirects.
        /// </summary>
        // protected abstract int OnResourceResponse(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response);
        
        private cef_response_filter_t* get_resource_response_filter(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.GetResourceResponseFilter
        }
        
        /// <summary>
        /// Called on the IO thread to optionally filter resource response content.
        /// The |browser| and |frame| values represent the source of the request, and
        /// may be NULL for requests originating from service workers or
        /// CefURLRequest. |request| and |response| represent the request and response
        /// respectively and cannot be modified in this callback.
        /// </summary>
        // protected abstract cef_response_filter_t* GetResourceResponseFilter(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response);
        
        private void on_resource_load_complete(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, CefUrlRequestStatus status, long received_content_length)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.OnResourceLoadComplete
        }
        
        /// <summary>
        /// Called on the IO thread when a resource load has completed. The |browser|
        /// and |frame| values represent the source of the request, and may be NULL
        /// for requests originating from service workers or CefURLRequest. |request|
        /// and |response| represent the request and response respectively and cannot
        /// be modified in this callback. |status| indicates the load completion
        /// status. |received_content_length| is the number of response bytes actually
        /// read. This method will be called for all requests, including requests that
        /// are aborted due to CEF shutdown or destruction of the associated browser.
        /// In cases where the associated browser is destroyed this callback may
        /// arrive after the CefLifeSpanHandler::OnBeforeClose callback for that
        /// browser. The CefFrame::IsValid method can be used to test for this
        /// situation, and care should be taken not to call |browser| or |frame|
        /// methods that modify state (like LoadURL, SendProcessMessage, etc.) if the
        /// frame is invalid.
        /// </summary>
        // protected abstract void OnResourceLoadComplete(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, cef_response_t* response, CefUrlRequestStatus status, long received_content_length);
        
        private void on_protocol_execution(cef_resource_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, int* allow_os_execution)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceRequestHandler.OnProtocolExecution
        }
        
        /// <summary>
        /// Called on the IO thread to handle requests for URLs with an unknown
        /// protocol component. The |browser| and |frame| values represent the source
        /// of the request, and may be NULL for requests originating from service
        /// workers or CefURLRequest. |request| cannot be modified in this callback.
        /// Set |allow_os_execution| to true to attempt execution via the registered
        /// OS protocol handler, if any. SECURITY WARNING: YOU SHOULD USE THIS METHOD
        /// TO ENFORCE RESTRICTIONS BASED ON SCHEME, HOST OR OTHER URL ANALYSIS BEFORE
        /// ALLOWING OS EXECUTION.
        /// </summary>
        // protected abstract void OnProtocolExecution(cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request, int* allow_os_execution);
        
    }
}
