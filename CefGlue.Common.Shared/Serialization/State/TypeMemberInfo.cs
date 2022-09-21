using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal record TypeMemberInfo
    {
        private readonly Action<object, object> _setter;

        public TypeMemberInfo(Type type, Action<object, object> setter = null)
        {
            Type = type;
            _setter = setter;
        }

        public Type Type { get; private set; }

        public void SetValue(object obj, object value)
        {
            _setter(obj, value);
        }
    }
}
