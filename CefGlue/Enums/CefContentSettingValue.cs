using System;
using System.Collections.Generic;
using System.Text;

namespace Xilium.CefGlue
{
    public enum CefContentSettingValue
    {
        Default = 0,
        Allow,
        Block,
        Ask,
        SessionOnly,
        DetectImportantContent,

        NumValues
    }
}
