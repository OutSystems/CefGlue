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
        public static CefRequest Create()
        {
            return CefRequest.FromNative(
                cef_request_t.create()
                );
        }

        /// <summary>
        /// Gets or sets the fully qualified URL.
        /// </summary>
        public string Url
        {
            get
            {
                var n_result = cef_request_t.get_url(_self);
                return cef_string_userfree.ToString(n_result);
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                fixed (char* value_str = value)
                {
                    var n_value = new cef_string_t(value_str, value.Length);
                    cef_request_t.set_url(_self, &n_value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the request method type.
        /// The value will default to POST if post data is provided and GET otherwise.
        /// </summary>
        public string Method
        {
            get
            {
                var n_result = cef_request_t.get_method(_self);
                return cef_string_userfree.ToString(n_result);
            }
            set
            {
                fixed (char* value_str = value)
                {
                    var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);
                    cef_request_t.set_method(_self, &n_value);
                }
            }
        }

        /// <summary>
        /// Get the post data.
        /// </summary>
        public CefPostData PostData
        {
            get
            {
                return CefPostData.FromNativeOrNull(
                    cef_request_t.get_post_data(_self)
                    );
            }
            set
            {
                var n_value = value != null ? value.ToNative() : null;
                cef_request_t.set_post_data(_self, n_value);
            }
        }

        /// <summary>
        /// Get the header values.
        /// </summary>
        public void GetHeaderMap()
        {
            var headerMap = libcef.string_multimap_alloc();
            cef_request_t.get_header_map(_self, headerMap);
            // var result = 0; // move to collection
            libcef.string_multimap_free(headerMap);
            //return result;
            throw new NotImplementedException(); // TODO: CefRequest.GetHeaderMap
        }

        /// <summary>
        /// Set the header values.
        /// </summary>
        public void SetHeaderMap()
        {
            throw new NotImplementedException(); // TODO: CefRequest.SetHeaderMap
            cef_string_multimap* headerMap = null;
            cef_request_t.set_header_map(_self, headerMap);
        }

        /// <summary>
        /// Set all values at one time.
        /// </summary>
        public void Set(string url, string method, CefPostData postData) // , cef_string_multimap* headerMap)
        {
            fixed (char* url_str = url)
            fixed (char* method_str = method)
            {
                var n_url = new cef_string_t(url_str, url != null ? url.Length : 0);
                var n_method = new cef_string_t(method_str, method_str != null ? method.Length : 0);
                var n_postData = postData != null ? postData.ToNative() : null;
                var n_headerMap = (cef_string_multimap*)null; // TODO: CefRequest.Set (headerMap)
                cef_request_t.set(_self, &n_url, &n_method, n_postData, n_headerMap);
            }
        }

        /// <summary>
        /// Get the flags used in combination with CefWebURLRequest.
        /// </summary>
        public CefWebUrlRequestOptions Options
        {
            get { return cef_request_t.get_flags(_self); }
            set { cef_request_t.set_flags(_self, value); }
        }

        /// <summary>
        /// Gets or sets the URL to the first party for cookies used in combination with
        /// CefWebURLRequest.
        /// </summary>
        public string FirstPartyForCookies
        {
            get
            {
                var n_result = cef_request_t.get_first_party_for_cookies(_self);
                return cef_string_userfree.ToString(n_result);
            }
            set
            {
                fixed (char* value_str = value)
                {
                    var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);
                    cef_request_t.set_first_party_for_cookies(_self, &n_value);
                }
            }
        }
    }
}
