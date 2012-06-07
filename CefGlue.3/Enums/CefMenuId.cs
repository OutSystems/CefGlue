//
// This file manually written from cef/include/internal/cef_types.h.
// C API name: cef_menu_id_t.
//
namespace Xilium.CefGlue
{
    /// <summary>
    /// Supported menu IDs. Non-English translations can be provided for the
    /// IDS_MENU_* strings in CefResourceBundleHandler::GetLocalizedString().
    /// </summary>
    public enum CefMenuId
    {
        // Navigation.
        Back = 100,
        Forward = 101,
        Reload = 102,
        ReloadNoCache = 103,
        StopLoad = 104,

        // Editing.
        Undo = 110,
        Redo = 111,
        Cut = 112,
        Copy = 113,
        Paste = 114,
        Delete = 115,
        SelectAll = 116,

        // Miscellaneous.
        Find = 130,
        Print = 131,
        ViewSource = 132,

        // All user-defined menu IDs should come between MENU_ID_USER_FIRST and
        // MENU_ID_USER_LAST to avoid overlapping the Chromium and CEF ID ranges
        // defined in the tools/gritsettings/resource_ids file.
        UserFirst = 26500,
        UserLast = 28500,
    }
}
