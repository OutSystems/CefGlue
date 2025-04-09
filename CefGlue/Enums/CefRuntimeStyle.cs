//
// This file manually written from cef/include/internal/cef_types_runtime.h.
// C API name: cef_runtime_style_t.
//
namespace Xilium.CefGlue
{
    using System;

    /// <summary>
    /// CEF supports both a Chrome runtime style (based on the Chrome UI layer) and
    /// an Alloy runtime style (based on the Chromium content layer). Chrome style
    /// provides the full Chrome UI and browser functionality whereas Alloy style
    /// provides less default browser functionality but adds additional client
    /// callbacks and support for windowless (off-screen) rendering. The style type
    /// is individually configured for each window/browser at creation time and
    /// different styles can be mixed during runtime. For additional comparative
    /// details on runtime styles see
    /// https://bitbucket.org/chromiumembedded/cef/wiki/Architecture.md#markdown-header-cef3
    ///
    /// Windowless rendering will always use Alloy style. Windowed rendering with a
    /// default window or client-provided parent window can configure the style via
    /// CefWindowInfo.runtime_style. Windowed rendering with the Views framework can
    /// configure the style via CefWindowDelegate::GetWindowRuntimeStyle and
    /// CefBrowserViewDelegate::GetBrowserRuntimeStyle. Alloy style Windows with the
    /// Views framework can host only Alloy style BrowserViews but Chrome style
    /// Windows can host both style BrowserViews. Additionally, a Chrome style
    /// Window can host at most one Chrome style BrowserView but potentially
    /// multiple Alloy style BrowserViews. See CefWindowInfo.runtime_style
    /// documentation for any additional platform-specific limitations.
    /// </summary>
    public enum CefRuntimeStyle
    {
        Default,
        Chrome,
        Alloy,
    }
}
