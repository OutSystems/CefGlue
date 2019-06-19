using System.Collections.Generic;
using System.Linq;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class NativeObjectAnalyser
    {
        public static string[] AnalyseObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods();
            return methods.Select(m => m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1)).ToArray();
        }
    }
}
