using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.BrowserProcess.Serialization
{
    internal class CefV8ValueEqualityComparer : IEqualityComparer<CefV8Value>
    {
        private static readonly CefV8ValueEqualityComparer _instance = new CefV8ValueEqualityComparer();

        private CefV8ValueEqualityComparer() { }

        public static CefV8ValueEqualityComparer Instance => _instance;

        public bool Equals(CefV8Value x, CefV8Value y)
        {
            return x.IsSame(y);
        }

        public int GetHashCode(CefV8Value obj)
        {
            return obj.GetHashCode();
        }
    }
}
