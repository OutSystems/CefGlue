//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_uri_unescape_rule_t.
//
namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// URI unescape rules passed to CefURIDecode().
    /// </summary>
    [Flags]
    public enum CefUriUnescapeRules
    {
        /// <summary>
        /// Don't unescape anything at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Don't unescape anything special, but all normal unescaping will happen.
        /// This is a placeholder and can't be combined with other flags (since it's
        /// just the absence of them). All other unescape rules imply "normal" in
        /// addition to their special meaning. Things like escaped letters, digits,
        /// and most symbols will get unescaped with this mode.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Convert %20 to spaces. In some places where we're showing URLs, we may
        /// want this. In places where the URL may be copied and pasted out, then
        /// you wouldn't want this since it might not be interpreted in one piece
        /// by other applications.
        /// </summary>
        Spaces = 2,

        /// <summary>
        /// Unescapes various characters that will change the meaning of URLs,
        /// including '%', '+', '&', '/', '#'. If we unescaped these characters, the
        /// resulting URL won't be the same as the source one. This flag is used when
        /// generating final output like filenames for URLs where we won't be
        /// interpreting as a URL and want to do as much unescaping as possible.
        /// </summary>
        UrlSpecialChars = 4,

        /// <summary>
        /// Unescapes control characters such as %01. This INCLUDES NULLs. This is
        /// used for rare cases such as data: URL decoding where the result is binary
        /// data. This flag also unescapes BiDi control characters.
        ///
        /// DO NOT use CONTROL_CHARS if the URL is going to be displayed in the UI
        /// for security reasons.
        /// </summary>
        ControlChars = 8,

        /// <summary>
        /// URL queries use "+" for space. This flag controls that replacement.
        /// </summary>
        ReplacePlusWithSpace = 16,
    }
}
