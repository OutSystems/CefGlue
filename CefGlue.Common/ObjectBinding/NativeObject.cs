using System.Collections.Generic;

namespace Xilium.CefGlue.Common.ObjectBinding
{
    internal class NativeObject
    {
        private readonly IDictionary<string, string> _methods;

        public NativeObject(object target, IDictionary<string, string> methods)
        {
            Target = target;
            _methods = methods;
        }

        public object Target { get; }

        public string GetNativeMethodName(string javascriptMethodName)
        {
            _methods.TryGetValue(javascriptMethodName, out var nativeMethodName);
            return nativeMethodName;
        }
    }
}
