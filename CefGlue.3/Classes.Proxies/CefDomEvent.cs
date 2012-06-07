namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class used to represent a DOM event. The methods of this class should only
    /// be called on the render process main thread.
    /// </summary>
    public sealed unsafe partial class CefDomEvent
    {
        /// <summary>
        /// Returns the event type.
        /// </summary>
        public string EventType
        {
            get
            {
                var n_result = cef_domevent_t.get_type(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the event category.
        /// </summary>
        public CefDomEventCategory Category
        {
            get { return cef_domevent_t.get_category(_self); }
        }

        /// <summary>
        /// Returns the event processing phase.
        /// </summary>
        public CefDomEventPhase Phase
        {
            get { return cef_domevent_t.get_phase(_self); }
        }

        /// <summary>
        /// Returns true if the event can bubble up the tree.
        /// </summary>
        public bool CanBubble
        {
            get { return cef_domevent_t.can_bubble(_self) != 0; }
        }

        /// <summary>
        /// Returns true if the event can be canceled.
        /// </summary>
        public bool CanCancel
        {
            get { return cef_domevent_t.can_cancel(_self) != 0; }
        }

        /// <summary>
        /// Returns the document associated with this event.
        /// </summary>
        public CefDomDocument Document
        {
            get
            {
                return CefDomDocument.FromNative(
                    cef_domevent_t.get_document(_self)
                    );
            }
        }

        /// <summary>
        /// Returns the target of the event.
        /// </summary>
        public CefDomNode Target
        {
            get
            {
                return CefDomNode.FromNative(
                    cef_domevent_t.get_target(_self)
                    );
            }
        }

        /// <summary>
        /// Returns the current target of the event.
        /// </summary>
        public CefDomNode CurrentTarget
        {
            get
            {
                return CefDomNode.FromNative(
                    cef_domevent_t.get_current_target(_self)
                    );
            }
        }
    }
}
