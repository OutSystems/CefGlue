using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue.Common.Shared.Serialization.State;

namespace Xilium.CefGlue.Common.Shared.Helpers
{
    internal static class TypeExtensions
    {
        internal static bool IsCollection(this Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type) ||
                typeof(ICollection<>).IsAssignableFrom(type);
        }
    }
}
