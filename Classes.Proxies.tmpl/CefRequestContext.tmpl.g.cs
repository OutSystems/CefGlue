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
        /// <summary>
        /// Returns the global context object.
        /// </summary>
        public static cef_request_context_t* GetGlobalContext()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetGlobalContext
        }
        
        /// <summary>
        /// Creates a new context object with the specified |settings| and optional
        /// |handler|.
        /// </summary>
        public static cef_request_context_t* CreateContext(cef_request_context_settings_t* settings, cef_request_context_handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.CreateContext
        }
        
        /// <summary>
        /// Creates a new context object that shares storage with |other| and uses an
        /// optional |handler|.
        /// </summary>
        public static cef_request_context_t* CreateContext(cef_request_context_t* other, cef_request_context_handler_t* handler)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.CreateContext
        }
        
        /// <summary>
        /// Returns true if this object is pointing to the same context as |that|
        /// object.
        /// </summary>
        public int IsSame(cef_request_context_t* other)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.IsSame
        }
        
        /// <summary>
        /// Returns true if this object is sharing the same storage as |that| object.
        /// </summary>
        public int IsSharingWith(cef_request_context_t* other)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.IsSharingWith
        }
        
        /// <summary>
        /// Returns true if this object is the global context. The global context is
        /// used by default when creating a browser or URL request with a NULL context
        /// argument.
        /// </summary>
        public int IsGlobal()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.IsGlobal
        }
        
        /// <summary>
        /// Returns the handler for this context if any.
        /// </summary>
        public cef_request_context_handler_t* GetHandler()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetHandler
        }
        
        /// <summary>
        /// Returns the cache path for this object. If empty an "incognito mode"
        /// in-memory cache is being used.
        /// </summary>
        public cef_string_userfree* GetCachePath()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetCachePath
        }
        
        /// <summary>
        /// Returns the cookie manager for this object. If |callback| is non-NULL it
        /// will be executed asnychronously on the UI thread after the manager's
        /// storage has been initialized.
        /// </summary>
        public cef_cookie_manager_t* GetCookieManager(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetCookieManager
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
        public int RegisterSchemeHandlerFactory(cef_string_t* scheme_name, cef_string_t* domain_name, cef_scheme_handler_factory_t* factory)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.RegisterSchemeHandlerFactory
        }
        
        /// <summary>
        /// Clear all registered scheme handler factories. Returns false on error.
        /// This function may be called on any thread in the browser process.
        /// </summary>
        public int ClearSchemeHandlerFactories()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.ClearSchemeHandlerFactories
        }
        
        /// <summary>
        /// Clears all certificate exceptions that were added as part of handling
        /// CefRequestHandler::OnCertificateError(). If you call this it is
        /// recommended that you also call CloseAllConnections() or you risk not
        /// being prompted again for server certificates if you reconnect quickly.
        /// If |callback| is non-NULL it will be executed on the UI thread after
        /// completion.
        /// </summary>
        public void ClearCertificateExceptions(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.ClearCertificateExceptions
        }
        
        /// <summary>
        /// Clears all HTTP authentication credentials that were added as part of
        /// handling GetAuthCredentials. If |callback| is non-NULL it will be executed
        /// on the UI thread after completion.
        /// </summary>
        public void ClearHttpAuthCredentials(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.ClearHttpAuthCredentials
        }
        
        /// <summary>
        /// Clears all active and idle connections that Chromium currently has.
        /// This is only recommended if you have released all other CEF objects but
        /// don't yet want to call CefShutdown(). If |callback| is non-NULL it will be
        /// executed on the UI thread after completion.
        /// </summary>
        public void CloseAllConnections(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.CloseAllConnections
        }
        
        /// <summary>
        /// Attempts to resolve |origin| to a list of associated IP addresses.
        /// |callback| will be executed on the UI thread after completion.
        /// </summary>
        public void ResolveHost(cef_string_t* origin, cef_resolve_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.ResolveHost
        }
        
        /// <summary>
        /// Returns the MediaRouter object associated with this context.  If
        /// |callback| is non-NULL it will be executed asnychronously on the UI thread
        /// after the manager's context has been initialized.
        /// </summary>
        public cef_media_router_t* GetMediaRouter(cef_completion_callback_t* callback)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetMediaRouter
        }
        
        /// <summary>
        /// Returns the current value for |content_type| that applies for the
        /// specified URLs. If both URLs are empty the default value will be returned.
        /// Returns nullptr if no value is configured. Must be called on the browser
        /// process UI thread.
        /// </summary>
        public cef_value_t* GetWebsiteSetting(cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetWebsiteSetting
        }
        
        /// <summary>
        /// Sets the current value for |content_type| for the specified URLs in the
        /// default scope. If both URLs are empty, and the context is not incognito,
        /// the default value will be set. Pass nullptr for |value| to remove the
        /// default value for this content type.
        /// WARNING: Incorrect usage of this method may cause instability or security
        /// issues in Chromium. Make sure that you first understand the potential
        /// impact of any changes to |content_type| by reviewing the related source
        /// code in Chromium. For example, if you plan to modify
        /// CEF_CONTENT_SETTING_TYPE_POPUPS, first review and understand the usage of
        /// ContentSettingsType::POPUPS in Chromium:
        /// https://source.chromium.org/search?q=ContentSettingsType::POPUPS
        /// </summary>
        public void SetWebsiteSetting(cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type, cef_value_t* value)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.SetWebsiteSetting
        }
        
        /// <summary>
        /// Returns the current value for |content_type| that applies for the
        /// specified URLs. If both URLs are empty the default value will be returned.
        /// Returns CEF_CONTENT_SETTING_VALUE_DEFAULT if no value is configured. Must
        /// be called on the browser process UI thread.
        /// </summary>
        public CefContentSettingValue GetContentSetting(cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetContentSetting
        }
        
        /// <summary>
        /// Sets the current value for |content_type| for the specified URLs in the
        /// default scope. If both URLs are empty, and the context is not incognito,
        /// the default value will be set. Pass CEF_CONTENT_SETTING_VALUE_DEFAULT for
        /// |value| to use the default value for this content type.
        /// WARNING: Incorrect usage of this method may cause instability or security
        /// issues in Chromium. Make sure that you first understand the potential
        /// impact of any changes to |content_type| by reviewing the related source
        /// code in Chromium. For example, if you plan to modify
        /// CEF_CONTENT_SETTING_TYPE_POPUPS, first review and understand the usage of
        /// ContentSettingsType::POPUPS in Chromium:
        /// https://source.chromium.org/search?q=ContentSettingsType::POPUPS
        /// </summary>
        public void SetContentSetting(cef_string_t* requesting_url, cef_string_t* top_level_url, CefContentSettingType content_type, CefContentSettingValue value)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.SetContentSetting
        }
        
        /// <summary>
        /// Add an observer for content and website setting changes. The observer will
        /// remain registered until the returned Registration object is destroyed.
        /// This method must be called on the browser process UI thread.
        /// </summary>
        public cef_registration_t* AddSettingObserver(cef_setting_observer_t* observer)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.AddSettingObserver
        }
        
        /// <summary>
        /// Sets the Chrome color scheme for all browsers that share this request
        /// context. |variant| values of SYSTEM, LIGHT and DARK change the underlying
        /// color mode (e.g. light vs dark). Other |variant| values determine how
        /// |user_color| will be applied in the current color mode. If |user_color| is
        /// transparent (0) the default color will be used.
        /// </summary>
        public void SetChromeColorScheme(cef_color_variant_t variant, uint user_color)
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.SetChromeColorScheme
        }
        
        /// <summary>
        /// Returns the current Chrome color scheme mode (SYSTEM, LIGHT or DARK). Must
        /// be called on the browser process UI thread.
        /// </summary>
        public cef_color_variant_t GetChromeColorSchemeMode()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetChromeColorSchemeMode
        }
        
        /// <summary>
        /// Returns the current Chrome color scheme color, or transparent (0) for the
        /// default color. Must be called on the browser process UI thread.
        /// </summary>
        public uint GetChromeColorSchemeColor()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetChromeColorSchemeColor
        }
        
        /// <summary>
        /// Returns the current Chrome color scheme variant. Must be called on the
        /// browser process UI thread.
        /// </summary>
        public cef_color_variant_t GetChromeColorSchemeVariant()
        {
            throw new NotImplementedException(); // TODO: CefRequestContext.GetChromeColorSchemeVariant
        }
        
    }
}
