namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class used to implement render process callbacks. The methods of this class
    /// will always be called on the render process main thread.
    /// </summary>
    public abstract unsafe partial class CefRenderProcessHandler
    {
        private void on_render_thread_created(cef_render_process_handler_t* self)
        {
            CheckSelf(self);

            OnRenderThreadCreated();
        }

        /// <summary>
        /// Called after the render process main thread has been created.
        /// </summary>
        protected virtual void OnRenderThreadCreated()
        {
        }


        private void on_web_kit_initialized(cef_render_process_handler_t* self)
        {
            CheckSelf(self);

            OnWebKitInitialized();
        }

        /// <summary>
        /// Called after WebKit has been initialized.
        /// </summary>
        protected virtual void OnWebKitInitialized()
        {
        }


        private void on_browser_created(cef_render_process_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            OnBrowserCreated(m_browser);
        }

        /// <summary>
        /// Called after a browser has been created.
        /// </summary>
        protected virtual void OnBrowserCreated(CefBrowser browser)
        {
        }


        private void on_browser_destroyed(cef_render_process_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);

            OnBrowserDestroyed(m_browser);
        }

        /// <summary>
        /// Called before a browser is destroyed.
        /// </summary>
        protected abstract void OnBrowserDestroyed(CefBrowser browser);


        private void on_context_created(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_context = CefV8Context.FromNative(context);

            OnContextCreated(m_browser, m_frame, m_context);
        }

        /// <summary>
        /// Called immediately after the V8 context for a frame has been created. To
        /// retrieve the JavaScript 'window' object use the CefV8Context::GetGlobal()
        /// method.
        /// </summary>
        protected virtual void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
        }


        private void on_context_released(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_frame = CefFrame.FromNative(frame);
            var m_context = CefV8Context.FromNative(context);

            OnContextReleased(m_browser, m_frame, m_context);
        }

        /// <summary>
        /// Called immediately before the V8 context for a frame is released. No
        /// references to the context should be kept after this method is called.
        /// </summary>
        protected virtual void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
        {
        }


        private int on_process_message_recieved(cef_render_process_handler_t* self, cef_browser_t* browser, CefProcessId source_process, cef_process_message_t* message)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_message = CefProcessMessage.FromNative(message);

            var result = OnProcessMessageRecieved(m_browser, source_process, m_message);

            m_message.Dispose();

            return result ? 1 : 0;
        }

        /// <summary>
        /// Called when a new message is received from a different process. Return true
        /// if the message was handled or false otherwise. Do not keep a reference to
        /// or attempt to access the message outside of this callback.
        /// </summary>
        protected virtual bool OnProcessMessageRecieved(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return false;
        }
    }
}
