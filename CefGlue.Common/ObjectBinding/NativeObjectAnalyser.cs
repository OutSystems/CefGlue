using System.Collections.Generic;
using System.Linq;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class NativeObjectAnalyser
    {
        public static IDictionary<string, string> AnalyseObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods();
            return methods.Select(m => new { NativeName = m.Name, JavascriptName = m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1) })
                          .ToDictionary(m => m.JavascriptName, m => m.NativeName);
        }

    }
}
