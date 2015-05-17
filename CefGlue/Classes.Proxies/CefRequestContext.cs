namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// A request context provides request handling for a set of related browser
    /// or URL request objects. A request context can be specified when creating a
    /// new browser via the CefBrowserHost static factory methods or when creating a
    /// new URL request via the CefURLRequest static factory methods. Browser objects
    /// with different request contexts will never be hosted in the same render
    /// process. Browser objects with the same request context may or may not be
    /// hosted in the same render process depending on the process model. Browser
    /// objects created indirectly via the JavaScript window.open function or
    /// targeted links will share the same render process and the same request
    /// context as the source browser. When running in single-process mode there is
    /// only a single render process (the main process) and so all browsers created
    /// in single-process mode will share the same request context. This will be the
    /// first request context passed into a CefBrowserHost static factory method and
    /// all other request context objects will be ignored.
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
        /// Creates a new context object with the specified |settings| and optional
        /// |handler|.
        /// </summary>
        public static CefRequestContext CreateContext(CefRequestContextSettings settings, CefRequestContextHandler handler)
        {
            var n_settings = settings.ToNative();

            var result = CefRequestContext.FromNative(
                cef_request_context_t.create_context(
                    n_settings,
                    handler != null ? handler.ToNative() : null
                    )
                );

            CefRequestContextSettings.Free(n_settings);

            return result;
        }

        /// <summary>
        /// Creates a new context object that shares storage with |other| and uses an
        /// optional |handler|.
        /// </summary>
        public static CefRequestContext CreateContext(CefRequestContext other, CefRequestContextHandler handler)
        {
            return CefRequestContext.FromNative(
                cef_request_context_t.create_context(
                    other.ToNative(),
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
        /// Returns true if this object is sharing the same storage as |that| object.
        /// </summary>
        public bool IsSharingWith(CefRequestContext other)
        {
            return cef_request_context_t.is_sharing_with(_self, other.ToNative()) != 0;
        }

        /// <summary>
        /// Returns true if this object is the global context. The global context is
        /// used by default when creating a browser or URL request with a NULL context
        /// argument.
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

        /// <summary>
        /// Returns the cache path for this object. If empty an "incognito mode"
        /// in-memory cache is being used.
        /// </summary>
        public string CachePath
        {
            get
            {
                var n_result = cef_request_context_t.get_cache_path(_self);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the default cookie manager for this object. This will be the global
        /// cookie manager if this object is the global request context. Otherwise,
        /// this will be the default cookie manager used when this request context does
        /// not receive a value via CefRequestContextHandler::GetCookieManager(). If
        /// |callback| is non-NULL it will be executed asnychronously on the IO thread
        /// after the manager's storage has been initialized.
        /// </summary>
        public CefCookieManager GetDefaultCookieManager(CefCompletionCallback callback)
        {
            var n_callback = callback != null ? callback.ToNative() : null;

            return CefCookieManager.FromNativeOrNull(
                cef_request_context_t.get_default_cookie_manager(_self, n_callback)
                );
        }

        /// <summary>
        /// Register a scheme handler factory for the specified |scheme_name| and
        /// optional |domain_name|. An empty |domain_name| value for a standard scheme
        /// will cause the factory to match all domain names. The |domain_name| value
        /// will be ignored for non-standard schemes. If |scheme_name| is a built-in
        /// scheme and no handler is returned by |factory| then the built-in scheme
        /// handler factory will be called. If |scheme_name| is a custom scheme then
        /// you must also implement the CefApp::OnRegisterCustomSchemes() method in all
        /// processes. This function may be called multiple times to change or remove
        /// the factory that matches the specified |scheme_name| and optional
        /// |domain_name|. Returns false if an error occurs. This function may be
        /// called on any thread in the browser process.
        /// </summary>
        public bool RegisterSchemeHandlerFactory(string schemeName, string domainName, CefSchemeHandlerFactory factory)
        {
            if (string.IsNullOrEmpty(schemeName)) throw new ArgumentNullException("schemeName");
            if (factory == null) throw new ArgumentNullException("factory");

            fixed (char* schemeName_str = schemeName)
            fixed (char* domainName_str = domainName)
            {
                var n_schemeName = new cef_string_t(schemeName_str, schemeName.Length);
                var n_domainName = new cef_string_t(domainName_str, domainName != null ? domainName.Length : 0);

                return cef_request_context_t.register_scheme_handler_factory(_self, &n_schemeName, &n_domainName, factory.ToNative()) != 0;
            }
        }

        /// <summary>
        /// Clear all registered scheme handler factories. Returns false on error. This
        /// function may be called on any thread in the browser process.
        /// </summary>
        public bool ClearSchemeHandlerFactories()
        {
            return cef_request_context_t.clear_scheme_handler_factories(_self) != 0;
        }
    }
}
