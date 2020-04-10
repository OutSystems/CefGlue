namespace Xilium.CefGlue.Common.Shared.Serialization
{
    public interface IValueProvider
    {
        ICefBinaryValue CreateBinary(byte[] data);
        ICefDictionaryValue CreateDictionary();
        ICefListValue CreateList();
    }
}
