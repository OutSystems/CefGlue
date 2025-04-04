namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Implement this interface to handle events related to permission requests.
    /// The methods of this class will be called on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefPermissionHandler
    {
        private int on_request_media_access_permission(cef_permission_handler_t* self, cef_browser_t* browser, cef_frame_t* frame, cef_string_t* requesting_origin, uint requested_permissions, cef_media_access_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefPermissionHandler.OnRequestMediaAccessPermission
        }
        
        /// <summary>
        /// Called when a page requests permission to access media.
        /// |requesting_origin| is the URL origin requesting permission.
        /// |requested_permissions| is a combination of values from
        /// cef_media_access_permission_types_t that represent the requested
        /// permissions. Return true and call CefMediaAccessCallback methods either in
        /// this method or at a later time to continue or cancel the request. Return
        /// false to proceed with default handling. With Chrome style, default
        /// handling will display the permission request UI. With Alloy style,
        /// default handling will deny the request. This method will not be called if
        /// the "--enable-media-stream" command-line switch is used to grant all
        /// permissions.
        /// </summary>
        // protected abstract int OnRequestMediaAccessPermission(cef_browser_t* browser, cef_frame_t* frame, cef_string_t* requesting_origin, uint requested_permissions, cef_media_access_callback_t* callback);
        
        private int on_show_permission_prompt(cef_permission_handler_t* self, cef_browser_t* browser, ulong prompt_id, cef_string_t* requesting_origin, uint requested_permissions, cef_permission_prompt_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefPermissionHandler.OnShowPermissionPrompt
        }
        
        /// <summary>
        /// Called when a page should show a permission prompt. |prompt_id| uniquely
        /// identifies the prompt. |requesting_origin| is the URL origin requesting
        /// permission. |requested_permissions| is a combination of values from
        /// cef_permission_request_types_t that represent the requested permissions.
        /// Return true and call CefPermissionPromptCallback::Continue either in this
        /// method or at a later time to continue or cancel the request. Return false
        /// to proceed with default handling. With Chrome style, default handling will
        /// display the permission prompt UI. With Alloy style, default handling is
        /// CEF_PERMISSION_RESULT_IGNORE.
        /// </summary>
        // protected abstract int OnShowPermissionPrompt(cef_browser_t* browser, ulong prompt_id, cef_string_t* requesting_origin, uint requested_permissions, cef_permission_prompt_callback_t* callback);
        
        private void on_dismiss_permission_prompt(cef_permission_handler_t* self, cef_browser_t* browser, ulong prompt_id, CefPermissionRequestResult result)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefPermissionHandler.OnDismissPermissionPrompt
        }
        
        /// <summary>
        /// Called when a permission prompt handled via OnShowPermissionPrompt is
        /// dismissed. |prompt_id| will match the value that was passed to
        /// OnShowPermissionPrompt. |result| will be the value passed to
        /// CefPermissionPromptCallback::Continue or CEF_PERMISSION_RESULT_IGNORE if
        /// the dialog was dismissed for other reasons such as navigation, browser
        /// closure, etc. This method will not be called if OnShowPermissionPrompt
        /// returned false for |prompt_id|.
        /// </summary>
        // protected abstract void OnDismissPermissionPrompt(cef_browser_t* browser, ulong prompt_id, CefPermissionRequestResult result);
        
    }
}
