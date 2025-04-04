namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a web response. The methods of this class may be
    /// called on any thread.
    /// </summary>
    public sealed unsafe partial class CefResponse
    {
        /// <summary>
        /// Create a new CefResponse object.
        /// </summary>
        public static cef_response_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefResponse.Create
        }
        
        /// <summary>
        /// Returns true if this object is read-only.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefResponse.IsReadOnly
        }
        
        /// <summary>
        /// Get the response error code. Returns ERR_NONE if there was no error.
        /// </summary>
        public CefErrorCode GetError()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetError
        }
        
        /// <summary>
        /// Set the response error code. This can be used by custom scheme handlers
        /// to return errors during initial request processing.
        /// </summary>
        public void SetError(CefErrorCode error)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetError
        }
        
        /// <summary>
        /// Get the response status code.
        /// </summary>
        public int GetStatus()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetStatus
        }
        
        /// <summary>
        /// Set the response status code.
        /// </summary>
        public void SetStatus(int status)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetStatus
        }
        
        /// <summary>
        /// Get the response status text.
        /// </summary>
        public cef_string_userfree* GetStatusText()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetStatusText
        }
        
        /// <summary>
        /// Set the response status text.
        /// </summary>
        public void SetStatusText(cef_string_t* statusText)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetStatusText
        }
        
        /// <summary>
        /// Get the response mime type.
        /// </summary>
        public cef_string_userfree* GetMimeType()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetMimeType
        }
        
        /// <summary>
        /// Set the response mime type.
        /// </summary>
        public void SetMimeType(cef_string_t* mimeType)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetMimeType
        }
        
        /// <summary>
        /// Get the response charset.
        /// </summary>
        public cef_string_userfree* GetCharset()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetCharset
        }
        
        /// <summary>
        /// Set the response charset.
        /// </summary>
        public void SetCharset(cef_string_t* charset)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetCharset
        }
        
        /// <summary>
        /// Get the value for the specified response header field.
        /// </summary>
        public cef_string_userfree* GetHeaderByName(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetHeaderByName
        }
        
        /// <summary>
        /// Set the header |name| to |value|. If |overwrite| is true any existing
        /// values will be replaced with the new value. If |overwrite| is false any
        /// existing values will not be overwritten.
        /// </summary>
        public void SetHeaderByName(cef_string_t* name, cef_string_t* value, int overwrite)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetHeaderByName
        }
        
        /// <summary>
        /// Get all response header fields.
        /// </summary>
        public void GetHeaderMap(cef_string_multimap* headerMap)
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetHeaderMap
        }
        
        /// <summary>
        /// Set all response header fields.
        /// </summary>
        public void SetHeaderMap(cef_string_multimap* headerMap)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetHeaderMap
        }
        
        /// <summary>
        /// Get the resolved URL after redirects or changed as a result of HSTS.
        /// </summary>
        public cef_string_userfree* GetURL()
        {
            throw new NotImplementedException(); // TODO: CefResponse.GetURL
        }
        
        /// <summary>
        /// Set the resolved URL after redirects or changed as a result of HSTS.
        /// </summary>
        public void SetURL(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefResponse.SetURL
        }
        
    }
}
