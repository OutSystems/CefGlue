namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Implement this interface to handle events related to CefFrame life span. The
    /// order of callbacks is:
    ///
    /// (1) During initial CefBrowserHost creation and navigation of the main frame:
    /// - CefFrameHandler::OnFrameCreated => The initial main frame object has been
    ///   created. Any commands will be queued until the frame is attached.
    /// - CefFrameHandler::OnMainFrameChanged => The initial main frame object has
    ///   been assigned to the browser.
    /// - CefLifeSpanHandler::OnAfterCreated => The browser is now valid and can be
    ///   used.
    /// - CefFrameHandler::OnFrameAttached => The initial main frame object is now
    ///   connected to its peer in the renderer process. Commands can be routed.
    ///
    /// (2) During further CefBrowserHost navigation/loading of the main frame
    ///     and/or sub-frames:
    /// - CefFrameHandler::OnFrameCreated => A new main frame or sub-frame object
    ///   has been created. Any commands will be queued until the frame is attached.
    /// - CefFrameHandler::OnFrameAttached => A new main frame or sub-frame object
    ///   is now connected to its peer in the renderer process. Commands can be
    ///   routed.
    /// - CefFrameHandler::OnFrameDetached => An existing main frame or sub-frame
    ///   object has lost its connection to the renderer process. If multiple
    ///   objects are detached at the same time then notifications will be sent for
    ///   any sub-frame objects before the main frame object. Commands can no longer
    ///   be routed and will be discarded.
    /// - CefFremeHadler::OnFrameDestroyed => An existing main frame or sub-frame
    ///   object has been destroyed.
    /// - CefFrameHandler::OnMainFrameChanged => A new main frame object has been
    ///   assigned to the browser. This will only occur with cross-origin navigation
    ///   or re-navigation after renderer process termination (due to crashes, etc).
    ///
    /// (3) During final CefBrowserHost destruction of the main frame:
    /// - CefFrameHandler::OnFrameDetached => Any sub-frame objects have lost their
    ///   connection to the renderer process. Commands can no longer be routed and
    ///   will be discarded.
    /// - CefFreameHandler::OnFrameDestroyed => Any sub-frame objects have been
    ///   destroyed.
    /// - CefLifeSpanHandler::OnBeforeClose => The browser has been destroyed.
    /// - CefFrameHandler::OnFrameDetached => The main frame object have lost its
    ///   connection to the renderer process. Notifications will be sent for any
    ///   sub-frame objects before the main frame object. Commands can no longer be
    ///   routed and will be discarded.
    /// - CefFreameHandler::OnFrameDestroyed => The main frame object has been
    ///   destroyed.
    /// - CefFrameHandler::OnMainFrameChanged => The final main frame object has
    ///   been removed from the browser.
    ///
    /// Special handling applies for cross-origin loading on creation/navigation of
    /// sub-frames, and cross-origin loading on creation of new popup browsers. A
    /// temporary frame will first be created in the parent frame's renderer
    /// process. This temporary frame will never attach and will be discarded after
    /// the real cross-origin frame is created in the new/target renderer process.
    /// The client will receive creation callbacks for the temporary frame, followed
    /// by cross-origin navigation callbacks (2) for the transition from the
    /// temporary frame to the real frame. The temporary frame will not receive or
    /// execute commands during this transitional period (any sent commands will be
    /// discarded).
    ///
    /// When the main frame navigates to a different origin the OnMainFrameChanged
    /// callback (2) will be executed with the old and new main frame objects.
    ///
    /// Callbacks will not be executed for placeholders that may be created during
    /// pre-commit navigation for sub-frames that do not yet exist in the renderer
    /// process. Placeholders will have CefFrame::GetIdentifier() == -4.
    ///
    /// The methods of this class will be called on the UI thread unless otherwise
    /// indicated.
    /// </summary>
    public abstract unsafe partial class CefFrameHandler
    {
        private void on_frame_created(cef_frame_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mFrame = CefFrame.FromNative(frame);
            OnFrameCreated(mBrowser, mFrame);
        }

        /// <summary>
        /// Called when a new frame is created. This will be the first notification
        /// that references |frame|. Any commands that require transport to the
        /// associated renderer process (LoadRequest, SendProcessMessage, GetSource,
        /// etc.) will be queued. The queued commands will be sent before
        /// OnFrameAttached or discarded before OnFrameDestroyed if the frame never
        /// attaches.
        /// </summary>
        protected virtual void OnFrameCreated(CefBrowser browser, CefFrame frame)
        {
        }

        private void on_frame_destroyed(cef_frame_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mFrame = CefFrame.FromNative(frame);
            OnFrameDestroyed(mBrowser, mFrame);
        }
        
        /// <summary>
        /// Called when an existing frame is destroyed. This will be the last
        /// notification that references |frame| and CefFrame::IsValid() will return
        /// false for |frame|. If called during browser destruction and after
        /// CefLifeSpanHandler::OnBeforeClose() then CefBrowser::IsValid() will return
        /// false for |browser|. Any queued commands that have not been sent will be
        /// discarded before this callback.
        /// </summary>
        protected virtual void OnFrameDestroyed(CefBrowser browser, CefFrame frame)
        {
        }

        private void on_frame_attached(cef_frame_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, int reattached)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mFrame = CefFrame.FromNative(frame);
            OnFrameAttached(mBrowser, mFrame, reattached != 0);
        }

        /// <summary>
        /// Called when a frame can begin routing commands to/from the associated
        /// renderer process. |reattached| will be true if the frame was re-attached
        /// after exiting the BackForwardCache or after encountering a recoverable
        /// connection error. Any queued commands will now have been dispatched. This
        /// method will not be called for temporary frames created during cross-origin
        /// navigation.
        /// </summary>
        protected virtual void OnFrameAttached(CefBrowser browser, CefFrame frame, bool reattached)
        { }


        private void on_frame_detached(cef_frame_handler_t* self, cef_browser_t* browser, cef_frame_t* frame)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mFrame = CefFrame.FromNative(frame);
            OnFrameDetached(mBrowser, mFrame);
        }
        
        /// <summary>
        /// Called when a frame loses its connection to the renderer process. This may
        /// occur when a frame is destroyed, enters the BackForwardCache, or
        /// encounters a rare connection error. In the case of frame destruction this
        /// call will be followed by a (potentially async) call to OnFrameDestroyed.
        /// If frame destruction is occuring synchronously then CefFrame::IsValid()
        /// will return false for |frame|. If called during browser destruction and
        /// after CefLifeSpanHandler::OnBeforeClose() then CefBrowser::IsValid() will
        /// return false for |browser|. If, in the non-destruction case, the same
        /// frame later exits the BackForwardCache or recovers from a connection error
        /// then there will be a follow-up call to OnFrameAttached. This method will
        /// not be called for temporary frames created during cross-origin navigation.
        /// </summary>
        protected virtual void OnFrameDetached(CefBrowser browser, CefFrame frame)
        { }


        private void on_main_frame_changed(cef_frame_handler_t* self, cef_browser_t* browser, cef_frame_t* old_frame, cef_frame_t* new_frame)
        {
            CheckSelf(self);

            var mBrowser = CefBrowser.FromNative(browser);
            var mOldFrame = CefFrame.FromNativeOrNull(old_frame);
            var mNewFrame = CefFrame.FromNativeOrNull(new_frame);
            OnMainFrameChanged(mBrowser, mOldFrame, mNewFrame);
        }

        /// <summary>
        /// Called when the main frame changes due to (a) initial browser creation,
        /// (b) final browser destruction, (c) cross-origin navigation or (d)
        /// re-navigation after renderer process termination (due to crashes, etc).
        /// |old_frame| will be NULL and |new_frame| will be non-NULL when a main
        /// frame is assigned to |browser| for the first time. |old_frame| will be
        /// non-NULL and |new_frame| will be NULL when a main frame is removed from
        /// |browser| for the last time. Both |old_frame| and |new_frame| will be
        /// non-NULL for cross-origin navigations or re-navigation after renderer
        /// process termination. This method will be called after OnFrameCreated() for
        /// |new_frame| and/or after OnFrameDestroyed() for |old_frame|. If called
        /// during browser destruction and after CefLifeSpanHandler::OnBeforeClose()
        /// then CefBrowser::IsValid() will return false for |browser|.
        /// </summary>
        protected virtual void OnMainFrameChanged(CefBrowser browser, CefFrame? oldFrame, CefFrame? newFrame)
        { }
    }
}
