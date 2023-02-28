using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal static class JsonTypeInfoCache
    {
        private static readonly ConcurrentDictionary<Type, JsonTypeInfo> TypesInfoCache = new ConcurrentDictionary<Type, JsonTypeInfo>();

        public static readonly JsonTypeInfo ObjectTypeInfo = new JsonTypeInfo(typeof(object), TypeCode.Object);
        public static readonly JsonTypeInfo ObjectDictionaryTypeInfo = new JsonTypeInfo(typeof(Dictionary<string, object>), TypeCode.Object);
        public static readonly JsonTypeInfo ObjectArrayTypeInfo = new JsonTypeInfo(typeof(object[]), TypeCode.Object);

        public static JsonTypeInfo GetOrAddTypeInfo(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.Boolean:
                case TypeCode.DateTime:
                case TypeCode.String:
                    return new JsonTypeInfo(type, typeCode);

                case TypeCode.DBNull:
                case TypeCode.Empty:
                    return ObjectTypeInfo;
                default:
                    return 
                        type == null ? 
                        null :
                        TypesInfoCache.GetOrAdd(type, _ => new JsonTypeInfo(type, typeCode));
            }
        }
    }
}
