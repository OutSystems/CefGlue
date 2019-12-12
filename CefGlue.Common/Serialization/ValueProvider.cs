using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Serialization
{
    public class ValueProvider : IValueProvider
    {
        public ICefBinaryValue CreateBinary(byte[] data)
        {
            return CefBinaryValue.Create(data);
        }

        public ICefDictionaryValue CreateDictionary()
        {
            return CefDictionaryValue.Create();
        }

        public ICefListValue CreateList()
        {
            return CefListValue.Create();
        }
    }
}
