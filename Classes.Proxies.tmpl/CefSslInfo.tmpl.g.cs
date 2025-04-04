namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing SSL information.
    /// </summary>
    public sealed unsafe partial class CefSslInfo
    {
        /// <summary>
        /// Returns a bitmask containing any and all problems verifying the server
        /// certificate.
        /// </summary>
        public CefCertStatus GetCertStatus()
        {
            throw new NotImplementedException(); // TODO: CefSslInfo.GetCertStatus
        }
        
        /// <summary>
        /// Returns the X.509 certificate.
        /// </summary>
        public cef_x509certificate_t* GetX509Certificate()
        {
            throw new NotImplementedException(); // TODO: CefSslInfo.GetX509Certificate
        }
        
    }
}
