namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    using System.IO;

    /// <summary>
    /// Interface that should be implemented by the CefURLRequest client. The
    /// methods of this class will be called on the same thread that created the
    /// request.
    /// </summary>
    public abstract unsafe partial class CefUrlRequestClient
    {
        private void on_request_complete(cef_urlrequest_client_t* self, cef_urlrequest_t* request)
        {
            CheckSelf(self);

            var m_request = CefUrlRequest.FromNative(request);

            OnRequestComplete(m_request);
        }

        /// <summary>
        /// Notifies the client that the request has completed. Use the
        /// CefURLRequest::GetRequestStatus method to determine if the request was
        /// successful or not.
        /// </summary>
        protected abstract void OnRequestComplete(CefUrlRequest request);


        private void on_upload_progress(cef_urlrequest_client_t* self, cef_urlrequest_t* request, ulong current, ulong total)
        {
            CheckSelf(self);

            var m_request = CefUrlRequest.FromNative(request);

            OnUploadProgress(m_request, current, total);
        }

        /// <summary>
        /// Notifies the client of upload progress. |current| denotes the number of
        /// bytes sent so far and |total| is the total size of uploading data (or -1 if
        /// chunked upload is enabled). This method will only be called if the
        /// UR_FLAG_REPORT_UPLOAD_PROGRESS flag is set on the request.
        /// </summary>
        protected abstract void OnUploadProgress(CefUrlRequest request, ulong current, ulong total);


        private void on_download_progress(cef_urlrequest_client_t* self, cef_urlrequest_t* request, ulong current, ulong total)
        {
            CheckSelf(self);

            var m_request = CefUrlRequest.FromNative(request);

            OnDownloadProgress(m_request, current, total);
        }

        /// <summary>
        /// Notifies the client of download progress. |current| denotes the number of
        /// bytes received up to the call and |total| is the expected total size of the
        /// response (or -1 if not determined).
        /// </summary>
        protected abstract void OnDownloadProgress(CefUrlRequest request, ulong current, ulong total);


        private void on_download_data(cef_urlrequest_client_t* self, cef_urlrequest_t* request, void* data, UIntPtr data_length)
        {
            CheckSelf(self);

            var m_request = CefUrlRequest.FromNative(request);

            using (var stream = new UnmanagedMemoryStream((byte*)data, (long)data_length))
            {
                OnDownloadData(m_request, stream);
            }
        }

        /// <summary>
        /// Called when some part of the response is read. |data| contains the current
        /// bytes received since the last call. This method will not be called if the
        /// UR_FLAG_NO_DOWNLOAD_DATA flag is set on the request.
        /// </summary>
        protected abstract void OnDownloadData(CefUrlRequest request, Stream data);
    }
}
