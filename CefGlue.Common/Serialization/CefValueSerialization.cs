using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Xilium.CefGlue.Common.Serialization
{
    internal static class CefValueSerialization
    {
        public static CefValue Serialize(object value)
        {
            return Serialize(value, new Stack<object>());
        }

        private static CefValue Serialize(object value, Stack<object> visitedObjects)
        {
            if (value == null)
            {
                var nullCefValue = CefValue.Create();
                nullCefValue.SetNull();
                return nullCefValue;
            }

            var typeCode = Type.GetTypeCode(value.GetType());

            if (typeCode == TypeCode.Object)
            {
                if (visitedObjects.Any(o => o == value))
                {
                    throw new InvalidOperationException("Cycle found in result");
                }

                visitedObjects.Push(value);

                var result = SerializeComplexObject(value, visitedObjects);

                visitedObjects.Pop();

                return result;
            }

            var cefValue = CefValue.Create();

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    cefValue.SetBool((bool)value);
                    break;
                case TypeCode.Byte:
                    // TODO
                    break;
                case TypeCode.Char:
                    cefValue.SetString(((char)value).ToString());
                    break;
                case TypeCode.DateTime:
                    // TODO
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    cefValue.SetDouble((double)value);
                    break;
                case TypeCode.Empty:
                    cefValue.SetNull();
                    break;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    cefValue.SetInt((int)value);
                    break;
                case TypeCode.SByte:
                    cefValue.SetInt((sbyte)value);
                    break;
                case TypeCode.String:
                    cefValue.SetString((string)value);
                    break;
            }

            return cefValue;
        }

        private static CefValue SerializeComplexObject(object value, Stack<object> visitedObjects)
        {
            var cefValue = CefValue.Create();

            if (value is IDictionary dictionary)
            {
                var cefDictionary = CefDictionaryValue.Create();

                foreach (var key in dictionary.Keys)
                {
                    var keyText = key.ToString();
                    cefDictionary.SetValue(keyText, Serialize(dictionary[key], visitedObjects));
                }

                cefValue.SetDictionary(cefDictionary);
            }

            if (value is IEnumerable enumerable)
            {
                var list = CefListValue.Create();
                var i = 0;
                foreach (var item in enumerable)
                {
                    list.SetValue(i++, Serialize(item, visitedObjects));
                }

                cefValue.SetList(list);
            }

            return cefValue;
        }

        public static object DeserializeCefValue(CefValue value)
        {
            switch (value.GetValueType())
            {
                case CefValueType.Binary:
                    var binaryDate = BitConverter.ToInt64(value.GetBinary().ToArray(), 0);
                    return DateTime.FromBinary(binaryDate);

                case CefValueType.Bool:
                    return value.GetBool();

                case CefValueType.Dictionary:
                    IDictionary<string, object> dictionary = new ExpandoObject();
                    var v8Dictionary = value.GetDictionary();
                    var keys = v8Dictionary.GetKeys();
                    foreach (var key in keys)
                    {
                        dictionary[key] = DeserializeCefValue(v8Dictionary.GetValue(key));
                    }
                    return dictionary;

                case CefValueType.Double:
                    return value.GetDouble();

                case CefValueType.List:
                    return DeserializeCefList<object>(value.GetList());

                case CefValueType.Int:
                    return value.GetInt();

                case CefValueType.String:
                    return value.GetString();

                case CefValueType.Null:
                    return null;
            }

            return null;
        }

        public static ListElementType[] DeserializeCefList<ListElementType>(CefListValue cefList)
        {
            var array = new ListElementType[cefList.Count];
            for (var i = 0; i < cefList.Count; i++)
            {
                array[i] = (ListElementType)DeserializeCefValue(cefList.GetValue(i));
            }
            return array;
        }
    }
}
