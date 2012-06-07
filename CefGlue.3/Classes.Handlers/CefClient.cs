namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    public abstract unsafe partial class CefClient
    {
        private cef_life_span_handler_t* get_life_span_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetLifeSpanHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for browser life span events.
        /// </summary>
        protected virtual CefLifeSpanHandler GetLifeSpanHandler()
        {
            return null;
        }

        private cef_load_handler_t* get_load_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetLoadHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        protected virtual CefLoadHandler GetLoadHandler()
        {
            return null;
        }

        private cef_request_handler_t* get_request_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetRequestHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for browser request events.
        /// </summary>
        protected virtual CefRequestHandler GetRequestHandler()
        {
            return null;
        }

        private cef_display_handler_t* get_display_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetDisplayHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for browser display state events.
        /// </summary>
        protected virtual CefDisplayHandler GetDisplayHandler()
        {
            return null;
        }

        private cef_geolocation_handler_t* get_geolocation_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetGeolocationHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for geolocation permissions requests. If no handler is
        /// provided geolocation access will be denied by default.
        /// </summary>
        protected virtual CefGeolocationHandler GetGeolocationHandler()
        {
            return null;
        }

        private cef_jsdialog_handler_t* get_jsdialog_handler(cef_client_t* self)
        {
            CheckSelf(self);

            var result = GetJSDialogHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for JavaScript dialogs. If no handler is provided the
        /// default implementation will be used.
        /// </summary>
        protected virtual CefJSDialogHandler GetJSDialogHandler()
        {
            return null;
        }

        private cef_context_menu_handler_t* get_context_menu_handler(cef_client_t* self)
        {
            CheckSelf(self);
            var result = GetContextMenuHandler();
            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for context menus. If no handler is provided the default
        /// implementation will be used.
        /// </summary>
        protected virtual CefContextMenuHandler GetContextMenuHandler()
        {
            return null;
        }

        private int on_process_message_recieved(cef_client_t* self, cef_browser_t* browser, CefProcessId source_process, cef_process_message_t* message)
        {
            CheckSelf(self);

            var m_browser = CefBrowser.FromNative(browser);
            var m_message = CefProcessMessage.FromNative(message);

            var result = OnProcessMessageRecieved(m_browser, source_process, m_message);

            m_browser.Dispose();
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
