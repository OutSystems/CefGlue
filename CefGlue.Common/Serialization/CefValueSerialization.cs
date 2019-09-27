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

        public static void Serialize(object value, CefValueWrapper cefValue)
        {
            Serialize(value, new Stack<object>(), cefValue);
        }

        private static void Serialize(object value, Stack<object> visitedObjects, CefValueWrapper cefValue)
        {
            if (value == null)
            {
                cefValue.SetNull();
                return;
            }

            var typeCode = Type.GetTypeCode(value.GetType());

            if (typeCode == TypeCode.Object)
            {
                if (visitedObjects.Any(o => o == value))
                {
                    throw new InvalidOperationException("Cycle found in result");
                }

                visitedObjects.Push(value);

                SerializeComplexObject(value, visitedObjects, cefValue);

                visitedObjects.Pop();

                return;
            }

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
        }

        private static void SerializeComplexObject(object value, Stack<object> visitedObjects, CefValueWrapper cefValue)
        {
            if (value is IDictionary dictionary)
            {
                var cefDictionary = CefDictionaryValue.Create();

                foreach (var key in dictionary.Keys)
                {
                    var keyText = key.ToString();
                    Serialize(dictionary[key], visitedObjects, new CefDictionaryWrapper(cefDictionary, keyText));
                }

                cefValue.SetDictionary(cefDictionary);
            }
            else if (value is IEnumerable enumerable)
            {
                var i = 0;
                var cefList = CefListValue.Create();
                
                foreach (var item in enumerable)
                {
                    Serialize(item, visitedObjects, new CefListWrapper(cefList, i));
                    i++;
                }

                cefValue.SetList(cefList);
            }
            else
            {
                var fields = value.GetType().GetFields();
                var properties = value.GetType().GetProperties();

                var cefDictionary = CefDictionaryValue.Create();

                foreach (var field in fields)
                {
                    Serialize(field.GetValue(value), visitedObjects, new CefDictionaryWrapper(cefDictionary, field.Name));
                }

                foreach (var property in properties)
                {
                    Serialize(property.GetValue(value), visitedObjects, new CefDictionaryWrapper(cefDictionary, property.Name));
                }

                cefValue.SetDictionary(cefDictionary);
            }
        }

        public static object DeserializeCefValue(CefValueWrapper cefValue)
        {
            switch (cefValue.GetValueType())
            {
                case CefValueType.Binary:
                    return FromCefBinary(cefValue.GetBinary(), out var kind);

                case CefValueType.Bool:
                    return cefValue.GetBool();

                case CefValueType.Dictionary:
                    IDictionary<string, object> dictionary = new ExpandoObject();
                    using (var cefDictionary = cefValue.GetDictionary())
                    {
                        var keys = cefDictionary.GetKeys();
                        foreach (var key in keys)
                        {
                            dictionary[key] = DeserializeCefValue(new CefDictionaryWrapper(cefDictionary, key));
                        }
                    }
                    return dictionary;

                case CefValueType.Double:
                    return cefValue.GetDouble();

                case CefValueType.List:
                    using (var cefList = cefValue.GetList())
                    {
                        return DeserializeCefList<object>(cefList);
                    }

                case CefValueType.Int:
                    return cefValue.GetInt();

                case CefValueType.String:
                    return cefValue.GetString() ?? ""; // default to "", because cef converts "" to null, and when null it will fall on the Null case

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
                array[i] = (ListElementType)DeserializeCefValue(new CefListWrapper(cefList, i));
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
