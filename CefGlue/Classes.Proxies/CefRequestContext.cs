namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// A request context provides request handling for a set of related browser
    /// objects. A request context is specified when creating a new browser object
    /// via the CefBrowserHost static factory methods. Browser objects with different
    /// request contexts will never be hosted in the same render process. Browser
    /// objects with the same request context may or may not be hosted in the same
    /// render process depending on the process model. Browser objects created
    /// indirectly via the JavaScript window.open function or targeted links will
    /// share the same render process and the same request context as the source
    /// browser. When running in single-process mode there is only a single render
    /// process (the main process) and so all browsers created in single-process mode
    /// will share the same request context. This will be the first request context
    /// passed into a CefBrowserHost static factory method and all other request
    /// context objects will be ignored.
    /// </summary>
    public sealed unsafe partial class CefRequestContext
    {
        /// <summary>
        /// Returns the global context object.
        /// </summary>
        public static CefRequestContext GetGlobalContext()
        {
            return CefRequestContext.FromNative(
                cef_request_context_t.get_global_context()
                );
        }

        /// <summary>
        /// Creates a new context object with the specified handler.
        /// </summary>
        public static CefRequestContext CreateContext(CefRequestContextHandler handler)
        {
            return CefRequestContext.FromNative(
                cef_request_context_t.create_context(
                    handler != null ? handler.ToNative() : null
                    )
                );
        }

        /// <summary>
        /// Returns true if this object is pointing to the same context as |that|
        /// object.
        /// </summary>
        public bool IsSame(CefRequestContext other)
        {
            if (other == null) return false;

            return cef_request_context_t.is_same(_self, other.ToNative()) != 0;
        }

        /// <summary>
        /// Returns true if this object is the global context.
        /// </summary>
        public bool IsGlobal
        {
            get
            {
                return cef_request_context_t.is_global(_self) != 0;
            }
        }

        /// <summary>
        /// Returns the handler for this context if any.
        /// </summary>
        public CefRequestContextHandler GetHandler()
        {
            return CefRequestContextHandler.FromNativeOrNull(
                cef_request_context_t.get_handler(_self)
                );
        }
    }
}
