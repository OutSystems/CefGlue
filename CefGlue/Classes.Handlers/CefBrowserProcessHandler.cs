namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;

    /// <summary>
    /// Class used to implement browser process callbacks. The methods of this class
    /// will be called on the browser process main thread unless otherwise indicated.
    /// </summary>
    public abstract unsafe partial class CefBrowserProcessHandler
    {
        private cef_proxy_handler_t* get_proxy_handler(cef_browser_process_handler_t* self)
        {
            CheckSelf(self);

            var result = GetProxyHandler();

            return result != null ? result.ToNative() : null;
        }

        /// <summary>
        /// Return the handler for proxy events. If no handler is returned the default
        /// system handler will be used. This method is called on the browser process
        /// IO thread.
        /// </summary>
        protected virtual CefProxyHandler GetProxyHandler()
        {
            return null;
        }


        private void on_context_initialized(cef_browser_process_handler_t* self)
        {
            CheckSelf(self);

            OnContextInitialized();
        }

        /// <summary>
        /// Called on the browser process UI thread immediately after the CEF context
        /// has been initialized.
        /// </summary>
        protected virtual void OnContextInitialized()
        {
        }


        private void on_before_child_process_launch(cef_browser_process_handler_t* self, cef_command_line_t* command_line)
        {
            CheckSelf(self);

            var m_commandLine = CefCommandLine.FromNative(command_line);
            OnBeforeChildProcessLaunch(m_commandLine);
            m_commandLine.Dispose();
        }

        /// <summary>
        /// Called on the browser process IO thread before a child process is launched.
        /// Provides an opportunity to modify the child process command line.
        /// </summary>
        protected virtual void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
        }
    }
}
