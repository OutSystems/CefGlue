﻿namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// A request context provides request handling for a set of related browser
    /// or URL request objects. A request context can be specified when creating a
    /// new browser via the CefBrowserHost static factory methods or when creating a
    /// new URL request via the CefURLRequest static factory methods. Browser
    /// objects with different request contexts will never be hosted in the same
    /// render process. Browser objects with the same request context may or may not
    /// be hosted in the same render process depending on the process model. Browser
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private cef_request_context_t* GetSelf()
            => (cef_request_context_t*)_self;

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

            return cef_request_context_t.is_same(GetSelf(), other.ToNative()) != 0;
        }

        /// <summary>
        /// Returns true if this object is sharing the same storage as |that| object.
        /// </summary>
        public bool IsSharingWith(CefRequestContext other)
        {
            return cef_request_context_t.is_sharing_with(GetSelf(), other.ToNative()) != 0;
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
                return cef_request_context_t.is_global(GetSelf()) != 0;
            }
        }

        /// <summary>
        /// Returns the handler for this context if any.
        /// </summary>
        public CefRequestContextHandler GetHandler()
        {
            return CefRequestContextHandler.FromNativeOrNull(
                cef_request_context_t.get_handler(GetSelf())
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
                var n_result = cef_request_context_t.get_cache_path(GetSelf());
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Returns the cookie manager for this object. If |callback| is non-NULL it
        /// will be executed asnychronously on the UI thread after the manager's
        /// storage has been initialized.
        /// </summary>
        public CefCookieManager GetCookieManager(CefCompletionCallback callback)
        {
            var n_callback = callback != null ? callback.ToNative() : null;

            return CefCookieManager.FromNativeOrNull(
                cef_request_context_t.get_cookie_manager(GetSelf(), n_callback)
                );
        }

        /// <summary>
        /// Register a scheme handler factory for the specified |scheme_name| and
        /// optional |domain_name|. An empty |domain_name| value for a standard scheme
        /// will cause the factory to match all domain names. The |domain_name| value
        /// will be ignored for non-standard schemes. If |scheme_name| is a built-in
        /// scheme and no handler is returned by |factory| then the built-in scheme
        /// handler factory will be called. If |scheme_name| is a custom scheme then
        /// you must also implement the CefApp::OnRegisterCustomSchemes() method in
        /// all processes. This function may be called multiple times to change or
        /// remove the factory that matches the specified |scheme_name| and optional
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

                return cef_request_context_t.register_scheme_handler_factory(GetSelf(), &n_schemeName, &n_domainName, factory.ToNative()) != 0;
            }
        }

        /// <summary>
        /// Clear all registered scheme handler factories. Returns false on error.
        /// This function may be called on any thread in the browser process.
        /// </summary>
        public bool ClearSchemeHandlerFactories()
        {
            return cef_request_context_t.clear_scheme_handler_factories(GetSelf()) != 0;
        }

        /// <summary>
        /// Clears all certificate exceptions that were added as part of handling
        /// CefRequestHandler::OnCertificateError(). If you call this it is
        /// recommended that you also call CloseAllConnections() or you risk not
        /// being prompted again for server certificates if you reconnect quickly.
        /// If |callback| is non-NULL it will be executed on the UI thread after
        /// completion.
        /// </summary>
        public void ClearCertificateExceptions(CefCompletionCallback callback)
        {
            var n_callback = callback != null ? callback.ToNative() : null;
            cef_request_context_t.clear_certificate_exceptions(GetSelf(), n_callback);
        }

        /// <summary>
        /// Clears all HTTP authentication credentials that were added as part of
        /// handling GetAuthCredentials. If |callback| is non-NULL it will be executed
        /// on the UI thread after completion.
        /// </summary>
        public void ClearHttpAuthCredentials(CefCompletionCallback callback)
        {
            var n_callback = callback != null ? callback.ToNative() : null;
            cef_request_context_t.clear_http_auth_credentials(GetSelf(), n_callback);
        }

        /// <summary>
        /// Clears all active and idle connections that Chromium currently has.
        /// This is only recommended if you have released all other CEF objects but
        /// don't yet want to call CefShutdown(). If |callback| is non-NULL it will be
        /// executed on the UI thread after completion.
        /// </summary>
        public void CloseAllConnections(CefCompletionCallback callback)
        {
            var n_callback = callback != null ? callback.ToNative() : null;
            cef_request_context_t.close_all_connections(GetSelf(), n_callback);
        }

        /// <summary>
        /// Attempts to resolve |origin| to a list of associated IP addresses.
        /// |callback| will be executed on the UI thread after completion.
        /// </summary>
        public void ResolveHost(string origin, CefResolveCallback callback)
        {
            if (string.IsNullOrEmpty(origin)) throw new ArgumentNullException("origin");
            if (callback == null) throw new ArgumentNullException("callback");

            fixed (char* origin_str = origin)
            {
                var n_origin = new cef_string_t(origin_str, origin != null ? origin.Length : 0);
                var n_callback = callback.ToNative();
                cef_request_context_t.resolve_host(GetSelf(), &n_origin, n_callback);
            }
        }

        /// <summary>
        /// Returns the MediaRouter object associated with this context.  If
        /// |callback| is non-NULL it will be executed asnychronously on the UI thread
        /// after the manager's context has been initialized.
        /// </summary>
        public CefMediaRouter GetMediaRouter(CefCompletionCallback? callback)
        {
            var nCallback = callback != null ? callback.ToNative() : null;
            var nResult = cef_request_context_t.get_media_router(GetSelf(), nCallback);
            return CefMediaRouter.FromNative(nResult);
        }

        /// <summary>
        /// Returns the current value for |content_type| that applies for the
        /// specified URLs. If both URLs are NULL the default value will be returned.
        /// Returns nullptr if no value is configured. Must be called on the browser
        /// process UI thread.
        /// </summary>
        public CefValue GetWebsiteSettings(
            string requestingUrl,
            string topLevelUrl,
            CefContentSettingType contentType)
        {
            fixed (char* requestingUrl_str = requestingUrl)
            fixed (char* topLevelUrl_str = topLevelUrl)
            {
                var n_requestingUrl = new cef_string_t(requestingUrl_str, requestingUrl != null ? requestingUrl.Length : 0);
                var n_topLevelUrl = new cef_string_t(topLevelUrl_str, topLevelUrl != null ? topLevelUrl.Length : 0);

                var n_result = cef_request_context_t.get_website_setting(GetSelf(), &n_requestingUrl, &n_topLevelUrl, contentType);

                return CefValue.FromNativeOrNull(n_result);
            }
        }

        /// <summary>
        /// Sets the current value for |content_type| for the specified URLs in the
        /// default scope. If both URLs are NULL, and the context is not incognito,
        /// the default value will be set. Pass nullptr for |value| to remove the
        /// default value for this content type.
        ///
        /// WARNING: Incorrect usage of this function may cause instability or
        /// security issues in Chromium. Make sure that you first understand the
        /// potential impact of any changes to |content_type| by reviewing the related
        /// source code in Chromium. For example, if you plan to modify
        /// CEF_CONTENT_SETTING_TYPE_POPUPS, first review and understand the usage of
        /// ContentSettingsType::POPUPS in Chromium:
        /// https://source.chromium.org/search?q=ContentSettingsType::POPUPS
        /// </summary>
        public void SetWebsiteSettings(
                 string requestingUrl,
                string topLevelUrl, 
                CefContentSettingType contentType, 
                CefValue value)
        {
            fixed (char* requestingUrl_str = requestingUrl)
            fixed (char* topLevelUrl_str = topLevelUrl)
            {
                var n_requestingUrl = new cef_string_t(requestingUrl_str, requestingUrl != null ? requestingUrl.Length : 0);
                var n_topLevelUrl = new cef_string_t(topLevelUrl_str, topLevelUrl != null ? topLevelUrl.Length : 0);
                var n_value = value.ToNative();

                cef_request_context_t.set_website_setting(GetSelf(), &n_requestingUrl, &n_topLevelUrl, contentType, n_value);
            }
        }

        /// <summary>
        /// Returns the current value for |content_type| that applies for the
        /// specified URLs. If both URLs are NULL the default value will be returned.
        /// Returns nullptr if no value is configured. Must be called on the browser
        /// process UI thread.
        /// </summary>
        public CefContentSettingValue GetContentSetting(
            string requestingUrl,
            string topLevelUrl,
            CefContentSettingType contentType)
        {
            fixed (char* requestingUrl_str = requestingUrl)
            fixed (char* topLevelUrl_str = topLevelUrl)
            {
                var n_requestingUrl = new cef_string_t(requestingUrl_str, requestingUrl != null ? requestingUrl.Length : 0);
                var n_topLevelUrl = new cef_string_t(topLevelUrl_str, topLevelUrl != null ? topLevelUrl.Length : 0);

                return cef_request_context_t.get_content_setting(GetSelf(), &n_requestingUrl, &n_topLevelUrl, contentType);
            }
        }

        /// <summary>
        /// Sets the current value for |content_type| for the specified URLs in the
        /// default scope. If both URLs are NULL, and the context is not incognito,
        /// the default value will be set. Pass CEF_CONTENT_SETTING_VALUE_DEFAULT for
        /// |value| to use the default value for this content type.
        ///
        /// WARNING: Incorrect usage of this function may cause instability or
        /// security issues in Chromium. Make sure that you first understand the
        /// potential impact of any changes to |content_type| by reviewing the related
        /// source code in Chromium. For example, if you plan to modify
        /// CEF_CONTENT_SETTING_TYPE_POPUPS, first review and understand the usage of
        /// ContentSettingsType::POPUPS in Chromium:
        /// https://source.chromium.org/search?q=ContentSettingsType::POPUPS
        /// </summary>
        public void setContentSetting(
                string requestingUrl,
                string topLevelUrl,
                CefContentSettingType contentType,
                CefContentSettingValue value)
        {
            fixed (char* requestingUrl_str = requestingUrl)
            fixed (char* topLevelUrl_str = topLevelUrl)
            {
                var n_requestingUrl = new cef_string_t(requestingUrl_str, requestingUrl != null ? requestingUrl.Length : 0);
                var n_topLevelUrl = new cef_string_t(topLevelUrl_str, topLevelUrl != null ? topLevelUrl.Length : 0);

                cef_request_context_t.set_content_setting(GetSelf(), &n_requestingUrl, &n_topLevelUrl, contentType, value);
            }
        }


    }
}
