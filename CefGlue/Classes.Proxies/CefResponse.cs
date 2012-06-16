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
        /// Gets or sets the response status code.
        /// </summary>
        public int Status
        {
            get { return cef_response_t.get_status(_self); }
            set { cef_response_t.set_status(_self, value); }
        }

        /// <summary>
        /// Get the response status text.
        /// </summary>
        public string StatusText
        {
            get
            {
                var n_result = cef_response_t.get_status_text(_self);
                return cef_string_userfree.ToString(n_result);
            }
            set
            {
                fixed (char* value_str = value)
                {
                    var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);
                    cef_response_t.set_status_text(_self, &n_value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the response mime type.
        /// </summary>
        public string MimeType
        {
            get
            {
                var n_result = cef_response_t.get_mime_type(_self);
                return cef_string_userfree.ToString(n_result);
            }
            set
            {
                fixed (char* value_str = value)
                {
                    var n_value = new cef_string_t(value_str, value != null ? value.Length : 0);
                    cef_response_t.set_mime_type(_self, &n_value);
                }
            }
        }

        /// <summary>
        /// Get the value for the specified response header field.
        /// </summary>
        public string GetHeader(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            fixed (char* name_str = name)
            {
                var n_name = new cef_string_t(name_str, name.Length);
                var n_result = cef_response_t.get_header(_self, &n_name);
                return cef_string_userfree.ToString(n_result);
            }
        }

        /// <summary>
        /// Get all response header fields.
        /// </summary>
        public void GetHeaderMap()
        {
            // cef_response_t.get_header_map(_self, headerMap);
            throw new NotImplementedException(); // TODO: CefResponse.GetHeaderMap
        }

        /// <summary>
        /// Set all response header fields.
        /// </summary>
        public void SetHeaderMap()
        {
            // cef_response_t.set_header_map(_self, headerMap);
            throw new NotImplementedException(); // TODO: CefResponse.SetHeaderMap
        }
    }
}
