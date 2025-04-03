namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class that manages custom scheme registrations.
    /// </summary>
    public sealed unsafe partial class CefSchemeRegistrar
    {
        /// <summary>
        /// Register a custom scheme. This method should not be called for the
        /// built-in HTTP, HTTPS, FILE, FTP, ABOUT and DATA schemes.
        /// See cef_scheme_options_t for possible values for |options|.
        /// This function may be called on any thread. It should only be called once
        /// per unique |scheme_name| value. If |scheme_name| is already registered or
        /// if an error occurs this method will return false.
        /// </summary>
        public int AddCustomScheme(cef_string_t* scheme_name, int options)
        {
            throw new NotImplementedException(); // TODO: CefSchemeRegistrar.AddCustomScheme
        }
        
    }
}
