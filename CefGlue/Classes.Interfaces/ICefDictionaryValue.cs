using System;
using Xilium.CefGlue.Interop;

namespace Xilium.CefGlue
{
    public unsafe interface ICefDictionaryValue : IDisposable
    {
        int Count { get; }
        bool IsOwned { get; }
        bool IsReadOnly { get; }
        bool IsValid { get; }

        bool Clear();
        ICefDictionaryValue Copy(bool excludeEmptyChildren);
        ICefBinaryValue GetBinary(string key);
        bool GetBool(string key);
        ICefDictionaryValue GetDictionary(string key);
        double GetDouble(string key);
        int GetInt(string key);
        string[] GetKeys();
        ICefListValue GetList(string key);
        string GetString(string key);
        CefValue GetValue(string key);
        CefValueType GetValueType(string key);
        bool HasKey(string key);
        bool IsEqual(ICefDictionaryValue that);
        bool IsSame(ICefDictionaryValue that);
        bool Remove(string key);
        bool SetBinary(string key, ICefBinaryValue value);
        bool SetBool(string key, bool value);
        bool SetDictionary(string key, ICefDictionaryValue value);
        bool SetDouble(string key, double value);
        bool SetInt(string key, int value);
        bool SetList(string key, ICefListValue value);
        bool SetNull(string key);
        bool SetString(string key, string value);
        bool SetValue(string key, CefValue value);
    }
}