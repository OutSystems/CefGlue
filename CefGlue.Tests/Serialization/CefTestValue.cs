using Xilium.CefGlue;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace CefGlue.Tests.Serialization
{
    internal class CefTestValue : CefValueWrapper

    {
        private object _value;
        private CefValueType _valueType;
        
        public override void SetNull()
        {
            _value = null;
            _valueType = CefValueType.Null;
        }

        public override void SetBool(bool value)
        {
            _value = value;
            _valueType = CefValueType.Bool;
        }

        public override void SetInt(int value)
        {
            _value = value;
            _valueType = CefValueType.Int;
        }

        public override void SetDouble(double value)
        {
            _value = value;
            _valueType = CefValueType.Double;
        }

        public override void SetString(string value)
        {
            _value = value;
            _valueType = CefValueType.String;
        }

        public override void SetBinary(ICefBinaryValue value)
        {
            _value = value;
            _valueType = CefValueType.Binary;
        }

        public override void SetList(ICefListValue value)
        {
            _value = value;
            _valueType = CefValueType.List;
        }

        public override void SetDictionary(ICefDictionaryValue value)
        {
            _value = value;
            _valueType = CefValueType.Dictionary;
        }

        public override bool GetBool()
        {
            return (bool) _value;
        }

        public override int GetInt()
        {
            return (int) _value;
        }

        public override double GetDouble()
        {
            return (double) _value;
        }

        public override string GetString()
        {
            return (string) _value;
        }

        public override ICefBinaryValue GetBinary()
        {
            return (ICefBinaryValue) _value;
        }

        public override ICefListValue GetList()
        {
            return (ICefListValue) _value;
        }

        public override ICefDictionaryValue GetDictionary()
        {
            return (ICefDictionaryValue) _value;
        }

        public override CefValueType GetValueType()
        {
            return _valueType;
        }
    }
}