using System;

namespace Xilium.CefGlue.Common.Helpers
{
    internal class MemberInfo
    {
        private readonly Action<object, object> _setter;

        public MemberInfo(string name, Type type, Action<object, object> setter)
        {
            Name = name;
            Type = type;
            _setter = setter;
        }

        public string Name { get; }

        public void SetValue(object obj, object value)
        {
            _setter(obj, value);
        } 

        public Type Type { get; }
    }
}
