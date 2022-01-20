using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class CefValueSerialization
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new StringJsonConverter(), 
                new DateTimeJsonConverter(), 
                new BinaryJsonConverter()
            },
            IncludeFields = true
        };
        
        private static readonly JsonSerializerOptions _jsonDeserializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new ObjectJsonConverter()
            }
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
                SerializeAsJson(byteArr, cefValue);
                return;
            }

            var typeCode = Type.GetTypeCode(value.GetType());

            switch (typeCode)
            {
                case TypeCode.Object:
                case TypeCode.DateTime:
                case TypeCode.String:
                    // string, datetime, and object are handled as json
                    SerializeAsJson(value, cefValue);
                    break;

                case TypeCode.Boolean:
                    cefValue.SetBool((bool)value);
                    break;

                case TypeCode.Byte:
                    cefValue.SetInt((byte)value);
                    break;

                case TypeCode.Char:
                    SerializeAsJson(((char)value).ToString(), cefValue);
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

        /// <summary>
        /// Using JSON serialization is usually faster than using CefList and CefDictionary.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cefValue"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private static void SerializeAsJson(object value, CefValueWrapper cefValue)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
                cefValue.SetString(json);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }

        public static object DeserializeCefValue(CefValueWrapper cefValue)
        {
            switch (cefValue.GetValueType())
            {
                case CefValueType.Binary:
                    using (var binary = cefValue.GetBinary())
                    {
                        return binary.ToArray();
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
                    var value = cefValue.GetString() ?? ""; // default to "", because cef converts "" to null, and when null it will fall on the Null case
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                    return DeserializeAsJson(value); 

                case CefValueType.Null:
                    return null;
            }

            return null;
        }
        
        private static object DeserializeAsJson(string value)
        {
            try
            {
                return JsonSerializer.Deserialize<object>(value, _jsonDeserializerOptions);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }

        public static TListElementType[] DeserializeCefList<TListElementType>(ICefListValue cefList)
        {
            var array = new TListElementType[cefList.Count];
            for (var i = 0; i < cefList.Count; i++)
            {
                array[i] = (TListElementType)DeserializeCefValue(new CefListWrapper(cefList, i));
            }
            return array;
        }
    }
}
