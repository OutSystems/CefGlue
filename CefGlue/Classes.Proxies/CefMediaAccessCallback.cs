﻿namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used for asynchronous continuation of media access
    /// permission requests.
    /// </summary>
    public sealed unsafe partial class CefMediaAccessCallback
    {
        /// <summary>
        /// Call to allow or deny media access. If this callback was initiated in
        /// response to a getUserMedia (indicated by
        /// CEF_MEDIA_PERMISSION_DEVICE_AUDIO_CAPTURE and/or
        /// CEF_MEDIA_PERMISSION_DEVICE_VIDEO_CAPTURE being set) then
        /// |allowed_permissions| must match |required_permissions| passed to
        /// OnRequestMediaAccessPermission.
        /// </summary>
        public void Continue(CefMediaAccessPermissionTypes allowedPermissions)
        {
            cef_media_access_callback_t.cont(_self, (uint)allowedPermissions);
        }

        /// <summary>
        /// Cancel the media access request.
        /// </summary>
        public void Cancel()
        {
            cef_media_access_callback_t.cancel(_self);
        }
    }
}
