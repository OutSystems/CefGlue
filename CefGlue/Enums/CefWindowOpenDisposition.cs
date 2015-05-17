//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_window_open_disposition_t.
//
namespace Xilium.CefGlue
{
    /// <summary>
    /// The manner in which a link click should be opened.
    /// </summary>
    public enum CefWindowOpenDisposition
    {
        Unknown,
        SuppressOpen,
        CurrentTab,
        SingletonTab,
        NewForegroundTab,
        NewBackgroundTab,
        NewPopup,
        NewWindow,
        SaveToDisk,
        OffTheRecord,
        IgnoreAction,
    }
}
