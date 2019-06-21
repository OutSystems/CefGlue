using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Xilium.CefGlue.Common.Serialization
{
    internal static class CefValueSerialization
    {
        public enum BinaryMagicBytes : byte
        {
            DateTime,
            Binary
        }

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
                    var originalBinary = ((byte[])value);
                    cefValue.SetBinary(ToCefBinary(BinaryMagicBytes.Binary, originalBinary));
                    break;

                case TypeCode.Char:
                    cefValue.SetString(((char)value).ToString());
                    break;

                case TypeCode.DateTime:
                    // datetime is serialized into a binary (cef value does not support datetime)
                    var dateBinary = BitConverter.GetBytes(((DateTime)value).ToBinary());
                    cefValue.SetBinary(ToCefBinary(BinaryMagicBytes.DateTime, dateBinary));
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
            else if (value is IEnumerable enumerable)
            {
                var list = CefListValue.Create();
                var i = 0;
                foreach (var item in enumerable)
                {
                    list.SetValue(i++, Serialize(item, visitedObjects));
                }

                cefValue.SetList(list);
            }
            else
            {
                var fields = value.GetType().GetFields();
                var properties = value.GetType().GetProperties();

                var cefDictionary = CefDictionaryValue.Create();

                foreach (var field in fields)
                {
                    cefDictionary.SetValue(field.Name, Serialize(field.GetValue(value), visitedObjects));
                }

                foreach (var property in properties)
                {
                    cefDictionary.SetValue(property.Name, Serialize(property.GetValue(value), visitedObjects));
                }

                cefValue.SetDictionary(cefDictionary);
            }

            return cefValue;
        }

        public static object DeserializeCefValue(CefValue value)
        {
            switch (value.GetValueType())
            {
                case CefValueType.Binary:
                    return FromCefBinary(value.GetBinary(), out var kind);

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

        internal static CefBinaryValue ToCefBinary(BinaryMagicBytes kind, byte[] originalBinary)
        {
            var binary = new byte[originalBinary.Length + 1]; // alloc space for the magic byte
            binary[0] = (byte)kind;
            originalBinary.CopyTo(binary, 1);

            return CefBinaryValue.Create(binary);
        }

        internal static object FromCefBinary(CefBinaryValue value, out BinaryMagicBytes kind)
        {
            var binary = value.ToArray();
            if (binary.Length > 0)
            {
                var rest = binary.Skip(1).ToArray();
                kind = (BinaryMagicBytes)binary[0];
                switch (kind)
                {
                    case BinaryMagicBytes.Binary:
                        return rest;

                    case BinaryMagicBytes.DateTime:
                        var binaryDate = BitConverter.ToInt64(rest, 0);
                        return DateTime.FromBinary(binaryDate);

                    default:
                        throw new InvalidOperationException("Unrecognized binary type: " + binary[0]);
                }
            }

            kind = BinaryMagicBytes.Binary;
            return new byte[0];
        }
    }
}
