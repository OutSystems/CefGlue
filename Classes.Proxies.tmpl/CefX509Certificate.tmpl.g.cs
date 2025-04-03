namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing a X.509 certificate.
    /// </summary>
    public sealed unsafe partial class CefX509Certificate
    {
        /// <summary>
        /// Returns the subject of the X.509 certificate. For HTTPS server
        /// certificates this represents the web server.  The common name of the
        /// subject should match the host name of the web server.
        /// </summary>
        public cef_x509cert_principal_t* GetSubject()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetSubject
        }
        
        /// <summary>
        /// Returns the issuer of the X.509 certificate.
        /// </summary>
        public cef_x509cert_principal_t* GetIssuer()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetIssuer
        }
        
        /// <summary>
        /// Returns the DER encoded serial number for the X.509 certificate. The value
        /// possibly includes a leading 00 byte.
        /// </summary>
        public cef_binary_value_t* GetSerialNumber()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetSerialNumber
        }
        
        /// <summary>
        /// Returns the date before which the X.509 certificate is invalid.
        /// CefBaseTime.GetTimeT() will return 0 if no date was specified.
        /// </summary>
        public CefBaseTime GetValidStart()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetValidStart
        }
        
        /// <summary>
        /// Returns the date after which the X.509 certificate is invalid.
        /// CefBaseTime.GetTimeT() will return 0 if no date was specified.
        /// </summary>
        public CefBaseTime GetValidExpiry()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetValidExpiry
        }
        
        /// <summary>
        /// Returns the DER encoded data for the X.509 certificate.
        /// </summary>
        public cef_binary_value_t* GetDEREncoded()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetDEREncoded
        }
        
        /// <summary>
        /// Returns the PEM encoded data for the X.509 certificate.
        /// </summary>
        public cef_binary_value_t* GetPEMEncoded()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetPEMEncoded
        }
        
        /// <summary>
        /// Returns the number of certificates in the issuer chain.
        /// If 0, the certificate is self-signed.
        /// </summary>
        public UIntPtr GetIssuerChainSize()
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetIssuerChainSize
        }
        
        /// <summary>
        /// Returns the DER encoded data for the certificate issuer chain.
        /// If we failed to encode a certificate in the chain it is still
        /// present in the array but is an empty string.
        /// </summary>
        public void GetDEREncodedIssuerChain(UIntPtr* chainCount, cef_binary_value_t** chain)
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetDEREncodedIssuerChain
        }
        
        /// <summary>
        /// Returns the PEM encoded data for the certificate issuer chain.
        /// If we failed to encode a certificate in the chain it is still
        /// present in the array but is an empty string.
        /// </summary>
        public void GetPEMEncodedIssuerChain(UIntPtr* chainCount, cef_binary_value_t** chain)
        {
            throw new NotImplementedException(); // TODO: CefX509Certificate.GetPEMEncodedIssuerChain
        }
        
    }
}
