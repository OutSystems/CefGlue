using System;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class TypeMemberInfo
    {
        private readonly Action<object, object> _setter;

        public TypeMemberInfo(Type type, Action<object, object> setter = null)
        {
            Type = type;
            _setter = setter;
        }

        public Type Type { get; }

        public void SetValue(object obj, object value)
        {
            _setter(obj, value);
        }
    }
}
