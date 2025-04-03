namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Callback interface used to select a client certificate for authentication.
    /// </summary>
    public sealed unsafe partial class CefSelectClientCertificateCallback
    {
        /// <summary>
        /// Chooses the specified certificate for client certificate authentication.
        /// NULL value means that no client certificate should be used.
        /// </summary>
        public void Select(cef_x509certificate_t* cert)
        {
            throw new NotImplementedException(); // TODO: CefSelectClientCertificateCallback.Select
        }
        
    }
}
