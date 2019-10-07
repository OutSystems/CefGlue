using System;
using Xilium.CefGlue;
using Xilium.CefGlue.Common.Serialization;

namespace CefGlue.Tests.Serialization
{
    internal class TestCefValueWrapper : CefValueWrapper
    {
        private object value;
        private bool isString;

        public TestCefValueWrapper(object value = null)
        {
            this.value = value;
        }

        public static TestCefValueWrapper NullString
        {
            get
            {
                var result = new TestCefValueWrapper();
                result.isString = true;
                return result;
            }
        }

        public override CefBinaryValue GetBinary()
        {
            return (CefBinaryValue)value;
        }

        public override bool GetBool()
        {
            return (bool)value;
        }

        public override CefDictionaryValue GetDictionary()
        {
            return (CefDictionaryValue)value;
        }

        public override double GetDouble()
        {
            return (double)value;
        }

        public override int GetInt()
        {
            return (int)value;
        }

        public override CefListValue GetList()
        {
            return (CefListValue)value;
        }

        public override string GetString()
        {
            return (string)value;
        }

        public override CefValueType GetValueType()
        {
            if (isString)
            {
                return CefValueType.String;
            }

            if (value == null)
            {
                return CefValueType.Null;
            }

            var type = Type.GetTypeCode(value.GetType());
            switch (type)
            {
                case TypeCode.Boolean:
                    return CefValueType.Bool;

                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return CefValueType.Int;

                case TypeCode.String:
                    return CefValueType.String;

                case TypeCode.Double:
                    return CefValueType.Double;

                default:
                    throw new InvalidOperationException("Missing case " + type);
            }
        }

        public override void SetBinary(CefBinaryValue value)
        {
            this.value = value;
        }

        public override void SetBool(bool value)
        {
            this.value = value;
        }

        public override void SetDictionary(CefDictionaryValue value)
        {
            this.value = value;
        }

        public override void SetDouble(double value)
        {
            this.value = value;
        }

        public override void SetInt(int value)
        {
            this.value = value;
        }

        public override void SetList(CefListValue value)
        {
            this.value = value;
        }

        public override void SetNull()
        {
            this.value = null;
        }

        public override void SetString(string value)
        {
            this.value = value;
        }
    }
}
