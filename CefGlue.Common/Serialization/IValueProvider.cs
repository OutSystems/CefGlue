using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Serialization
{
    public interface IValueProvider
    {
        ICefBinaryValue CreateBinary(byte[] data);
        ICefDictionaryValue CreateDictionary();
        ICefListValue CreateList();
    }
}
