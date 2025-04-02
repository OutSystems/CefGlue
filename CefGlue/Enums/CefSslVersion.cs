//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_ssl_version_t.
//
namespace Xilium.CefGlue
{
    /// <summary>
    /// Supported SSL version values. See net/ssl/ssl_connection_status_flags.h
    /// for more information.
    /// </summary>
    public enum CefSslVersion
    {
        /// <summary>
        /// Unknown SSL version.
        /// </summary>
        Unknown,
        Ssl2,
        Ssl3,
        Tls1,
        Tls1_1,
        Tls1_2,
        Tls1_3,
        Quic,
        NumValues,
    }
}
