using System;

namespace Xilium.CefGlue
{
    public interface ICefListValue : IDisposable
    {
        int Count { get; }
        bool IsOwned { get; }
        bool IsReadOnly { get; }
        bool IsValid { get; }

        bool Clear();
        ICefListValue Copy();
        ICefBinaryValue GetBinary(int index);
        bool GetBool(int index);
        ICefDictionaryValue GetDictionary(int index);
        double GetDouble(int index);
        int GetInt(int index);
        ICefListValue GetList(int index);
        string GetString(int index);
        CefValue GetValue(int index);
        CefValueType GetValueType(int index);
        bool IsEqual(ICefListValue that);
        bool IsSame(ICefListValue that);
        bool Remove(int index);
        bool SetBinary(int index, ICefBinaryValue value);
        bool SetBool(int index, bool value);
        bool SetDictionary(int index, ICefDictionaryValue value);
        bool SetDouble(int index, double value);
        bool SetInt(int index, int value);
        bool SetList(int index, ICefListValue value);
        bool SetNull(int index);
        bool SetSize(int size);
        bool SetString(int index, string value);
        bool SetValue(int index, CefValue value);
    }
}