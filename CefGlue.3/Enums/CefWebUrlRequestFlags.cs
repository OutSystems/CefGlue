//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_weburlrequest_flags_t.
//
namespace Xilium.CefGlue
{
    using System;

    [Flags]
    public enum CefWebUrlRequestOptions
    {
        None = 0,
        SkipCache = 0x1,
        AllowCachedCredentials = 0x2,
        AllowCookies = 0x4,
        ReportUploadProgress = 0x8,
        ReportLoadTiming = 0x10,
        ReportRawHeaders = 0x20,
    }
}
