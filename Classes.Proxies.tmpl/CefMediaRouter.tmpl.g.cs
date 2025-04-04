namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Supports discovery of and communication with media devices on the local
    /// network via the Cast and DIAL protocols. The methods of this class may be
    /// called on any browser process thread unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefMediaRouter
    {
        /// <summary>
        /// Returns the MediaRouter object associated with the global request context.
        /// If |callback| is non-NULL it will be executed asnychronously on the UI
        /// thread after the manager's storage has been initialized. Equivalent to
        /// calling CefRequestContext::GetGlobalContext()-&gt;GetMediaRouter().
        /// </summary>
        public static cef_media_router_t* GetGlobalMediaRouter(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.GetGlobalMediaRouter
        }
        
        /// <summary>
        /// Add an observer for MediaRouter events. The observer will remain
        /// registered until the returned Registration object is destroyed.
        /// </summary>
        public cef_registration_t* AddObserver(cef_media_observer_t* observer)
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.AddObserver
        }
        
        /// <summary>
        /// Returns a MediaSource object for the specified media source URN. Supported
        /// URN schemes include "cast:" and "dial:", and will be already known by the
        /// client application (e.g. "cast:&lt;appId&gt;?clientId=&lt;clientId&gt;").
        /// </summary>
        public cef_media_source_t* GetSource(cef_string_t* urn)
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.GetSource
        }
        
        /// <summary>
        /// Trigger an asynchronous call to CefMediaObserver::OnSinks on all
        /// registered observers.
        /// </summary>
        public void NotifyCurrentSinks()
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.NotifyCurrentSinks
        }
        
        /// <summary>
        /// Create a new route between |source| and |sink|. Source and sink must be
        /// valid, compatible (as reported by CefMediaSink::IsCompatibleWith), and a
        /// route between them must not already exist. |callback| will be executed
        /// on success or failure. If route creation succeeds it will also trigger an
        /// asynchronous call to CefMediaObserver::OnRoutes on all registered
        /// observers.
        /// </summary>
        public void CreateRoute(cef_media_source_t* source, cef_media_sink_t* sink, cef_media_route_create_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.CreateRoute
        }
        
        /// <summary>
        /// Trigger an asynchronous call to CefMediaObserver::OnRoutes on all
        /// registered observers.
        /// </summary>
        public void NotifyCurrentRoutes()
        {
            throw new NotImplementedException(); // TODO: CefMediaRouter.NotifyCurrentRoutes
        }
        
    }
}
