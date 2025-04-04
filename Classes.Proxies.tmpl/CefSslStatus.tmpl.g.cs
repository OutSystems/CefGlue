namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing the SSL information for a navigation entry.
    /// </summary>
    public sealed unsafe partial class CefSslStatus
    {
        /// <summary>
        /// Returns true if the status is related to a secure SSL/TLS connection.
        /// </summary>
        public int IsSecureConnection()
        {
            throw new NotImplementedException(); // TODO: CefSslStatus.IsSecureConnection
        }
        
        /// <summary>
        /// Returns a bitmask containing any and all problems verifying the server
        /// certificate.
        /// </summary>
        public CefCertStatus GetCertStatus()
        {
            throw new NotImplementedException(); // TODO: CefSslStatus.GetCertStatus
        }
        
        /// <summary>
        /// Returns the SSL version used for the SSL connection.
        /// </summary>
        public CefSslVersion GetSSLVersion()
        {
            throw new NotImplementedException(); // TODO: CefSslStatus.GetSSLVersion
        }
        
        /// <summary>
        /// Returns a bitmask containing the page security content status.
        /// </summary>
        public CefSslContentStatus GetContentStatus()
        {
            throw new NotImplementedException(); // TODO: CefSslStatus.GetContentStatus
        }
        
        /// <summary>
        /// Returns the X.509 certificate.
        /// </summary>
        public cef_x509certificate_t* GetX509Certificate()
        {
            throw new NotImplementedException(); // TODO: CefSslStatus.GetX509Certificate
        }
        
    }
}
