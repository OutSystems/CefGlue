namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to implement a custom request handler interface. The methods of
    /// this class will be called on the IO thread unless otherwise indicated.
    /// </summary>
    public abstract unsafe partial class CefResourceHandler
    {
        private int open(cef_resource_handler_t* self, cef_request_t* request, int* handle_request, cef_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.Open
        }
        
        /// <summary>
        /// Open the response stream. To handle the request immediately set
        /// |handle_request| to true and return true. To decide at a later time set
        /// |handle_request| to false, return true, and execute |callback| to continue
        /// or cancel the request. To cancel the request immediately set
        /// |handle_request| to true and return false. This method will be called in
        /// sequence but not from a dedicated thread. For backwards compatibility set
        /// |handle_request| to false and return false and the ProcessRequest method
        /// will be called.
        /// </summary>
        // protected abstract int Open(cef_request_t* request, int* handle_request, cef_callback_t* callback);
        
        private int process_request(cef_resource_handler_t* self, cef_request_t* request, cef_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.ProcessRequest
        }
        
        /// <summary>
        /// Begin processing the request. To handle the request return true and call
        /// CefCallback::Continue() once the response header information is available
        /// (CefCallback::Continue() can also be called from inside this method if
        /// header information is available immediately). To cancel the request return
        /// false.
        /// WARNING: This method is deprecated. Use Open instead.
        /// </summary>
        // protected abstract int ProcessRequest(cef_request_t* request, cef_callback_t* callback);
        
        private void get_response_headers(cef_resource_handler_t* self, cef_response_t* response, long* response_length, cef_string_t* redirectUrl)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.GetResponseHeaders
        }
        
        /// <summary>
        /// Retrieve response header information. If the response length is not known
        /// set |response_length| to -1 and ReadResponse() will be called until it
        /// returns false. If the response length is known set |response_length|
        /// to a positive value and ReadResponse() will be called until it returns
        /// false or the specified number of bytes have been read. Use the |response|
        /// object to set the mime type, http status code and other optional header
        /// values. To redirect the request to a new URL set |redirectUrl| to the new
        /// URL. |redirectUrl| can be either a relative or fully qualified URL.
        /// It is also possible to set |response| to a redirect http status code
        /// and pass the new URL via a Location header. Likewise with |redirectUrl| it
        /// is valid to set a relative or fully qualified URL as the Location header
        /// value. If an error occured while setting up the request you can call
        /// SetError() on |response| to indicate the error condition.
        /// </summary>
        // protected abstract void GetResponseHeaders(cef_response_t* response, long* response_length, cef_string_t* redirectUrl);
        
        private int skip(cef_resource_handler_t* self, long bytes_to_skip, long* bytes_skipped, cef_resource_skip_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.Skip
        }
        
        /// <summary>
        /// Skip response data when requested by a Range header. Skip over and discard
        /// |bytes_to_skip| bytes of response data. If data is available immediately
        /// set |bytes_skipped| to the number of bytes skipped and return true. To
        /// read the data at a later time set |bytes_skipped| to 0, return true and
        /// execute |callback| when the data is available. To indicate failure set
        /// |bytes_skipped| to &lt; 0 (e.g. -2 for ERR_FAILED) and return false. This
        /// method will be called in sequence but not from a dedicated thread.
        /// </summary>
        // protected abstract int Skip(long bytes_to_skip, long* bytes_skipped, cef_resource_skip_callback_t* callback);
        
        private int read(cef_resource_handler_t* self, void* data_out, int bytes_to_read, int* bytes_read, cef_resource_read_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.Read
        }
        
        /// <summary>
        /// Read response data. If data is available immediately copy up to
        /// |bytes_to_read| bytes into |data_out|, set |bytes_read| to the number of
        /// bytes copied, and return true. To read the data at a later time keep a
        /// pointer to |data_out|, set |bytes_read| to 0, return true and execute
        /// |callback| when the data is available (|data_out| will remain valid until
        /// the callback is executed). To indicate response completion set
        /// |bytes_read| to 0 and return false. To indicate failure set |bytes_read|
        /// to &lt; 0 (e.g. -2 for ERR_FAILED) and return false. This method will be
        /// called in sequence but not from a dedicated thread. For backwards
        /// compatibility set |bytes_read| to -1 and return false and the ReadResponse
        /// method will be called.
        /// </summary>
        // protected abstract int Read(void* data_out, int bytes_to_read, int* bytes_read, cef_resource_read_callback_t* callback);
        
        private int read_response(cef_resource_handler_t* self, void* data_out, int bytes_to_read, int* bytes_read, cef_callback_t* callback)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.ReadResponse
        }
        
        /// <summary>
        /// Read response data. If data is available immediately copy up to
        /// |bytes_to_read| bytes into |data_out|, set |bytes_read| to the number of
        /// bytes copied, and return true. To read the data at a later time set
        /// |bytes_read| to 0, return true and call CefCallback::Continue() when the
        /// data is available. To indicate response completion return false.
        /// WARNING: This method is deprecated. Use Skip and Read instead.
        /// </summary>
        // protected abstract int ReadResponse(void* data_out, int bytes_to_read, int* bytes_read, cef_callback_t* callback);
        
        private void cancel(cef_resource_handler_t* self)
        {
            CheckSelf(self);
            throw new NotImplementedException(); // TODO: CefResourceHandler.Cancel
        }
        
        /// <summary>
        /// Request processing has been canceled.
        /// </summary>
        // protected abstract void Cancel();
        
    }
}
