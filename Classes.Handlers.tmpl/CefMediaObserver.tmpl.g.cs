namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implemented by the client to observe MediaRouter events and registered via
    /// CefMediaRouter::AddObserver. The methods of this class will be called on the
    /// browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefMediaObserver
    {
        private void on_sinks(cef_media_observer_t* self, UIntPtr sinksCount, cef_media_sink_t** sinks)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMediaObserver.OnSinks
        }
        
        /// <summary>
        /// The list of available media sinks has changed or
        /// CefMediaRouter::NotifyCurrentSinks was called.
        /// </summary>
        // protected abstract void OnSinks(UIntPtr sinksCount, cef_media_sink_t** sinks);
        
        private void on_routes(cef_media_observer_t* self, UIntPtr routesCount, cef_media_route_t** routes)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMediaObserver.OnRoutes
        }
        
        /// <summary>
        /// The list of available media routes has changed or
        /// CefMediaRouter::NotifyCurrentRoutes was called.
        /// </summary>
        // protected abstract void OnRoutes(UIntPtr routesCount, cef_media_route_t** routes);
        
        private void on_route_state_changed(cef_media_observer_t* self, cef_media_route_t* route, CefMediaRouteConnectionState state)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMediaObserver.OnRouteStateChanged
        }
        
        /// <summary>
        /// The connection state of |route| has changed.
        /// </summary>
        // protected abstract void OnRouteStateChanged(cef_media_route_t* route, CefMediaRouteConnectionState state);
        
        private void on_route_message_received(cef_media_observer_t* self, cef_media_route_t* route, void* message, UIntPtr message_size)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefMediaObserver.OnRouteMessageReceived
        }
        
        /// <summary>
        /// A message was received over |route|. |message| is only valid for
        /// the scope of this callback and should be copied if necessary.
        /// </summary>
        // protected abstract void OnRouteMessageReceived(cef_media_route_t* route, void* message, UIntPtr message_size);
        
    }
}
