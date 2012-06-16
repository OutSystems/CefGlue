namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class used to represent the browser process aspects of a browser window. The
    /// methods of this class can only be called in the browser process. They may be
    /// called on any thread in that process unless otherwise indicated in the
    /// comments.
    /// </summary>
    public sealed unsafe partial class CefBrowserHost
    {
        /// <summary>
        /// Create a new browser window using the window parameters specified by
        /// |windowInfo|. All values will be copied internally and the actual window
        /// will be created on the UI thread. This method can be called on any browser
        /// process thread and will not block.
        /// </summary>
        public static void CreateBrowser(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings, string url)
        {
            if (windowInfo == null) throw new ArgumentNullException("windowInfo");
            if (client == null) throw new ArgumentNullException("client");
            if (settings == null) throw new ArgumentNullException("settings");

            var n_windowInfo = windowInfo.ToNative();
            var n_client = client.ToNative();
            var n_settings = settings.ToNative();

            fixed (char* url_ptr = url)
            {
                cef_string_t n_url = new cef_string_t(url_ptr, url != null ? url.Length : 0);
                var n_success = cef_browser_host_t.create_browser(n_windowInfo, n_client, &n_url, n_settings);
                if (n_success != 1) throw ExceptionBuilder.FailedToCreateBrowser();
            }
        }

        public static void CreateBrowser(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings, Uri url)
        {
            CreateBrowser(windowInfo, client, settings, url.ToString());
        }

        public static void CreateBrowser(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings)
        {
            CreateBrowser(windowInfo, client, settings, string.Empty);
        }

        /// <summary>
        /// Create a new browser window using the window parameters specified by
        /// |windowInfo|. This method can only be called on the browser process UI
        /// thread.
        /// </summary>
        public static CefBrowser CreateBrowserSync(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings, string url)
        {
            if (windowInfo == null) throw new ArgumentNullException("windowInfo");
            if (client == null) throw new ArgumentNullException("client");
            if (settings == null) throw new ArgumentNullException("settings");

            var n_windowInfo = windowInfo.ToNative();
            var n_client = client.ToNative();
            var n_settings = settings.ToNative();

            fixed (char* url_ptr = url)
            {
                cef_string_t n_url = new cef_string_t(url_ptr, url != null ? url.Length : 0);
                var n_browser = cef_browser_host_t.create_browser_sync(n_windowInfo, n_client, &n_url, n_settings);
                return CefBrowser.FromNative(n_browser);
            }
        }

        public static CefBrowser CreateBrowserSync(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings, Uri url)
        {
            return CreateBrowserSync(windowInfo, client, settings, url.ToString());
        }

        public static CefBrowser CreateBrowserSync(CefWindowInfo windowInfo, CefClient client, CefBrowserSettings settings)
        {
            return CreateBrowserSync(windowInfo, client, settings, string.Empty);
        }

        /// <summary>
        /// Returns the hosted browser object.
        /// </summary>
        public CefBrowser GetBrowser()
        {
            return CefBrowser.FromNative(cef_browser_host_t.get_browser(_self));
        }

        /// <summary>
        /// Call this method before destroying a contained browser window. This method
        /// performs any internal cleanup that may be needed before the browser window
        /// is destroyed.
        /// </summary>
        public void ParentWindowWillClose()
        {
            cef_browser_host_t.parent_window_will_close(_self);
        }

        /// <summary>
        /// Closes this browser window.
        /// </summary>
        public void CloseBrowser()
        {
            cef_browser_host_t.close_browser(_self);
        }

        /// <summary>
        /// Set focus for the browser window. If |enable| is true focus will be set to
        /// the window. Otherwise, focus will be removed.
        /// </summary>
        public void SetFocus(bool enable)
        {
            cef_browser_host_t.set_focus(_self, enable ? 1 : 0);
        }

        /// <summary>
        /// Retrieve the window handle for this browser.
        /// </summary>
        public IntPtr GetWindowHandle()
        {
            return cef_browser_host_t.get_window_handle(_self);
        }

        /// <summary>
        /// Retrieve the window handle of the browser that opened this browser. Will
        /// return NULL for non-popup windows. This method can be used in combination
        /// with custom handling of modal windows.
        /// </summary>
        public IntPtr GetOpenerWindowHandle()
        {
            return cef_browser_host_t.get_opener_window_handle(_self);
        }

        /// <summary>
        /// Returns the client for this browser.
        /// </summary>
        public CefClient GetClient()
        {
            return CefClient.FromNative(
                cef_browser_host_t.get_client(_self)
                );
        }

        /// <summary>
        /// Returns the DevTools URL for this browser. If |http_scheme| is true the
        /// returned URL will use the http scheme instead of the chrome-devtools
        /// scheme. Remote debugging can be enabled by specifying the
        /// "remote-debugging-port" command-line flag or by setting the
        /// CefSettings.remote_debugging_port value. If remote debugging is not enabled
        /// this method will return an empty string.
        /// </summary>
        public string GetDevToolsUrl(bool httpScheme)
        {
            var n_url = cef_browser_host_t.get_dev_tools_url(_self, httpScheme ? 1 : 0);
            return cef_string_userfree.ToString(n_url);
        }

        /// <summary>
        /// Get the zoom level. This method can only be called on the UI thread.
        /// </summary>
        public double GetZoomLevel()
        {
            return cef_browser_host_t.get_zoom_level(_self);
        }

        /// <summary>
        /// Change the zoom level to the specified value.
        /// </summary>
        public void SetZoomLevel(double value)
        {
            cef_browser_host_t.set_zoom_level(_self, value);
        }
    }
}
