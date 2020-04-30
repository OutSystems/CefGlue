using System;
using System.Linq;

namespace Xilium.CefGlue.Common.Shared
{
    public class CustomScheme
    {
        private const string CommandLineSchemesSeparator = ";";
        private const string CommandLinePropertiesSeparator = "|";

        /// <summary>
        /// Gets or sets schema Name e.g. custom
        /// </summary>
        public string SchemeName { get; set; }

        /// <summary>
        /// Optional Domain Name. An empty value for a standard scheme
        /// will cause the factory to match all domain names. The |domain_name| value
        /// will be ignored for non-standard schemes.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// If true the scheme will be treated as a standard scheme.
        /// Standard schemes are subject to URL canonicalization and parsing rules as
        /// defined in the Common Internet Scheme Syntax RFC 1738 Section 3.1 available
        /// at http://www.ietf.org/rfc/rfc1738.txt
        /// In particular, the syntax for standard scheme URLs must be of the form:
        /// <pre>
        /// [scheme]://[username]:[password]@[host]:[port]/[url-path]
        /// </pre>
        /// Standard scheme URLs must have a host component that is a fully qualified
        /// domain name as defined in Section 3.5 of RFC 1034 [13] and Section 2.1 of
        /// RFC 1123. These URLs will be canonicalized to "scheme://host/path" in the
        /// simplest case and "scheme://username:password@host:port/path" in the most
        /// explicit case. For example, "scheme:host/path" and "scheme:///host/path"
        /// will both be canonicalized to "scheme://host/path". The origin of a
        /// standard scheme URL is the combination of scheme, host and port (i.e.,
        /// "scheme://host:port" in the most explicit case).
        /// For non-standard scheme URLs only the "scheme:" component is parsed and
        /// canonicalized. The remainder of the URL will be passed to the handler
        /// as-is. For example, "scheme:///some%20text" will remain the same.
        /// Non-standard scheme URLs cannot be used as a target for form submission.
        /// </summary>
        public bool IsStandard { get; set; }

        /// <summary>
        /// If true the scheme will be treated with the same security
        /// rules as those applied to "file" URLs. Normal pages cannot link to or
        /// access local URLs. Also, by default, local URLs can only perform
        /// XMLHttpRequest calls to the same URL (origin + path) that originated the
        /// request. To allow XMLHttpRequest calls from a local URL to other URLs with
        /// the same origin set the CefSettings.file_access_from_file_urls_allowed
        /// value to true. To allow XMLHttpRequest calls from a local URL to all
        /// origins set the CefSettings.universal_access_from_file_urls_allowed value
        /// to true.
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// If true the scheme can only be displayed from
        /// other content hosted with the same scheme. For example, pages in other
        /// origins cannot create iframes or hyperlinks to URLs with the scheme. For
        /// schemes that must be accessible from other schemes set this value to false,
        /// set |is_cors_enabled| to true, and use CORS "Access-Control-Allow-Origin"
        /// headers to further restrict access.
        /// </summary>
        public bool IsDisplayIsolated { get; set; }

        /// <summary>
        /// If true the scheme will be treated with the same security
        /// rules as those applied to "https" URLs. For example, loading this scheme
        /// from other secure schemes will not trigger mixed content warnings.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// If true the scheme can be sent CORS requests. This
        /// value should be true in most cases where |is_standard| is true.
        /// </summary>
        public bool IsCorsEnabled { get; set; }

        /// <summary>
        /// If true the scheme can bypass Content-Security-Policy
        /// (CSP) checks. This value should be false in most cases where |is_standard|
        /// is true.
        /// </summary>
        public bool IsCSPBypassing { get; set; }

        /// <summary>
        /// If ture the scheme can perform Fetch API requests. 
        /// </summary>
        public bool IsFetchEnabled { get; set; }

        /// <summary>
        /// Factory class that creates <see cref="CefResourceHandler"/> instances
        /// for handling current scheme requests.
        /// </summary>
        public CefSchemeHandlerFactory SchemeHandlerFactory { get; set; }

        /// <summary>
        /// Creates a new CustomScheme.
        /// </summary>
        public CustomScheme()
        {
            IsStandard = true;
            IsLocal = false;
            IsDisplayIsolated = false;
            IsSecure = true;
            IsCorsEnabled = true;
            IsCSPBypassing = false;
            IsFetchEnabled = true;
        }

        public CefSchemeOptions Options
        {
            get
            {
                var options = CefSchemeOptions.None;
                if (IsStandard)
                {
                    options |= CefSchemeOptions.Standard;
                }
                if (IsLocal)
                {
                    options |= CefSchemeOptions.Local;
                }
                if (IsSecure)
                {
                    options |= CefSchemeOptions.Secure;
                }
                if (IsDisplayIsolated)
                {
                    options |= CefSchemeOptions.DisplayIsolated;
                }
                if (IsCorsEnabled)
                {
                    options |= CefSchemeOptions.CorsEnabled;
                }
                if (IsCSPBypassing)
                {
                    options |= CefSchemeOptions.CspBypassing;
                }
                if (IsFetchEnabled)
                {
                    options |= CefSchemeOptions.FetchEnabled;
                }
                return options;
            }
        }

        private string SerializeToCommandLineValue()
        {
            return SchemeName + CommandLinePropertiesSeparator + DomainName + CommandLinePropertiesSeparator + ((int)Options).ToString();
        }

        private static CustomScheme DeserializeFromCommandLineValue(string value)
        {
            var tokens = value.Split(new string[] { CommandLinePropertiesSeparator }, StringSplitOptions.None);
            if (tokens.Length < 3)
            {
                return null;
            }

            Enum.TryParse<CefSchemeOptions>(tokens[2], out var properties);

            return new CustomScheme()
            {
                SchemeName = tokens[0],
                DomainName = tokens[1],
                IsStandard = properties.HasFlag(CefSchemeOptions.Standard),
                IsLocal = properties.HasFlag(CefSchemeOptions.Local),
                IsDisplayIsolated = properties.HasFlag(CefSchemeOptions.DisplayIsolated),
                IsSecure = properties.HasFlag(CefSchemeOptions.Secure),
                IsCorsEnabled = properties.HasFlag(CefSchemeOptions.CorsEnabled),
                IsCSPBypassing = properties.HasFlag(CefSchemeOptions.CspBypassing),
                IsFetchEnabled = properties.HasFlag(CefSchemeOptions.FetchEnabled)
            };
        }

        internal static string ToCommandLineValue(CustomScheme[] schemes)
        {
            return string.Join(CommandLineSchemesSeparator, schemes.Select(s => s.SerializeToCommandLineValue()));
        }

        internal static CustomScheme[] FromCommandLineValue(string value)
        {
            var schemes = value?.Split(new string[] { CommandLineSchemesSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (schemes == null)
            {
                return new CustomScheme[0];
            }
            return schemes.Select(DeserializeFromCommandLineValue).Where(s => s != null).ToArray();
        }
    }
}
