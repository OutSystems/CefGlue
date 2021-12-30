using System.Collections.Generic;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, NativeMethod> _methods;

        public NativeObject(string name, object target, IDictionary<string, NativeMethod> methods, JavascriptObjectMethodCallHandler methodHandler = null)
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

        public NativeMethod GetNativeMethod(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethod);
            return nativeMethod;
        }
    }
}
