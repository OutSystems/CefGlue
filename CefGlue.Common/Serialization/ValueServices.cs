using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xilium.CefGlue.Common.Serialization
{
    public interface IValueProxy
    {
        ICefBinaryValue CreateBinary(byte[] data);
        ICefDictionaryValue CreateDictionary();
        ICefListValue CreateList();
    }

    public class ValueProvider : IValueProxy
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

    public static class ValueServices
    {
        internal static IValueProxy ValueProxy { private get; set; } 
        static ValueServices()
        {
            Reset();
        }
        internal static void Reset()
        {
            ValueProxy = new ValueProvider();
        }
        public static ICefBinaryValue CreateBinary(byte[] data)
        {
            return ValueProxy.CreateBinary(data);
        }
        public static ICefDictionaryValue CreateDictionary()
        {
            return ValueProxy.CreateDictionary();
        }
        public static ICefListValue CreateList()
        {
            return ValueProxy.CreateList();
        }
    }
}
