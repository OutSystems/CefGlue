//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_log_severity_t.
//
namespace Xilium.CefGlue
{
    /// <summary>
    /// Log severity levels.
    /// </summary>
    public enum CefLogSeverity
    {
        Verbose = -1,
        Info,
        Warning,
        Error,
        ErrorReport,

        /// <summary>
        /// Disables logging completely.
        /// </summary>
        Disable = 99,
    }
}
