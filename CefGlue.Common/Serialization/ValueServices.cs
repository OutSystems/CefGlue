namespace Xilium.CefGlue.Common.Serialization
{
    public static class ValueServices
    {
        internal static IValueProvider ValueProxy { private get; set; } 

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
