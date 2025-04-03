namespace Xilium.CefGlue
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Xilium.CefGlue.Interop;
    
    /// <summary>
    /// Class representing the issuer or subject field of an X.509 certificate.
    /// </summary>
    public sealed unsafe partial class CefX509CertPrincipal
    {
        /// <summary>
        /// Returns a name that can be used to represent the issuer. It tries in this
        /// order: Common Name (CN), Organization Name (O) and Organizational Unit
        /// Name (OU) and returns the first non-empty one found.
        /// </summary>
        public cef_string_userfree* GetDisplayName()
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetDisplayName
        }
        
        /// <summary>
        /// Returns the common name.
        /// </summary>
        public cef_string_userfree* GetCommonName()
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetCommonName
        }
        
        /// <summary>
        /// Returns the locality name.
        /// </summary>
        public cef_string_userfree* GetLocalityName()
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetLocalityName
        }
        
        /// <summary>
        /// Returns the state or province name.
        /// </summary>
        public cef_string_userfree* GetStateOrProvinceName()
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetStateOrProvinceName
        }
        
        /// <summary>
        /// Returns the country name.
        /// </summary>
        public cef_string_userfree* GetCountryName()
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetCountryName
        }
        
        /// <summary>
        /// Retrieve the list of organization names.
        /// </summary>
        public void GetOrganizationNames(cef_string_list* names)
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetOrganizationNames
        }
        
        /// <summary>
        /// Retrieve the list of organization unit names.
        /// </summary>
        public void GetOrganizationUnitNames(cef_string_list* names)
        {
            throw new NotImplementedException(); // TODO: CefX509CertPrincipal.GetOrganizationUnitNames
        }
        
    }
}
