namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Represents the route between a media source and sink. Instances of this
    /// object are created via CefMediaRouter::CreateRoute and retrieved via
    /// CefMediaObserver::OnRoutes. Contains the status and metadata of a
    /// routing operation. The methods of this class may be called on any browser
    /// process thread unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefMediaRoute
    {
        /// <summary>
        /// Returns the ID for this route.
        /// </summary>
        public cef_string_userfree* GetId()
        {
            throw new NotImplementedException(); // TODO: CefMediaRoute.GetId
        }
        
        /// <summary>
        /// Returns the source associated with this route.
        /// </summary>
        public cef_media_source_t* GetSource()
        {
            throw new NotImplementedException(); // TODO: CefMediaRoute.GetSource
        }
        
        /// <summary>
        /// Returns the sink associated with this route.
        /// </summary>
        public cef_media_sink_t* GetSink()
        {
            throw new NotImplementedException(); // TODO: CefMediaRoute.GetSink
        }
        
        /// <summary>
        /// Send a message over this route. |message| will be copied if necessary.
        /// </summary>
        public void SendRouteMessage(void* message, UIntPtr message_size)
        {
            throw new NotImplementedException(); // TODO: CefMediaRoute.SendRouteMessage
        }
        
        /// <summary>
        /// Terminate this route. Will result in an asynchronous call to
        /// CefMediaObserver::OnRoutes on all registered observers.
        /// </summary>
        public void Terminate()
        {
            throw new NotImplementedException(); // TODO: CefMediaRoute.Terminate
        }
        
    }
}
