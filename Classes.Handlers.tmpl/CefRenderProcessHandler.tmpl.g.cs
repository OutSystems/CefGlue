namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to implement render process callbacks. The methods of this class
    /// will be called on the render process main thread (TID_RENDERER) unless
    /// otherwise indicated.
    /// </summary>
    public abstract unsafe partial class CefRenderProcessHandler
    {
        private void on_web_kit_initialized(cef_render_process_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnWebKitInitialized
        }
        
        /// <summary>
        /// Called after WebKit has been initialized.
        /// </summary>
        // protected abstract void OnWebKitInitialized();
        
        private void on_browser_created(cef_render_process_handler_t* self, cef_browser_t* browser, cef_dictionary_value_t* extra_info)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnBrowserCreated
        }
        
        /// <summary>
        /// Called after a browser has been created. When browsing cross-origin a new
        /// browser will be created before the old browser with the same identifier is
        /// destroyed. |extra_info| is an optional read-only value originating from
        /// CefBrowserHost::CreateBrowser(), CefBrowserHost::CreateBrowserSync(),
        /// CefLifeSpanHandler::OnBeforePopup() or
        /// CefBrowserView::CreateBrowserView().
        /// </summary>
        // protected abstract void OnBrowserCreated(cef_browser_t* browser, cef_dictionary_value_t* extra_info);
        
        private void on_browser_destroyed(cef_render_process_handler_t* self, cef_browser_t* browser)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnBrowserDestroyed
        }
        
        /// <summary>
        /// Called before a browser is destroyed.
        /// </summary>
        // protected abstract void OnBrowserDestroyed(cef_browser_t* browser);
        
        private cef_load_handler_t* get_load_handler(cef_render_process_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.GetLoadHandler
        }
        
        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        // protected abstract cef_load_handler_t* GetLoadHandler();
        
        private void on_context_created(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnContextCreated
        }
        
        /// <summary>
        /// Called immediately after the V8 context for a frame has been created. To
        /// retrieve the JavaScript 'window' object use the CefV8Context::GetGlobal()
        /// method. V8 handles can only be accessed from the thread on which they are
        /// created. A task runner for posting tasks on the associated thread can be
        /// retrieved via the CefV8Context::GetTaskRunner() method.
        /// </summary>
        // protected abstract void OnContextCreated(cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context);
        
        private void on_context_released(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnContextReleased
        }
        
        /// <summary>
        /// Called immediately before the V8 context for a frame is released. No
        /// references to the context should be kept after this method is called.
        /// </summary>
        // protected abstract void OnContextReleased(cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context);
        
        private void on_uncaught_exception(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context, cef_v8exception_t* exception, cef_v8stack_trace_t* stackTrace)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnUncaughtException
        }
        
        /// <summary>
        /// Called for global uncaught exceptions in a frame. Execution of this
        /// callback is disabled by default. To enable set
        /// cef_settings_t.uncaught_exception_stack_size &gt; 0.
        /// </summary>
        // protected abstract void OnUncaughtException(cef_browser_t* browser, cef_frame_t* frame, cef_v8context_t* context, cef_v8exception_t* exception, cef_v8stack_trace_t* stackTrace);
        
        private void on_focused_node_changed(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_domnode_t* node)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnFocusedNodeChanged
        }
        
        /// <summary>
        /// Called when a new node in the the browser gets focus. The |node| value may
        /// be empty if no specific node has gained focus. The node object passed to
        /// this method represents a snapshot of the DOM at the time this method is
        /// executed. DOM objects are only valid for the scope of this method. Do not
        /// keep references to or attempt to access any DOM objects outside the scope
        /// of this method.
        /// </summary>
        // protected abstract void OnFocusedNodeChanged(cef_browser_t* browser, cef_frame_t* frame, cef_domnode_t* node);
        
        private int on_process_message_received(cef_render_process_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, CefProcessId source_process, cef_process_message_t* message)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefRenderProcessHandler.OnProcessMessageReceived
        }
        
        /// <summary>
        /// Called when a new message is received from a different process. Return
        /// true if the message was handled or false otherwise. It is safe to keep a
        /// reference to |message| outside of this callback.
        /// </summary>
        // protected abstract int OnProcessMessageReceived(cef_browser_t* browser, cef_frame_t* frame, CefProcessId source_process, cef_process_message_t* message);
        
    }
}
