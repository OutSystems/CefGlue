namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that creates CefResourceHandler instances for handling scheme
    /// requests. The methods of this class will always be called on the IO thread.
    /// </summary>
    public abstract unsafe partial class CefSchemeHandlerFactory
    {
        private cef_resource_handler_t* create(cef_scheme_handler_factory_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* scheme_name, cef_request_t* request)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefSchemeHandlerFactory.Create
        }
        
        /// <summary>
        /// Return a new resource handler instance to handle the request or an empty
        /// reference to allow default handling of the request. |browser| and |frame|
        /// will be the browser window and frame respectively that originated the
        /// request or NULL if the request did not originate from a browser window
        /// (for example, if the request came from CefURLRequest). The |request|
        /// object passed to this method cannot be modified.
        /// </summary>
        // protected abstract cef_resource_handler_t* Create(cef_browser_t* browser, cef_frame_t* frame, cef_string_t* scheme_name, cef_request_t* request);
        
    }
}
