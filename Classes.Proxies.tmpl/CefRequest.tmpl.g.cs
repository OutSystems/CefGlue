namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent a web request. The methods of this class may be
    /// called on any thread.
    /// </summary>
    public sealed unsafe partial class CefRequest
    {
        /// <summary>
        /// Create a new CefRequest object.
        /// </summary>
        public static cef_request_t* Create()
        {
            throw new NotImplementedException(); // TODO: CefRequest.Create
        }
        
        /// <summary>
        /// Returns true if this object is read-only.
        /// </summary>
        public int IsReadOnly()
        {
            throw new NotImplementedException(); // TODO: CefRequest.IsReadOnly
        }
        
        /// <summary>
        /// Get the fully qualified URL.
        /// </summary>
        public cef_string_userfree* GetURL()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetURL
        }
        
        /// <summary>
        /// Set the fully qualified URL.
        /// </summary>
        public void SetURL(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetURL
        }
        
        /// <summary>
        /// Get the request method type. The value will default to POST if post data
        /// is provided and GET otherwise.
        /// </summary>
        public cef_string_userfree* GetMethod()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetMethod
        }
        
        /// <summary>
        /// Set the request method type.
        /// </summary>
        public void SetMethod(cef_string_t* method)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetMethod
        }
        
        /// <summary>
        /// Set the referrer URL and policy. If non-empty the referrer URL must be
        /// fully qualified with an HTTP or HTTPS scheme component. Any username,
        /// password or ref component will be removed.
        /// </summary>
        public void SetReferrer(cef_string_t* referrer_url, CefReferrerPolicy policy)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetReferrer
        }
        
        /// <summary>
        /// Get the referrer URL.
        /// </summary>
        public cef_string_userfree* GetReferrerURL()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetReferrerURL
        }
        
        /// <summary>
        /// Get the referrer policy.
        /// </summary>
        public CefReferrerPolicy GetReferrerPolicy()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetReferrerPolicy
        }
        
        /// <summary>
        /// Get the post data.
        /// </summary>
        public cef_post_data_t* GetPostData()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetPostData
        }
        
        /// <summary>
        /// Set the post data.
        /// </summary>
        public void SetPostData(cef_post_data_t* postData)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetPostData
        }
        
        /// <summary>
        /// Get the header values. Will not include the Referer value if any.
        /// </summary>
        public void GetHeaderMap(cef_string_multimap* headerMap)
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetHeaderMap
        }
        
        /// <summary>
        /// Set the header values. If a Referer value exists in the header map it will
        /// be removed and ignored.
        /// </summary>
        public void SetHeaderMap(cef_string_multimap* headerMap)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetHeaderMap
        }
        
        /// <summary>
        /// Returns the first header value for |name| or an empty string if not found.
        /// Will not return the Referer value if any. Use GetHeaderMap instead if
        /// |name| might have multiple values.
        /// </summary>
        public cef_string_userfree* GetHeaderByName(cef_string_t* name)
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetHeaderByName
        }
        
        /// <summary>
        /// Set the header |name| to |value|. If |overwrite| is true any existing
        /// values will be replaced with the new value. If |overwrite| is false any
        /// existing values will not be overwritten. The Referer value cannot be set
        /// using this method.
        /// </summary>
        public void SetHeaderByName(cef_string_t* name, cef_string_t* value, int overwrite)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetHeaderByName
        }
        
        /// <summary>
        /// Set all values at one time.
        /// </summary>
        public void Set(cef_string_t* url, cef_string_t* method, cef_post_data_t* postData, cef_string_multimap* headerMap)
        {
            throw new NotImplementedException(); // TODO: CefRequest.Set
        }
        
        /// <summary>
        /// Get the flags used in combination with CefURLRequest. See
        /// cef_urlrequest_flags_t for supported values.
        /// </summary>
        public int GetFlags()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetFlags
        }
        
        /// <summary>
        /// Set the flags used in combination with CefURLRequest.  See
        /// cef_urlrequest_flags_t for supported values.
        /// </summary>
        public void SetFlags(int flags)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetFlags
        }
        
        /// <summary>
        /// Get the URL to the first party for cookies used in combination with
        /// CefURLRequest.
        /// </summary>
        public cef_string_userfree* GetFirstPartyForCookies()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetFirstPartyForCookies
        }
        
        /// <summary>
        /// Set the URL to the first party for cookies used in combination with
        /// CefURLRequest.
        /// </summary>
        public void SetFirstPartyForCookies(cef_string_t* url)
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetFirstPartyForCookies
        }
        
        /// <summary>
        /// Get the resource type for this request. Only available in the browser
        /// process.
        /// </summary>
        public CefResourceType GetResourceType()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetResourceType
        }
        
        /// <summary>
        /// Get the transition type for this request. Only available in the browser
        /// process and only applies to requests that represent a main frame or
        /// sub-frame navigation.
        /// </summary>
        public CefTransitionType GetTransitionType()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetTransitionType
        }
        
        /// <summary>
        /// Returns the globally unique identifier for this request or 0 if not
        /// specified. Can be used by CefResourceRequestHandler implementations in the
        /// browser process to track a single request across multiple callbacks.
        /// </summary>
        public ulong GetIdentifier()
        {
            throw new NotImplementedException(); // TODO: CefRequest.GetIdentifier
        }
        
    }
}
