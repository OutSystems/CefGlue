using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal static class NativeObjectAnalyser
    {
        public static IDictionary<string, NativeMethod> AnalyseObjectMembers(object obj)
        {
            var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => !m.IsSpecialName);
            return methods.Select(m => new { Member = m, JavascriptName = m.Name.Substring(0, 1).ToLowerInvariant() + m.Name.Substring(1) })
                          .ToDictionary(m => m.JavascriptName, m => new NativeMethod(m.Member, GetAsyncResultGetter(m.Member)));
        }

        /// <summary>
        /// Create an helper function to obtain the result from Tasks.
        /// </summary>
        private static Func<Task, object> GetAsyncResultGetter(MethodInfo method)
        {
            var returnType = method.ReturnType;
            if (typeof(Task).IsAssignableFrom(returnType))
            {
                if (returnType.IsGenericType)
                {
                    // generic Task<T>
                    var resultGetter = returnType.GetProperty(nameof(Task<object>.Result));
                    return (task) => resultGetter.GetValue(task);
                }
                
                // non-generic Task
                return (task) => null;
            }

            return null;
        }
    }
}
