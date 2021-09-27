using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class NativeObjectAnalyser
    {
        public static IDictionary<string, MethodInfo> AnalyseObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => !m.IsSpecialName);
            return methods.Select(m => new { Member = m, JavascriptName = m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1) })
                          .ToDictionary(m => m.JavascriptName, m => m.Member);
        }

    }
}
