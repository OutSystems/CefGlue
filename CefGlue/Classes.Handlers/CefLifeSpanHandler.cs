namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to browser life span. The
    /// methods of this class will be called on the UI thread unless otherwise
    /// indicated.
    /// </summary>
    public abstract unsafe partial class CefLifeSpanHandler
    {
        private int on_before_popup(cef_life_span_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, int popup_id, cef_string_t* target_url, cef_string_t* target_frame_name, CefWindowOpenDisposition target_disposition, int user_gesture, cef_popup_features_t* popupFeatures, cef_window_info_t* windowInfo, cef_client_t** client, cef_browser_settings_t* settings, cef_dictionary_value_t** extra_info, int* no_javascript_access)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_targetUrl = cef_string_t.ToString(target_url);
            var m_targetFrameName = cef_string_t.ToString(target_frame_name);
            var m_userGesture = user_gesture != 0;
            var m_popupFeatures = new CefPopupFeatures(popupFeatures);
            var m_windowInfo = CefWindowInfo.FromNative(windowInfo);
            var m_client = CefClient.FromNative(*client);
            var m_settings = new CefBrowserSettings(settings);
            var m_extraInfo = CefDictionaryValue.FromNativeOrNull(*extra_info);  // TODO dispose?
            var m_noJavascriptAccess = (*no_javascript_access) != 0;

            var o_extraInfo = m_extraInfo;
            var o_client = m_client;
            var result = OnBeforePopup(m_browser, m_frame, popup_id, m_targetUrl, m_targetFrameName, target_disposition, m_userGesture, m_popupFeatures, m_windowInfo, ref m_client, m_settings, ref m_extraInfo, ref m_noJavascriptAccess);

            if ((object)o_client != m_client && m_client != null)
            {
                *client = m_client.ToNative();
            }

            if ((object)o_extraInfo != m_extraInfo)
            {
                *extra_info = m_extraInfo != null ? m_extraInfo.ToNative() : null;
            }
            
            *no_javascript_access = m_noJavascriptAccess ? 1 : 0;

            m_popupFeatures.Dispose();
            m_windowInfo.Dispose();
            m_settings.Dispose();

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called on the UI thread before a new popup browser is created. The
        /// |browser| and |frame| values represent the source of the popup request
        /// (opener browser and frame). The |popup_id| value uniquely identifies the
        /// popup in the context of the opener browser. The |target_url| and
        /// |target_frame_name| values indicate where the popup browser should
        /// navigate and may be empty if not specified with the request. The
        /// |target_disposition| value indicates where the user intended to open the
        /// popup (e.g. current tab, new tab, etc). The |user_gesture| value will be
        /// true if the popup was opened via explicit user gesture (e.g. clicking a
        /// link) or false if the popup opened automatically (e.g. via the
        /// DomContentLoaded event). The |popupFeatures| structure contains additional
        /// information about the requested popup window. To allow creation of the
        /// popup browser optionally modify |windowInfo|, |client|, |settings| and
        /// |no_javascript_access| and return false. To cancel creation of the popup
        /// browser return true. The |client| and |settings| values will default to
        /// the source browser's values. If the |no_javascript_access| value is set to
        /// false the new browser will not be scriptable and may not be hosted in the
        /// same renderer process as the source browser. Any modifications to
        /// |windowInfo| will be ignored if the parent browser is wrapped in a
        /// CefBrowserView. The |extra_info| parameter provides an opportunity to
        /// specify extra information specific to the created popup browser that will
        /// be passed to CefRenderProcessHandler::OnBrowserCreated() in the render
        /// process.
        ///
        /// If popup browser creation succeeds then OnAfterCreated will be called for
        /// the new popup browser. If popup browser creation fails, and if the opener
        /// browser has not yet been destroyed, then OnBeforePopupAborted will be
        /// called for the opener browser. See OnBeforePopupAborted documentation for
        /// additional details.
        /// </summary>
        protected virtual bool OnBeforePopup(CefBrowser browser, CefFrame frame, int popupId, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
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
        /// Called after a new browser is created. It is now safe to begin performing
        /// actions with |browser|. CefFrameHandler callbacks related to initial main
        /// frame creation will arrive before this callback. See CefFrameHandler
        /// documentation for additional usage information.
        /// </summary>
        protected virtual void OnAfterCreated(CefBrowser browser)
        {
        }

        private void on_before_popup_aborted(cef_life_span_handler_t* self, cef_browser_t* browser, int popup_id)
        {
            OnBeforePopupAborted(CefBrowser.FromNative(browser),popup_id);
        }

        /// <summary>
        /// Called on the UI thread if a new popup browser is aborted. This only
        /// occurs if the popup is allowed in OnBeforePopup and creation fails before
        /// OnAfterCreated is called for the new popup browser. The |browser| value is
        /// the source of the popup request (opener browser). The |popup_id| value
        /// uniquely identifies the popup in the context of the opener browser, and is
        /// the same value that was passed to OnBeforePopup.
        ///
        /// Any client state associated with pending popups should be cleared in
        /// OnBeforePopupAborted, OnAfterCreated of the popup browser, or
        /// OnBeforeClose of the opener browser. OnBeforeClose of the opener browser
        /// may be called before this method in cases where the opener is closing
        /// during popup creation, in which case CefBrowserHost::IsValid will return
        /// false in this method.
        /// </summary>
        protected virtual void OnBeforePopupAborted(CefBrowser browser, int popup_id)
        {
        }

        private void on_before_dev_tools_popup(cef_life_span_handler_t* self, cef_browser_t* browser, cef_window_info_t* windowInfo, cef_client_t** client, cef_browser_settings_t* settings, cef_dictionary_value_t** extra_info, int* use_default_window)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_windowInfo = CefWindowInfo.FromNative(windowInfo);
            var m_client = CefClient.FromNative(*client);
            var m_settings = new CefBrowserSettings(settings);
            var m_extraInfo = CefDictionaryValue.FromNativeOrNull(*extra_info);  // TODO dispose?
            var m_useDefaultWindow = (*use_default_window) != 0;

            var o_extraInfo = m_extraInfo;
            var o_client = m_client;

            OnBeforeDevToolsPopup(m_browser, m_windowInfo, ref m_client, m_settings, ref m_extraInfo, ref m_useDefaultWindow);

            if ((object)o_client != m_client && m_client != null)
            {
                *client = m_client.ToNative();
            }

            if ((object)o_extraInfo != m_extraInfo)
            {
                *extra_info = m_extraInfo != null ? m_extraInfo.ToNative() : null;
            }
            
            *use_default_window = m_useDefaultWindow ? 1 : 0;

            m_windowInfo.Dispose();
            m_settings.Dispose();
        }

        /// <summary>
        /// Called on the UI thread before a new DevTools popup browser is created.
        /// The |browser| value represents the source of the popup request. Optionally
        /// modify |windowInfo|, |client|, |settings| and |extra_info| values. The
        /// |client|, |settings| and |extra_info| values will default to the source
        /// browser's values. Any modifications to |windowInfo| will be ignored if the
        /// parent browser is Views-hosted (wrapped in a CefBrowserView).
        ///
        /// The |extra_info| parameter provides an opportunity to specify extra
        /// information specific to the created popup browser that will be passed to
        /// CefRenderProcessHandler::OnBrowserCreated() in the render process. The
        /// existing |extra_info| object, if any, will be read-only but may be
        /// replaced with a new object.
        ///
        /// Views-hosted source browsers will create Views-hosted DevTools popups
        /// unless |use_default_window| is set to to true. DevTools popups can be
        /// blocked by returning true from CefCommandHandler::OnChromeCommand for
        /// IDC_DEV_TOOLS. Only used with Chrome style.
        /// </summary>
        protected virtual void OnBeforeDevToolsPopup(CefBrowser browser, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool useDefaultWindow)
        {
        }

        private int do_close(cef_life_span_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            return DoClose(m_browser) ? 1 : 0;
        }

        /// <summary>
        /// Called when an Alloy style browser is ready to be closed, meaning that the
        /// close has already been initiated and that JavaScript unload handlers have
        /// already executed or should be ignored. This may result directly from a
        /// call to CefBrowserHost::[Try]CloseBrowser() or indirectly if the browser's
        /// top-level parent window was created by CEF and the user attempts to
        /// close that window (by clicking the 'X', for example). DoClose() will not
        /// be called if the browser's host window/view has already been destroyed
        /// (via parent window/view hierarchy tear-down, for example), as it is no
        /// longer possible to customize the close behavior at that point.
        ///
        /// An application should handle top-level parent window close notifications
        /// by calling CefBrowserHost::TryCloseBrowser() or
        /// CefBrowserHost::CloseBrowser(false) instead of allowing the window to
        /// close immediately (see the examples below). This gives CEF an opportunity
        /// to process JavaScript unload handlers and optionally cancel the close
        /// before DoClose() is called.
        ///
        /// When windowed rendering is enabled CEF will create an internal child
        /// window/view to host the browser. In that case returning false from
        /// DoClose() will send the standard close notification to the browser's
        /// top-level parent window (e.g. WM_CLOSE on Windows, performClose: on OS X,
        /// "delete_event" on Linux or CefWindowDelegate::CanClose() callback from
        /// Views).
        ///
        /// When windowed rendering is disabled there is no internal window/view
        /// and returning false from DoClose() will cause the browser object to be
        /// destroyed immediately.
        ///
        /// If the browser's top-level parent window requires a non-standard close
        /// notification then send that notification from DoClose() and return true.
        /// You are still required to complete the browser close as soon as possible
        /// (either by calling [Try]CloseBrowser() or by proceeding with window/view
        /// hierarchy tear-down), otherwise the browser will be left in a partially
        /// closed state that interferes with proper functioning. Top-level windows
        /// created on the browser process UI thread can alternately call
        /// CefBrowserHost::IsReadyToBeClosed() in the close handler to check close
        /// status instead of relying on custom DoClose() handling. See documentation
        /// on that method for additional details.
        ///
        /// The CefLifeSpanHandler::OnBeforeClose() method will be called after
        /// DoClose() (if DoClose() is called) and immediately before the browser
        /// object is destroyed. The application should only exit after
        /// OnBeforeClose() has been called for all existing browsers.
        ///
        /// The below examples describe what should happen during window close when
        /// the browser is parented to an application-provided top-level window.
        ///
        /// Example 1: Using CefBrowserHost::TryCloseBrowser(). This is recommended
        /// for clients using standard close handling and windows created on the
        /// browser process UI thread.
        /// 1.  User clicks the window close button which sends a close notification
        ///     to the application's top-level window.
        /// 2.  Application's top-level window receives the close notification and
        ///     calls TryCloseBrowser() (similar to calling CloseBrowser(false)).
        ///     TryCloseBrowser() returns false so the client cancels the window
        ///     close.
        /// 3.  JavaScript 'onbeforeunload' handler executes and shows the close
        ///     confirmation dialog (which can be overridden via
        ///     CefJSDialogHandler::OnBeforeUnloadDialog()).
        /// 4.  User approves the close.
        /// 5.  JavaScript 'onunload' handler executes.
        /// 6.  Application's DoClose() handler is called and returns false by
        ///     default.
        /// 7.  CEF sends a close notification to the application's top-level window
        ///     (because DoClose() returned false).
        /// 8.  Application's top-level window receives the close notification and
        ///     calls TryCloseBrowser(). TryCloseBrowser() returns true so the client
        ///     allows the window close.
        /// 9.  Application's top-level window is destroyed, triggering destruction
        ///     of the child browser window.
        /// 10. Application's OnBeforeClose() handler is called and the browser object
        ///     is destroyed.
        /// 11. Application exits by calling CefQuitMessageLoop() if no other browsers
        ///     exist.
        ///
        /// Example 2: Using CefBrowserHost::CloseBrowser(false) and implementing the
        /// DoClose() callback. This is recommended for clients using non-standard
        /// close handling or windows that were not created on the browser process UI
        /// thread.
        /// 1.  User clicks the window close button which sends a close notification
        ///     to the application's top-level window.
        /// 2.  Application's top-level window receives the close notification and:
        ///     A. Calls CefBrowserHost::CloseBrowser(false).
        ///     B. Cancels the window close.
        /// 3.  JavaScript 'onbeforeunload' handler executes and shows the close
        ///     confirmation dialog (which can be overridden via
        ///     CefJSDialogHandler::OnBeforeUnloadDialog()).
        /// 4.  User approves the close.
        /// 5.  JavaScript 'onunload' handler executes.
        /// 6.  Application's DoClose() handler is called. Application will:
        ///     A. Set a flag to indicate that the next top-level window close attempt
        ///        will be allowed.
        ///     B. Return false.
        /// 7.  CEF sends a close notification to the application's top-level window
        ///     (because DoClose() returned false).
        /// 8.  Application's top-level window receives the close notification and
        ///     allows the window to close based on the flag from #6A.
        /// 9.  Application's top-level window is destroyed, triggering destruction
        ///     of the child browser window.
        /// 10. Application's OnBeforeClose() handler is called and the browser object
        ///     is destroyed.
        /// 11. Application exits by calling CefQuitMessageLoop() if no other browsers
        ///     exist.
        ///
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
        /// Called just before a browser is destroyed. Release all references to the
        /// browser object and do not attempt to execute any methods on the browser
        /// object (other than IsValid, GetIdentifier or IsSame) after this callback
        /// returns. CefFrameHandler callbacks related to final main frame
        /// destruction, and OnBeforePopupAborted callbacks for any pending popups,
        /// will arrive after this callback and CefBrowser::IsValid will return false
        /// at that time. Any in-progress network requests associated with |browser|
        /// will be aborted when the browser is destroyed, and
        /// CefResourceRequestHandler callbacks related to those requests may still
        /// arrive on the IO thread after this callback. See CefFrameHandler and
        /// DoClose() documentation for additional usage information.
        /// </summary>
        protected virtual void OnBeforeClose(CefBrowser browser)
        {
        }
    }
}
