namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to browser requests. The
    /// methods of this class will be called on the thread indicated.
    /// </summary>
    public abstract unsafe partial class CefRequestHandler
    {
        private int on_before_resource_load(cef_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_request = CefRequest.FromNative(request);

            var result = OnBeforeResourceLoad(m_browser, m_frame, m_request);

            m_request.Dispose();

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on the IO thread before a resource request is loaded. The |request|
        /// object may be modified. To cancel the request return true otherwise return
        /// false.
        /// </summary>
        protected virtual bool OnBeforeResourceLoad(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            return false;
        }


        private cef_resource_handler_t* get_resource_handler(cef_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_request_t* request)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_request = CefRequest.FromNative(request);

            var handler = GetResourceHandler(m_browser, m_frame, m_request);

            m_request.Dispose();

            return handler != null ? handler.ToNative() : null;
        }

        /// <summary>
        /// Called on the IO thread before a resource is loaded. To allow the resource
        /// to load normally return NULL. To specify a handler for the resource return
        /// a CefResourceHandler object. The |request| object should not be modified in
        /// this callback.
        /// </summary>
        protected virtual CefResourceHandler GetResourceHandler(CefBrowser browser, CefFrame frame, CefRequest request)
        {
            return null;
        }


        private void on_resource_redirect(cef_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* old_url, cef_string_t* new_url)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_oldUrl = cef_string_t.ToString(old_url);
            var m_newUrl = cef_string_t.ToString(new_url);

            var o_newUrl = m_newUrl;
            OnResourceRedirect(m_browser, m_frame, m_oldUrl, ref m_newUrl);

            if ((object)m_newUrl != (object)o_newUrl)
            {
                cef_string_t.Copy(m_newUrl, new_url);
            }
        }

        /// <summary>
        /// Called on the IO thread when a resource load is redirected. The |old_url|
        /// parameter will contain the old URL. The |new_url| parameter will contain
        /// the new URL and can be changed if desired.
        /// </summary>
        protected virtual void OnResourceRedirect(CefBrowser browser, CefFrame frame, string oldUrl, ref string newUrl)
        {
        }


        private int get_auth_credentials(cef_request_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, int isProxy, cef_string_t* host, int port, cef_string_t* realm, cef_string_t* scheme, cef_auth_callback_t* callback)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_host = cef_string_t.ToString(host);
            var m_realm = cef_string_t.ToString(realm);
            var m_scheme = cef_string_t.ToString(scheme);
            var m_callback = CefAuthCallback.FromNative(callback);

            var result = GetAuthCredentials(m_browser, m_frame, isProxy != 0, m_host, port, m_realm, m_scheme, m_callback);

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on the IO thread when the browser needs credentials from the user.
        /// |isProxy| indicates whether the host is a proxy server. |host| contains the
        /// hostname and |port| contains the port number. Return true to continue the
        /// request and call CefAuthCallback::Continue() when the authentication
        /// information is available. Return false to cancel the request.
        /// </summary>
        protected virtual bool GetAuthCredentials(CefBrowser browser, CefFrame frame, bool isProxy, string host, int port, string realm, string scheme, CefAuthCallback callback)
        {
            return false;
        }


        private int on_quota_request(cef_request_handler_t* self, cef_browser_t* browser, cef_string_t* origin_url, long new_size, cef_quota_callback_t* callback)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_origin_url = cef_string_t.ToString(origin_url);
            var m_callback = CefQuotaCallback.FromNative(callback);

            var result = OnQuotaRequest(m_browser, m_origin_url, new_size, m_callback);

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on the IO thread when JavaScript requests a specific storage quota
        /// size via the webkitStorageInfo.requestQuota function. |origin_url| is the
        /// origin of the page making the request. |new_size| is the requested quota
        /// size in bytes. Return true and call CefQuotaCallback::Continue() either in
        /// this method or at a later time to grant or deny the request. Return false
        /// to cancel the request.
        /// </summary>
        protected abstract bool OnQuotaRequest(CefBrowser browser, string originUrl, long newSize, CefQuotaCallback callback);


        private cef_cookie_manager_t* get_cookie_manager(cef_request_handler_t* self, cef_browser_t* browser, cef_string_t* main_url)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_mainUrl = cef_string_t.ToString(main_url);

            var manager = GetCookieManager(m_browser, m_mainUrl);

            return manager != null ? manager.ToNative() : null;
        }

        /// <summary>
        /// Called on the IO thread to retrieve the cookie manager. |main_url| is the
        /// URL of the top-level frame. Cookies managers can be unique per browser or
        /// shared across multiple browsers. The global cookie manager will be used if
        /// this method returns NULL.
        /// </summary>
        protected virtual CefCookieManager GetCookieManager(CefBrowser browser, string mainUrl)
        {
            return null;
        }



        private void on_protocol_execution(cef_request_handler_t* self, cef_browser_t* browser, cef_string_t* url, int* allow_os_execution)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_url = cef_string_t.ToString(url);
            bool m_allow_os_execution;

            OnProtocolExecution(m_browser, m_url, out m_allow_os_execution);

            *allow_os_execution = m_allow_os_execution ? 1 : 0;
        }

        /// <summary>
        /// Called on the UI thread to handle requests for URLs with an unknown
        /// protocol component. Set |allow_os_execution| to true to attempt execution
        /// via the registered OS protocol handler, if any.
        /// SECURITY WARNING: YOU SHOULD USE THIS METHOD TO ENFORCE RESTRICTIONS BASED
        /// ON SCHEME, HOST OR OTHER URL ANALYSIS BEFORE ALLOWING OS EXECUTION.
        /// </summary>
        protected virtual void OnProtocolExecution(CefBrowser browser, string url, out bool allowOSExecution)
        {
            allowOSExecution = true;
        }


        private int on_before_plugin_load(cef_request_handler_t* self, cef_browser_t* browser, cef_string_t* url, cef_string_t* policy_url, cef_web_plugin_info_t* info)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_url = cef_string_t.ToString(url);
            var m_policy_url = cef_string_t.ToString(policy_url);
            var m_info = CefWebPluginInfo.FromNative(info);

            var result = OnBeforePluginLoad(m_browser, m_url, m_policy_url, m_info);

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on the browser process IO thread before a plugin is loaded. Return
        /// true to block loading of the plugin.
        /// </summary>
        protected virtual bool OnBeforePluginLoad(CefBrowser browser, string url, string policyUrl, CefWebPluginInfo info)
        {
            return false;
        }


        private int on_certificate_error(cef_request_handler_t* self, CefErrorCode cert_error, cef_string_t* request_url, cef_allow_certificate_error_callback_t* callback)
        {
            CheckSelf(self);

            var m_requestUrl = cef_string_t.ToString(request_url);
            var m_callback = CefAllowCertificateErrorCallback.FromNative(callback);

            var m_result = OnCertificateError(cert_error, m_requestUrl, m_callback);

            return m_result ? 1 : 0;
        }

        /// <summary>
        /// Called on the UI thread to handle requests for URLs with an invalid
        /// SSL certificate. Return true and call CefAllowCertificateErrorCallback::
        /// Continue() either in this method or at a later time to continue or cancel
        /// the request. Return false to cancel the request immediately. If |callback|
        /// is empty the error cannot be recovered from and the request will be
        /// canceled automatically. If CefSettings.ignore_certificate_errors is set
        /// all invalid certificates will be accepted without calling this method.
        /// </summary>
        protected virtual bool OnCertificateError(CefErrorCode certError, string requestUrl, CefAllowCertificateErrorCallback callback)
        {
            return false;
        }
    }
}
