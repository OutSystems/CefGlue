namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to browser load status. The
    /// methods of this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefLoadHandler
    {
        private void on_load_start(cef_load_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);

            OnLoadStart(m_browser, m_frame);
        }

        /// <summary>
        /// Called when the browser begins loading a frame. The |frame| value will
        /// never be empty -- call the IsMain() method to check if this frame is the
        /// main frame. Multiple frames may be loading at the same time. Sub-frames may
        /// start or continue loading after the main frame load has ended. This method
        /// may not be called for a particular frame if the load request for that frame
        /// fails.
        /// </summary>
        protected virtual void OnLoadStart(CefBrowser browser, CefFrame frame)
        {
        }


        private void on_load_end(cef_load_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, int httpStatusCode)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);

            OnLoadEnd(m_browser, m_frame, httpStatusCode);
        }

        /// <summary>
        /// Called when the browser is done loading a frame. The |frame| value will
        /// never be empty -- call the IsMain() method to check if this frame is the
        /// main frame. Multiple frames may be loading at the same time. Sub-frames may
        /// start or continue loading after the main frame load has ended. This method
        /// will always be called for all frames irrespective of whether the request
        /// completes successfully.
        /// </summary>
        protected virtual void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
        {
        }


        private void on_load_error(cef_load_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, CefErrorCode errorCode, cef_string_t* errorText, cef_string_t* failedUrl)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_errorText = cef_string_t.ToString(errorText);
            var m_failedUrl = cef_string_t.ToString(failedUrl);

            OnLoadError(m_browser, m_frame, errorCode, m_errorText, m_failedUrl);
        }

        /// <summary>
        /// Called when the browser fails to load a resource. |errorCode| is the error
        /// code number, |errorText| is the error text and and |failedUrl| is the URL
        /// that failed to load. See net\base\net_error_list.h for complete
        /// descriptions of the error codes.
        /// </summary>
        protected virtual void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
        }


        private void on_render_process_terminated(cef_load_handler_t* self, cef_browser_t* browser, CefTerminationStatus status)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            OnRenderProcessTerminated(m_browser, status);
        }

        /// <summary>
        /// Called when the render process terminates unexpectedly. |status| indicates
        /// how the process terminated.
        /// </summary>
        protected virtual void OnRenderProcessTerminated(CefBrowser browser, CefTerminationStatus status)
        {
        }


        private void on_plugin_crashed(cef_load_handler_t* self, cef_browser_t* browser, cef_string_t* plugin_path)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_plugin_path = cef_string_t.ToString(plugin_path);
            OnPluginCrashed(m_browser, m_plugin_path);
        }

        /// <summary>
        /// Called when a plugin has crashed. |plugin_path| is the path of the plugin
        /// that crashed.
        /// </summary>
        protected virtual void OnPluginCrashed(CefBrowser browser, string pluginPath)
        {
        }
    }
}
