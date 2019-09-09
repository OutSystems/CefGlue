using System;
using System.Collections.Generic;
using System.Reflection;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, MethodInfo> _methods;

        public NativeObject(string name, object target, IDictionary<string, MethodInfo> methods, JavascriptObjectMethodCallHandler methodHandler = null)
        {
            Name = name;
            Target = target;
            _methods = methods;
            MethodHandler = methodHandler;
        }

        public string Name { get; }

        public IEnumerable<string> MethodsNames => _methods.Keys;

        public JavascriptObjectMethodCallHandler MethodHandler { get; }

        public object Target { get; }

        public MethodInfo GetNativeMethod(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethod);
            return nativeMethod;
        }
    }
}
