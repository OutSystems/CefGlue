namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to provide handler implementations.
    /// </summary>
    public abstract unsafe partial class CefClient
    {
        private cef_audio_handler_t* get_audio_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetAudioHandler
        }
        
        /// <summary>
        /// Return the handler for audio rendering events.
        /// </summary>
        // protected abstract cef_audio_handler_t* GetAudioHandler();
        
        private cef_command_handler_t* get_command_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetCommandHandler
        }
        
        /// <summary>
        /// Return the handler for commands. If no handler is provided the default
        /// implementation will be used.
        /// </summary>
        // protected abstract cef_command_handler_t* GetCommandHandler();
        
        private cef_context_menu_handler_t* get_context_menu_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetContextMenuHandler
        }
        
        /// <summary>
        /// Return the handler for context menus. If no handler is provided the
        /// default implementation will be used.
        /// </summary>
        // protected abstract cef_context_menu_handler_t* GetContextMenuHandler();
        
        private cef_dialog_handler_t* get_dialog_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetDialogHandler
        }
        
        /// <summary>
        /// Return the handler for dialogs. If no handler is provided the default
        /// implementation will be used.
        /// </summary>
        // protected abstract cef_dialog_handler_t* GetDialogHandler();
        
        private cef_display_handler_t* get_display_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetDisplayHandler
        }
        
        /// <summary>
        /// Return the handler for browser display state events.
        /// </summary>
        // protected abstract cef_display_handler_t* GetDisplayHandler();
        
        private cef_download_handler_t* get_download_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetDownloadHandler
        }
        
        /// <summary>
        /// Return the handler for download events. If no handler is returned
        /// downloads will not be allowed.
        /// </summary>
        // protected abstract cef_download_handler_t* GetDownloadHandler();
        
        private cef_drag_handler_t* get_drag_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetDragHandler
        }
        
        /// <summary>
        /// Return the handler for drag events.
        /// </summary>
        // protected abstract cef_drag_handler_t* GetDragHandler();
        
        private cef_find_handler_t* get_find_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetFindHandler
        }
        
        /// <summary>
        /// Return the handler for find result events.
        /// </summary>
        // protected abstract cef_find_handler_t* GetFindHandler();
        
        private cef_focus_handler_t* get_focus_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetFocusHandler
        }
        
        /// <summary>
        /// Return the handler for focus events.
        /// </summary>
        // protected abstract cef_focus_handler_t* GetFocusHandler();
        
        private cef_frame_handler_t* get_frame_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetFrameHandler
        }
        
        /// <summary>
        /// Return the handler for events related to CefFrame lifespan. This method
        /// will be called once during CefBrowser creation and the result will be
        /// cached for performance reasons.
        /// </summary>
        // protected abstract cef_frame_handler_t* GetFrameHandler();
        
        private cef_permission_handler_t* get_permission_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetPermissionHandler
        }
        
        /// <summary>
        /// Return the handler for permission requests.
        /// </summary>
        // protected abstract cef_permission_handler_t* GetPermissionHandler();
        
        private cef_jsdialog_handler_t* get_jsdialog_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetJSDialogHandler
        }
        
        /// <summary>
        /// Return the handler for JavaScript dialogs. If no handler is provided the
        /// default implementation will be used.
        /// </summary>
        // protected abstract cef_jsdialog_handler_t* GetJSDialogHandler();
        
        private cef_keyboard_handler_t* get_keyboard_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetKeyboardHandler
        }
        
        /// <summary>
        /// Return the handler for keyboard events.
        /// </summary>
        // protected abstract cef_keyboard_handler_t* GetKeyboardHandler();
        
        private cef_life_span_handler_t* get_life_span_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetLifeSpanHandler
        }
        
        /// <summary>
        /// Return the handler for browser life span events.
        /// </summary>
        // protected abstract cef_life_span_handler_t* GetLifeSpanHandler();
        
        private cef_load_handler_t* get_load_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetLoadHandler
        }
        
        /// <summary>
        /// Return the handler for browser load status events.
        /// </summary>
        // protected abstract cef_load_handler_t* GetLoadHandler();
        
        private cef_print_handler_t* get_print_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetPrintHandler
        }
        
        /// <summary>
        /// Return the handler for printing on Linux. If a print handler is not
        /// provided then printing will not be supported on the Linux platform.
        /// </summary>
        // protected abstract cef_print_handler_t* GetPrintHandler();
        
        private cef_render_handler_t* get_render_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetRenderHandler
        }
        
        /// <summary>
        /// Return the handler for off-screen rendering events.
        /// </summary>
        // protected abstract cef_render_handler_t* GetRenderHandler();
        
        private cef_request_handler_t* get_request_handler(cef_client_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.GetRequestHandler
        }
        
        /// <summary>
        /// Return the handler for browser request events.
        /// </summary>
        // protected abstract cef_request_handler_t* GetRequestHandler();
        
        private int on_process_message_received(cef_client_t* self, cef_browser_t* browser, cef_frame_t* frame, CefProcessId source_process, cef_process_message_t* message)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefClient.OnProcessMessageReceived
        }
        
        /// <summary>
        /// Called when a new message is received from a different process. Return
        /// true if the message was handled or false otherwise.  It is safe to keep a
        /// reference to |message| outside of this callback.
        /// </summary>
        // protected abstract int OnProcessMessageReceived(cef_browser_t* browser, cef_frame_t* frame, CefProcessId source_process, cef_process_message_t* message);
        
    }
}
