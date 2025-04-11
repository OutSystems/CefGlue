//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_termination_status_t.
//

namespace Xilium.CefGlue
{
    /// <summary>
    /// Process termination status values.
    /// </summary>
    public enum CefTerminationStatus
    {
        /// <summary>
        /// Non-zero exit status.
        /// </summary>
        AbnormalTermination,

        /// <summary>
        /// SIGKILL or task manager kill.
        /// </summary>
        ProcessWasKilled,

        /// <summary>
        /// Segmentation fault.
        /// </summary>
        ProcessCrashed,

        /// <summary>
        /// Out of memory. Some platforms may use PROCESS_CRASHED instead.
        /// </summary>
        ProcessOom,

        /// <summary>
        /// Child process never launched.
        /// </summary>
        LaunchFailed,

        /// <summary>
        /// On Windows, the OS terminated the process due to code integrity failure.
        /// </summary>
        IntegrityFailure,
        NumValues
    }
}
