using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class CefValueSerialization
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters = { new StringJsonConverter(), new DateTimeJsonConverter() },
            IncludeFields = true
        };
        
        public static void Serialize(object value, CefValueWrapper cefValue)
        {
            if (value == null)
            {
                cefValue.SetNull();
                return;
            }

            if (value is byte[] byteArr)
            {
                // handle binaries in a special way (otherwise it will fall on object and be serialized as a collection)
                using (var cefBinary = ToCefBinary(DataMarkers.BinaryMagicBytes.Binary, byteArr))
                {
                    cefValue.SetBinary(cefBinary);
                }
                return;
            }

            var typeCode = Type.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                case TypeCode.String: // string and objects are handled as json
                    SerializeComplexObject(value, cefValue);
                    break;

                case TypeCode.Boolean:
                    cefValue.SetBool((bool)value);
                    break;

                case TypeCode.Byte:
                    cefValue.SetInt((byte)value);
                    break;

                case TypeCode.Char:
                    cefValue.SetString(((char)value).ToString());
                    break;
                
                case TypeCode.Decimal:
                    cefValue.SetDouble((double)(decimal)value);
                    break;

                case TypeCode.Double:
                    cefValue.SetDouble((double)value);
                    break;

                case TypeCode.Single:
                    cefValue.SetDouble((float)value);
                    break;

                case TypeCode.Empty:
                    cefValue.SetNull();
                    break;

                case TypeCode.Int16:
                    cefValue.SetInt((short)value);
                    break;

                case TypeCode.Int32:
                    cefValue.SetInt((int)value);
                    break;

                case TypeCode.Int64:
                    cefValue.SetDouble((long)value);
                    break;

                case TypeCode.UInt16:
                    cefValue.SetInt((ushort)value);
                    break;

                case TypeCode.UInt32:
                    cefValue.SetInt((int)(uint)value);
                    break;

                case TypeCode.UInt64:
                    cefValue.SetDouble((ulong)value);
                    break;

                case TypeCode.SByte:
                    cefValue.SetInt((sbyte)value);
                    break;
            }
        }

        private static void SerializeComplexObject(object value, CefValueWrapper cefValue)
        {
            var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            cefValue.SetString(json);
        }

        public static object DeserializeCefValue(CefValueWrapper cefValue)
        {
            switch (cefValue.GetValueType())
            {
                case CefValueType.Binary:
                    using (var binary = cefValue.GetBinary())
                    {
                        return FromCefBinary(binary, out var kind);
                    }

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
                        return dictionary;
                    }

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

        public static ListElementType[] DeserializeCefList<ListElementType>(ICefListValue cefList)
        {
            var array = new ListElementType[cefList.Count];
            for (var i = 0; i < cefList.Count; i++)
            {
                array[i] = (ListElementType)DeserializeCefValue(new CefListWrapper(cefList, i));
            }
            return array;
        }

        internal static ICefBinaryValue ToCefBinary(DataMarkers.BinaryMagicBytes kind, byte[] originalBinary)
        {
            var binary = new byte[originalBinary.Length + 1]; // alloc space for the magic byte
            binary[0] = (byte)kind;
            originalBinary.CopyTo(binary, 1);

            return ValueServices.CreateBinary(binary);
        }

        internal static object FromCefBinary(ICefBinaryValue value, out DataMarkers.BinaryMagicBytes kind)
        {
            var binary = value.ToArray();
            if (binary.Length > 0)
            {
                var rest = binary.Skip(1).ToArray();
                kind = (DataMarkers.BinaryMagicBytes)binary[0];
                switch (kind)
                {
                    case DataMarkers.BinaryMagicBytes.Binary:
                        return rest;

                    case DataMarkers.BinaryMagicBytes.DateTime:
                        var binaryDate = BitConverter.ToInt64(rest, 0);
                        return DateTime.FromBinary(binaryDate);

                    default:
                        throw new InvalidOperationException("Unrecognized binary type: " + binary[0]);
                }
            }

            kind = DataMarkers.BinaryMagicBytes.Binary;
            return new byte[0];
        }
    }
}
