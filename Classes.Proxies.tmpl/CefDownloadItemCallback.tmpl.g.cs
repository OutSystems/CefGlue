namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used to asynchronously cancel a download.
    /// </summary>
    public sealed unsafe partial class CefDownloadItemCallback
    {
        /// <summary>
        /// Call to cancel the download.
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItemCallback.Cancel
        }
        
        /// <summary>
        /// Call to pause the download.
        /// </summary>
        public void Pause()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItemCallback.Pause
        }
        
        /// <summary>
        /// Call to resume the download.
        /// </summary>
        public void Resume()
        {
            throw new NotImplementedException(); // TODO: CefDownloadItemCallback.Resume
        }
        
    }
}
