namespace Xilium.CefGlue.Common.Shared.Serialization
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
