using System.Collections.Generic;
using Xilium.CefGlue.Common.Events;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, NativeMethod> _methods;

        public NativeObject(string name, object target, IDictionary<string, NativeMethod> methods, (NativeMethod, MethodCallHandler) methodHandler = default)
        {
            Name = name;
            Target = target;
            _methods = methods;
            MethodHandlerInfo = methodHandler.Item1;
            MethodHandler = methodHandler.Item2;
        }

        public string Name { get; }

        public IEnumerable<string> MethodsNames => _methods.Keys;

        public NativeMethod MethodHandlerInfo { get; }
        
        public MethodCallHandler MethodHandler { get; }

        public object Target { get; }

        public NativeMethod GetNativeMethod(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethod);
            return nativeMethod;
        }
    }
}
