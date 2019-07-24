using System;
using System.Collections.Generic;
using System.Reflection;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, MethodInfo> _methods;

        public NativeObject(object target, IDictionary<string, MethodInfo> methods, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            Target = target;
            _methods = methods;
            MethodHandler = methodHandler;
        }

        public object Target { get; }

        public MethodInfo GetNativeMethod(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethod);
            return nativeMethod;
        }

        public JavascriptObjectMethodCallHandler MethodHandler { get; }
    }
}
