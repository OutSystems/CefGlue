using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue
{
    /// <summary>
    /// Download interrupt reasons. Should be kept in sync with
    /// Chromium's download::DownloadInterruptReason type.
    /// </summary>
    public enum CefDownloadInterruptReason
    {
        None = 0,

        /// Generic file operation failure.
        FileFailed = 1,

        /// The file cannot be accessed due to security restrictions.
        FileAccessDenied = 2,

        /// There is not enough room on the drive.
        FileNoSpace = 3,

        /// The directory or file name is too long.
        FileNameTooLong = 5,

        /// The file is too large for the file system to handle.
        FileTooLarge = 6,

        /// The file contains a virus.
        FileVirusInfected = 7,

        /// The file was in use. Too many files are opened at once. We have run out of
        /// memory.
        FileTransientError = 10,

        /// The file was blocked due to local policy.
        FileBlocked = 11,

        /// An attempt to check the safety of the download failed due to unexpected
        /// reasons. See http://crbug.com/153212.
        FileSecurityCheckFailed = 12,

        /// An attempt was made to seek past the end of a file in opening
        /// a file (as part of resuming a previously interrupted download).
        FileTooShort = 13,

        /// The partial file didn't match the expected hash.
        FileHashMismatch = 14,

        /// The source and the target of the download were the same.
        FileSameAsSource = 15,

        // Network errors.

        /// Generic network failure.
        NetworkFailed = 20,

        /// The network operation timed out.
        NetworkTimeout = 21,

        /// The network connection has been lost.
        NetworkDisconnected = 22,

        /// The server has gone down.
        NetworkServerDown = 23,

        /// The network request was invalid. This may be due to the original URL or a
        /// redirected URL:
        /// - Having an unsupported scheme.
        /// - Being an invalid URL.
        /// - Being disallowed by policy.
        NetworkInvalidRequest = 24,

        // Server responses.

        /// The server indicates that the operation has failed (generic).
        ServerFailed = 30,

        /// The server does not support range requests.
        /// Internal use only:  must restart from the beginning.
        ServerNoRange = 31,

        /// The server does not have the requested data.
        ServerBadContent = 33,

        /// Server didn't authorize access to resource.
        ServerUnauthorized = 34,

        /// Server certificate problem.
        ServerCertProblem = 35,

        /// Server access forbidden.
        ServerForbidden = 36,

        /// Unexpected server response. This might indicate that the responding server
        /// may not be the intended server.
        ServerUnreachable = 37,

        /// The server sent fewer bytes than the content-length header. It may
        /// indicate that the connection was closed prematurely, or the Content-Length
        /// header was invalid. The download is only interrupted if strong validators
        /// are present. Otherwise, it is treated as finished.
        ServerContentLengthMismatch = 38,

        /// An unexpected cross-origin redirect happened.
        ServerCrossOriginRedirect = 39,

        // User input.

        /// The user canceled the download.
        UserCanceled = 40,

        /// The user shut down the browser.
        /// Internal use only:  resume pending downloads if possible.
        UserShutdown = 41,

        // Crash.

        /// The browser crashed.
        /// Internal use only:  resume pending downloads if possible.
        Crash = 50,
    }
}
