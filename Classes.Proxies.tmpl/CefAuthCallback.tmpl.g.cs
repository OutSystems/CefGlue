namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used for asynchronous continuation of authentication
    /// requests.
    /// </summary>
    public sealed unsafe partial class CefAuthCallback
    {
        /// <summary>
        /// Continue the authentication request.
        /// </summary>
        public void Continue(cef_string_t* username, cef_string_t* password)
        {
            throw new NotImplementedException(); // TODO: CefAuthCallback.Continue
        }
        
        /// <summary>
        /// Cancel the authentication request.
        /// </summary>
        public void Cancel()
        {
            throw new NotImplementedException(); // TODO: CefAuthCallback.Cancel
        }
        
    }
}
