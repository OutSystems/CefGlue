//
// This file manually written from cef/include/internal/cef_types_runtime.h.
// C API name: cef_runtime_style_t.
//
namespace Xilium.CefGlue
{
    public enum CefRuntimeStyle
    {
        ///
        /// Use the default runtime style. The default style will match the
        /// CefSettings.chrome_runtime value in most cases. See above documentation
        /// for exceptions.
        ///
        Default,

        ///
        /// Use the Chrome runtime style. Only supported with the Chrome runtime.
        ///
        Chrome,

        ///
        /// Use the Alloy runtime style. Supported with both the Alloy and Chrome
        /// runtime.
        ///
        Alloy
    }
}
