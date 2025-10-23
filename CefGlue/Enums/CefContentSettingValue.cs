//
// This file manually written from cef/include/internal/cef_types_content_settings.h.
// C API name: cef_content_setting_values_t.
//
using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue
{
    public enum CefContentSettingValue
    {
        Default,
        Allow,
        Block,
        Ask,
        SessionOnly,
        DetectImportantContent,
        NumValues,
    }
}
