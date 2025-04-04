namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to handle file downloads. The methods of this class will called
    /// on the browser process UI thread.
    /// </summary>
    public abstract unsafe partial class CefDownloadHandler
    {
        private int can_download(cef_download_handler_t* self, cef_browser_t* browser, cef_string_t* url, cef_string_t* request_method)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefDownloadHandler.CanDownload
        }
        
        /// <summary>
        /// Called before a download begins in response to a user-initiated action
        /// (e.g. alt + link click or link click that returns a `Content-Disposition:
        /// attachment` response from the server). |url| is the target download URL
        /// and |request_method| is the target method (GET, POST, etc). Return true to
        /// proceed with the download or false to cancel the download.
        /// </summary>
        // protected abstract int CanDownload(cef_browser_t* browser, cef_string_t* url, cef_string_t* request_method);
        
        private int on_before_download(cef_download_handler_t* self, cef_browser_t* browser, cef_download_item_t* download_item, cef_string_t* suggested_name, cef_before_download_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefDownloadHandler.OnBeforeDownload
        }
        
        /// <summary>
        /// Called before a download begins. |suggested_name| is the suggested name
        /// for the download file. Return true and execute |callback| either
        /// asynchronously or in this method to continue or cancel the download.
        /// Return false to proceed with default handling (cancel with Alloy style,
        /// download shelf with Chrome style). Do not keep a reference to
        /// |download_item| outside of this method.
        /// </summary>
        // protected abstract int OnBeforeDownload(cef_browser_t* browser, cef_download_item_t* download_item, cef_string_t* suggested_name, cef_before_download_callback_t* callback);
        
        private void on_download_updated(cef_download_handler_t* self, cef_browser_t* browser, cef_download_item_t* download_item, cef_download_item_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefDownloadHandler.OnDownloadUpdated
        }
        
        /// <summary>
        /// Called when a download's status or progress information has been updated.
        /// This may be called multiple times before and after OnBeforeDownload().
        /// Execute |callback| either asynchronously or in this method to cancel the
        /// download if desired. Do not keep a reference to |download_item| outside of
        /// this method.
        /// </summary>
        // protected abstract void OnDownloadUpdated(cef_browser_t* browser, cef_download_item_t* download_item, cef_download_item_callback_t* callback);
        
    }
}
