using System;
using System.Collections.Concurrent;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal static class JsonTypeInfoCache
    {
        private static readonly ConcurrentDictionary<Type, JsonTypeInfo> TypesInfoCache = new ConcurrentDictionary<Type, JsonTypeInfo>();

        public static readonly JsonTypeInfo DefaultTypeInfo = new JsonTypeInfo(typeof(object));

        public static JsonTypeInfo GetOrAddTypeInfo(Type type)
        {
            switch (Type.GetTypeCode(type))
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
                    return new JsonTypeInfo(type);

                case TypeCode.Boolean:
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Empty:
                case TypeCode.String:
                    return DefaultTypeInfo;
                default:
                    return 
                        type == null ? 
                        null :
                        TypesInfoCache.GetOrAdd(type, (_) => new JsonTypeInfo(type));
            }
        }

        public static JsonTypeInfo GetOrAddTypeInfo(JsonTypeInfo ownerTypeInfo, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return ownerTypeInfo;
            }

            if (!ownerTypeInfo.TypeMembers.TryGetValue(propertyName, out var propertyType))
            {
                return DefaultTypeInfo;
            }

            return GetOrAddTypeInfo(propertyType.Type);
        }
    }
}
