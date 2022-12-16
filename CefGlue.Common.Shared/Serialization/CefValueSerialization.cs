﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    // TODO - bcs - rename file
    internal static class CefValueSerialization
    {
        private const int SerializerMaxDepth = int.MaxValue;
        
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new StringJsonConverter(),
                new DateTimeJsonConverter(),
                new BinaryJsonConverter()
            },
            IncludeFields = true,
            MaxDepth = SerializerMaxDepth,
            ReferenceHandler = ReferenceHandler.Preserve,
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

        internal static string SerializeAsJson(object value)
        {
            try
            {
                return JsonSerializer.Serialize(value, _jsonSerializerOptions);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
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
            cefValue.SetString(SerializeAsJson(value));
        }

        internal static object DeserializeCefValue(CefValueWrapper cefValue)
        {
            return DeserializeCefValue(cefValue, null);
        }

        private static object DeserializeCefValue(CefValueWrapper cefValue, IReferencesResolver<object> referencesResolver = null)
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
                    referencesResolver = referencesResolver ?? new ReferencesResolver<object>();

                    using (var cefDictionary = cefValue.GetDictionary())
                    {
                        IDictionary<string, object> dictionary = null;
                        var keys = cefDictionary.GetKeys();

                        if (keys.Length > 0)
                        {
                            switch (keys.First())
                            {
                                case JsonAttributes.Ref:
                                    return referencesResolver.ResolveReference(cefDictionary.GetString(JsonAttributes.Ref));
                                case JsonAttributes.Id:
                                    var id = cefDictionary.GetString(JsonAttributes.Id);

                                    // it's a list
                                    if (keys.Length == 2 && keys.Last() == JsonAttributes.Values)
                                    {
                                        var cefList = cefDictionary.GetList(JsonAttributes.Values);
                                        return DeserializeCefList<object>(cefList, id, referencesResolver);
                                    }

                                    dictionary = new ExpandoObject();
                                    referencesResolver.AddReference(id, dictionary);
                                    break;
                                default:
                                    dictionary = new ExpandoObject();
                                    break;
                            }
                            foreach (var key in keys.Skip(1))
                            {
                                dictionary[key] = DeserializeCefValue(new CefDictionaryWrapper(cefDictionary, key), referencesResolver);
                            }
                        }
                        return dictionary;
                    }

                case CefValueType.Double:
                    return cefValue.GetDouble();

                case CefValueType.List:
                    using (var cefList = cefValue.GetList())
                    {
                        return DeserializeCefList<object>(cefList, referencesResolver);
                    }

                case CefValueType.Int:
                    return cefValue.GetInt();

                case CefValueType.String:
                    var value = cefValue.GetString() ?? ""; // default to "", because cef converts "" to null, and when null it will fall on the Null case
                    if (string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                    return DeserializeFromJson(value);

                case CefValueType.Null:
                    return null;
            }

            return null;
        }

        private static object DeserializeFromJson(string value)
        {
            return JsonDeserializer.Deserialize<object>(value);
        }

        internal static TListElementType[] DeserializeCefList<TListElementType>(ICefListValue cefList, IReferencesResolver<object> referencesResolver = null)
        {
            return DeserializeCefList<TListElementType>(cefList, null, referencesResolver);
        }

        private static TListElementType[] DeserializeCefList<TListElementType>(ICefListValue cefList, string id, IReferencesResolver<object> referencesResolver)
        {
            var array = new TListElementType[cefList.Count];
            
            if (!string.IsNullOrEmpty(id) && referencesResolver != null)
            {
                referencesResolver.AddReference(id, array);
            }

            for (var i = 0; i < cefList.Count; i++)
            {
                array[i] = (TListElementType)DeserializeCefValue(new CefListWrapper(cefList, i), referencesResolver);
            }

            return array;
        }
    }
}
