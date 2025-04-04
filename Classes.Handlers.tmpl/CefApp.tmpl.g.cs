namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to provide handler implementations. Methods will be
    /// called by the process and/or thread indicated.
    /// </summary>
    public abstract unsafe partial class CefApp
    {
        private void on_before_command_line_processing(cef_app_t* self, cef_string_t* process_type, cef_command_line_t* command_line)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefApp.OnBeforeCommandLineProcessing
        }
        
        /// <summary>
        /// Provides an opportunity to view and/or modify command-line arguments
        /// before processing by CEF and Chromium. The |process_type| value will be
        /// empty for the browser process. Do not keep a reference to the
        /// CefCommandLine object passed to this method. The
        /// cef_settings_t.command_line_args_disabled value can be used to start with
        /// an empty command-line object. Any values specified in CefSettings that
        /// equate to command-line arguments will be set before this method is called.
        /// Be cautious when using this method to modify command-line arguments for
        /// non-browser processes as this may result in undefined behavior including
        /// crashes.
        /// </summary>
        // protected abstract void OnBeforeCommandLineProcessing(cef_string_t* process_type, cef_command_line_t* command_line);
        
        private void on_register_custom_schemes(cef_app_t* self, cef_scheme_registrar_t* registrar)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefApp.OnRegisterCustomSchemes
        }
        
        /// <summary>
        /// Provides an opportunity to register custom schemes. Do not keep a
        /// reference to the |registrar| object. This method is called on the main
        /// thread for each process and the registered schemes should be the same
        /// across all processes.
        /// </summary>
        // protected abstract void OnRegisterCustomSchemes(cef_scheme_registrar_t* registrar);
        
        private cef_resource_bundle_handler_t* get_resource_bundle_handler(cef_app_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefApp.GetResourceBundleHandler
        }
        
        /// <summary>
        /// Return the handler for resource bundle events. If no handler is returned
        /// resources will be loaded from pack files. This method is called by the
        /// browser and render processes on multiple threads.
        /// </summary>
        // protected abstract cef_resource_bundle_handler_t* GetResourceBundleHandler();
        
        private cef_browser_process_handler_t* get_browser_process_handler(cef_app_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefApp.GetBrowserProcessHandler
        }
        
        /// <summary>
        /// Return the handler for functionality specific to the browser process. This
        /// method is called on multiple threads in the browser process.
        /// </summary>
        // protected abstract cef_browser_process_handler_t* GetBrowserProcessHandler();
        
        private cef_render_process_handler_t* get_render_process_handler(cef_app_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefApp.GetRenderProcessHandler
        }
        
        /// <summary>
        /// Return the handler for functionality specific to the render process. This
        /// method is called on the render process main thread.
        /// </summary>
        // protected abstract cef_render_process_handler_t* GetRenderProcessHandler();
        
    }
}
