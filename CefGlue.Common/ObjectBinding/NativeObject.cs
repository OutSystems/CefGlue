using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, MethodInfo> _methods;

        public NativeObject(object target, IDictionary<string, MethodInfo> methods)
        {
            Target = target;
            _methods = methods;
        }

        public object Target { get; }

        public MethodInfo GetNativeMethod(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethod);
            return nativeMethod;
        }
    }
}
