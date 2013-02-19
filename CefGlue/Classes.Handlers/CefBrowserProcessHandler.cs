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
        /// Called before a child process is launched. Will be called on the browser
        /// process UI thread when launching a render process and on the browser
        /// process IO thread when launching a GPU or plugin process. Provides an
        /// opportunity to modify the child process command line. Do not keep a
        /// reference to |command_line| outside of this method.
        /// </summary>
        protected virtual void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
        }


        private void on_render_process_thread_created(cef_browser_process_handler_t* self, cef_list_value_t* extra_info)
        {
            CheckSelf(self);

            var mExtraInfo = CefListValue.FromNative(extra_info);
            OnRenderProcessThreadCreated(mExtraInfo);
            mExtraInfo.Dispose();
        }

        /// <summary>
        /// Called on the browser process IO thread after the main thread has been
        /// created for a new render process. Provides an opportunity to specify extra
        /// information that will be passed to
        /// CefRenderProcessHandler::OnRenderThreadCreated() in the render process. Do
        /// not keep a reference to |extra_info| outside of this method.
        /// </summary>
        protected virtual void OnRenderProcessThreadCreated(CefListValue extraInfo)
        {
        }
    }
}
