namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class used for managing cookies. The methods of this class may be called on
    /// any thread unless otherwise indicated.
    /// </summary>
    public sealed unsafe partial class CefCookieManager
    {
        /// <summary>
        /// Returns the global cookie manager. By default data will be stored at
        /// CefSettings.cache_path if specified or in memory otherwise.
        /// </summary>
        public static CefCookieManager Global
        {
            get
            {
                return CefCookieManager.FromNative(
                    cef_cookie_manager_t.get_global_manager()
                    );
            }
        }

        /// <summary>
        /// Creates a new cookie manager. If |path| is empty data will be stored in
        /// memory only. Otherwise, data will be stored at the specified |path|. To
        /// persist session cookies (cookies without an expiry date or validity
        /// interval) set |persist_session_cookies| to true. Session cookies are
        /// generally intended to be transient and most Web browsers do not persist
        /// them. Returns NULL if creation fails.
        /// </summary>
        public static CefCookieManager Create(string path, bool persistSessionCookies)
        {
            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path != null ? path.Length : 0);

                return CefCookieManager.FromNativeOrNull(
                    cef_cookie_manager_t.create_manager(&n_path, persistSessionCookies ? 1 : 0)
                    );
            }
        }

        /// <summary>
        /// Set the schemes supported by this manager. By default only "http" and
        /// "https" schemes are supported. Must be called before any cookies are
        /// accessed.
        /// </summary>
        public void SetSupportedSchemes(string[] schemes)
        {
            var n_schemes = cef_string_list.From(schemes);
            cef_cookie_manager_t.set_supported_schemes(_self, n_schemes);
            libcef.string_list_free(n_schemes);
        }

        /// <summary>
        /// Visit all cookies. The returned cookies are ordered by longest path, then
        /// by earliest creation date. Returns false if cookies cannot be accessed.
        /// </summary>
        public bool VisitAllCookies(CefCookieVisitor visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return cef_cookie_manager_t.visit_all_cookies(_self, visitor.ToNative()) != 0;
        }

        /// <summary>
        /// Visit a subset of cookies. The results are filtered by the given url
        /// scheme, host, domain and path. If |includeHttpOnly| is true HTTP-only
        /// cookies will also be included in the results. The returned cookies are
        /// ordered by longest path, then by earliest creation date. Returns false if
        /// cookies cannot be accessed.
        /// </summary>
        public bool VisitUrlCookies(string url, bool includeHttpOnly, CefCookieVisitor visitor)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (visitor == null) throw new ArgumentNullException("visitor");

            fixed (char* url_str = url)
            {
                var n_url = new cef_string_t(url_str, url.Length);

                return cef_cookie_manager_t.visit_url_cookies(_self, &n_url, includeHttpOnly ? 1 : 0, visitor.ToNative()) != 0;
            }
        }

        /// <summary>
        /// Sets a cookie given a valid URL and explicit user-provided cookie
        /// attributes. This function expects each attribute to be well-formed. It will
        /// check for disallowed characters (e.g. the ';' character is disallowed
        /// within the cookie value attribute) and will return false without setting
        /// the cookie if such characters are found. This method must be called on the
        /// IO thread.
        /// </summary>
        public bool SetCookie(string url, CefCookie cookie)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException("url");
            if (cookie == null) throw new ArgumentNullException("cookie");

            int n_result;
            var n_cookie = cookie.ToNative();
            fixed (char* url_str = url)
            {
                var n_url = new cef_string_t(url_str, url.Length);
                n_result = cef_cookie_manager_t.set_cookie(_self, &n_url, n_cookie);
            }
            CefCookie.Free(n_cookie);
            return n_result != 0;
        }

        /// <summary>
        /// Delete all cookies that match the specified parameters. If both |url| and
        /// values |cookie_name| are specified all host and domain cookies matching
        /// both will be deleted. If only |url| is specified all host cookies (but not
        /// domain cookies) irrespective of path will be deleted. If |url| is empty all
        /// cookies for all hosts and domains will be deleted. Returns false if a non-
        /// empty invalid URL is specified or if cookies cannot be accessed. This
        /// method must be called on the IO thread.
        /// </summary>
        public bool DeleteCookies(string url, string cookieName)
        {
            fixed (char* url_str = url)
            fixed (char* cookieName_str = cookieName)
            {
                var n_url = new cef_string_t(url_str, url != null ? url.Length : 0);
                var n_cookieName = new cef_string_t(cookieName_str, cookieName != null ? cookieName.Length : 0);

                return cef_cookie_manager_t.delete_cookies(_self, &n_url, &n_cookieName) != 0;
            }
        }

        /// <summary>
        /// Sets the directory path that will be used for storing cookie data. If
        /// |path| is empty data will be stored in memory only. Otherwise, data will be
        /// stored at the specified |path|. To persist session cookies (cookies without
        /// an expiry date or validity interval) set |persist_session_cookies| to true.
        /// Session cookies are generally intended to be transient and most Web browsers
        /// do not persist them. Returns false if cookies cannot be accessed.
        /// </summary>
        public bool SetStoragePath(string path, bool persistSessionCookies)
        {
            fixed (char* path_str = path)
            {
                var n_path = new cef_string_t(path_str, path != null ? path.Length : 0);

                return cef_cookie_manager_t.set_storage_path(_self, &n_path, persistSessionCookies ? 1 : 0) != 0;
            }
        }

        /// <summary>
        /// Flush the backing store (if any) to disk and execute the specified
        /// |handler| on the IO thread when done. Returns false if cookies cannot be
        /// accessed.
        /// </summary>
        public bool FlushStore(CefCompletionHandler handler)
        {
            var n_handler = handler != null ? handler.ToNative() : null;

            return cef_cookie_manager_t.flush_store(_self, n_handler) != 0;
        }
    }
}
