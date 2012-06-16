namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface to implement for handling DOM events. The methods of this class
    /// will be called on the render process main thread.
    /// </summary>
    public abstract unsafe partial class CefDomEventListener
    {
        private void handle_event(cef_domevent_listener_t* self, cef_domevent_t* @event)
        {
            CheckSelf(self);

            var m_event = CefDomEvent.FromNative(@event);

            HandleEvent(m_event);

            m_event.Dispose();
        }
        
        /// <summary>
        /// Called when an event is received. The event object passed to this method
        /// contains a snapshot of the DOM at the time this method is executed. DOM
        /// objects are only valid for the scope of this method. Do not keep references
        /// to or attempt to access any DOM objects outside the scope of this method.
        /// </summary>
        protected abstract void HandleEvent(CefDomEvent @event);
    }
}
