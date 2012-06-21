//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_urlrequest_flags_t.
//
namespace Xilium.CefGlue
{
    using System;

    [Flags]
    public enum CefUrlRequestOptions
    {
        /// <summary>
        /// Default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// If set the cache will be skipped when handling the request.
        /// </summary>
        SkipCache = 1 << 0,

        /// <summary>
        /// If set user name, password, and cookies may be sent with the request.
        /// </summary>
        AllowCachedCredentials = 1 << 1,

        /// <summary>
        /// If set cookies may be sent with the request and saved from the response.
        /// <c>AllowCachedCredentials</c> must also be set.
        /// </summary>
        AllowCookies = 1 << 2,

        /// <summary>
        /// If set upload progress events will be generated when a request has a body.
        /// </summary>
        ReportUploadProgress = 1 << 3,

        /// <summary>
        /// If set load timing info will be collected for the request.
        /// </summary>
        ReportLoadTiming = 1 << 4,

        /// <summary>
        /// If set the headers sent and received for the request will be recorded.
        /// </summary>
        ReportRawHeaders = 1 << 5,

        /// <summary>
        /// If set the <c>CefUrlRequestClient.OnDownloadData</c> method will not be called.
        /// </summary>
        NoDownloadData = 1 << 6,

        /// <summary>
        /// If set 5XX redirect errors will be propagated to the observer instead of
        /// automatically re-tried. This currently only applies for requests
        /// originated in the browser process.
        /// </summary>
        NoRetryOn5XX = 1 << 7,
    }
}
