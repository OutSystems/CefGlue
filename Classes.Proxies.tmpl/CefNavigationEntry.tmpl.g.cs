namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class used to represent an entry in navigation history.
    /// </summary>
    public sealed unsafe partial class CefNavigationEntry
    {
        /// <summary>
        /// Returns true if this object is valid. Do not call any other methods if
        /// this function returns false.
        /// </summary>
        public int IsValid()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.IsValid
        }
        
        /// <summary>
        /// Returns the actual URL of the page. For some pages this may be data: URL
        /// or similar. Use GetDisplayURL() to return a display-friendly version.
        /// </summary>
        public cef_string_userfree* GetURL()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetURL
        }
        
        /// <summary>
        /// Returns a display-friendly version of the URL.
        /// </summary>
        public cef_string_userfree* GetDisplayURL()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetDisplayURL
        }
        
        /// <summary>
        /// Returns the original URL that was entered by the user before any
        /// redirects.
        /// </summary>
        public cef_string_userfree* GetOriginalURL()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetOriginalURL
        }
        
        /// <summary>
        /// Returns the title set by the page. This value may be empty.
        /// </summary>
        public cef_string_userfree* GetTitle()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetTitle
        }
        
        /// <summary>
        /// Returns the transition type which indicates what the user did to move to
        /// this page from the previous page.
        /// </summary>
        public CefTransitionType GetTransitionType()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetTransitionType
        }
        
        /// <summary>
        /// Returns true if this navigation includes post data.
        /// </summary>
        public int HasPostData()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.HasPostData
        }
        
        /// <summary>
        /// Returns the time for the last known successful navigation completion. A
        /// navigation may be completed more than once if the page is reloaded. May be
        /// 0 if the navigation has not yet completed.
        /// </summary>
        public CefBaseTime GetCompletionTime()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetCompletionTime
        }
        
        /// <summary>
        /// Returns the HTTP status code for the last known successful navigation
        /// response. May be 0 if the response has not yet been received or if the
        /// navigation has not yet completed.
        /// </summary>
        public int GetHttpStatusCode()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetHttpStatusCode
        }
        
        /// <summary>
        /// Returns the SSL information for this navigation entry.
        /// </summary>
        public cef_sslstatus_t* GetSSLStatus()
        {
            throw new NotImplementedException(); // TODO: CefNavigationEntry.GetSSLStatus
        }
        
    }
}
