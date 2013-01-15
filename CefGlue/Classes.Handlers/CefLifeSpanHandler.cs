namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to browser life span. The
    /// methods of this class will be called on the UI thread.
    /// </summary>
    public abstract unsafe partial class CefLifeSpanHandler
    {
        private int on_before_popup(cef_life_span_handler_t* self, cef_browser_t* parentBrowser, cef_popup_features_t* popupFeatures, cef_window_info_t* windowInfo, cef_string_t* url, cef_client_t** client, cef_browser_settings_t* settings)
        {
            CheckSelf(self);

            var m_parentBrowser = CefBrowser.FromNative(parentBrowser);
            var m_popupFeatures = new CefPopupFeatures(popupFeatures);
            var m_windowInfo = CefWindowInfo.FromNative(windowInfo);
            var m_url = cef_string_t.ToString(url);
            var m_client = CefClient.FromNative(*client);
            var m_settings = new CefBrowserSettings(settings);

            var o_client = m_client;
            var result = OnBeforePopup(m_parentBrowser, m_popupFeatures, m_windowInfo, m_url, ref m_client, m_settings);

            if ((object)o_client != m_client && m_client != null)
            {
                *client = m_client.ToNative();
            }

            m_popupFeatures.Dispose();
            m_windowInfo.Dispose();
            m_settings.Dispose();

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called before a new popup window is created. The |parentBrowser| parameter
        /// will point to the parent browser window. The |popupFeatures| parameter will
        /// contain information about the style of popup window requested. Return false
        /// to have the framework create the new popup window based on the parameters
        /// in |windowInfo|. Return true to cancel creation of the popup window. By
        /// default, a newly created popup window will have the same client and
        /// settings as the parent window. To change the client for the new window
        /// modify the object that |client| points to. To change the settings for the
        /// new window modify the |settings| structure.
        /// </summary>
        protected virtual bool OnBeforePopup(CefBrowser parentBrowser, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, string url, ref CefClient client, CefBrowserSettings settings)
        {
            return false;
        }


        private void on_after_created(cef_life_span_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            OnAfterCreated(m_browser);
        }

        /// <summary>
        /// Called after a new window is created.
        /// </summary>
        protected virtual void OnAfterCreated(CefBrowser browser)
        {
        }


        private int run_modal(cef_life_span_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            return RunModal(m_browser) ? 1 : 0;
        }

        /// <summary>
        /// Called when a modal window is about to display and the modal loop should
        /// begin running. Return false to use the default modal loop implementation or
        /// true to use a custom implementation.
        /// </summary>
        protected virtual bool RunModal(CefBrowser browser)
        {
            return false;
        }


        private int do_close(cef_life_span_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            return DoClose(m_browser) ? 1 : 0;
        }

        /// <summary>
        /// Called when a window has recieved a request to close. Return false to
        /// proceed with the window close or true to cancel the window close. If this
        /// is a modal window and a custom modal loop implementation was provided in
        /// RunModal() this callback should be used to restore the opener window to a
        /// usable state.
        /// </summary>
        protected virtual bool DoClose(CefBrowser browser)
        {
            return false;
        }


        private void on_before_close(cef_life_span_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            OnBeforeClose(m_browser);
        }

        /// <summary>
        /// Called just before a window is closed. If this is a modal window and a
        /// custom modal loop implementation was provided in RunModal() this callback
        /// should be used to exit the custom modal loop.
        /// </summary>
        protected virtual void OnBeforeClose(CefBrowser browser)
        {
        }
    }
}
