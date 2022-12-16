using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal record TypeMethodInfo
    {
        private readonly Action<object, object[]> _action;

        public TypeMethodInfo(Action<object, object[]> action = null)
        {
            _action = action;
        }

        public void Invoke(object obj, object[] parameters)
        {
            _action(obj, parameters);
        }
    }
}
