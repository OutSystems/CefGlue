namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Interface that should be implemented by the CefURLRequest client. The
    /// methods of this class will be called on the same thread that created the
    /// request unless otherwise documented.
    /// </summary>
    public abstract unsafe partial class CefUrlRequestClient
    {
        private void on_request_complete(cef_urlrequest_client_t* self, cef_urlrequest_t* request)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefUrlRequestClient.OnRequestComplete
        }
        
        /// <summary>
        /// Notifies the client that the request has completed. Use the
        /// CefURLRequest::GetRequestStatus method to determine if the request was
        /// successful or not.
        /// </summary>
        // protected abstract void OnRequestComplete(cef_urlrequest_t* request);
        
        private void on_upload_progress(cef_urlrequest_client_t* self, cef_urlrequest_t* request, long current, long total)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefUrlRequestClient.OnUploadProgress
        }
        
        /// <summary>
        /// Notifies the client of upload progress. |current| denotes the number of
        /// bytes sent so far and |total| is the total size of uploading data (or -1
        /// if chunked upload is enabled). This method will only be called if the
        /// UR_FLAG_REPORT_UPLOAD_PROGRESS flag is set on the request.
        /// </summary>
        // protected abstract void OnUploadProgress(cef_urlrequest_t* request, long current, long total);
        
        private void on_download_progress(cef_urlrequest_client_t* self, cef_urlrequest_t* request, long current, long total)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefUrlRequestClient.OnDownloadProgress
        }
        
        /// <summary>
        /// Notifies the client of download progress. |current| denotes the number of
        /// bytes received up to the call and |total| is the expected total size of
        /// the response (or -1 if not determined).
        /// </summary>
        // protected abstract void OnDownloadProgress(cef_urlrequest_t* request, long current, long total);
        
        private void on_download_data(cef_urlrequest_client_t* self, cef_urlrequest_t* request, void* data, UIntPtr data_length)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefUrlRequestClient.OnDownloadData
        }
        
        /// <summary>
        /// Called when some part of the response is read. |data| contains the current
        /// bytes received since the last call. This method will not be called if the
        /// UR_FLAG_NO_DOWNLOAD_DATA flag is set on the request.
        /// </summary>
        // protected abstract void OnDownloadData(cef_urlrequest_t* request, void* data, UIntPtr data_length);
        
        private int get_auth_credentials(cef_urlrequest_client_t* self, int isProxy, cef_string_t* host, int port, cef_string_t* realm, cef_string_t* scheme, cef_auth_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefUrlRequestClient.GetAuthCredentials
        }
        
        /// <summary>
        /// Called on the IO thread when the browser needs credentials from the user.
        /// |isProxy| indicates whether the host is a proxy server. |host| contains
        /// the hostname and |port| contains the port number. Return true to continue
        /// the request and call CefAuthCallback::Continue() when the authentication
        /// information is available. If the request has an associated browser/frame
        /// then returning false will result in a call to GetAuthCredentials on the
        /// CefRequestHandler associated with that browser, if any. Otherwise,
        /// returning false will cancel the request immediately. This method will only
        /// be called for requests initiated from the browser process.
        /// </summary>
        // protected abstract int GetAuthCredentials(int isProxy, cef_string_t* host, int port, cef_string_t* realm, cef_string_t* scheme, cef_auth_callback_t* callback);
        
    }
}
