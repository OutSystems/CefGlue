using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal static class JsonTypeInfoCache
    {
        private static readonly ConcurrentDictionary<Type, JsonTypeInfo> TypesInfoCache = new ConcurrentDictionary<Type, JsonTypeInfo>();

        internal static readonly JsonTypeInfo DefaultTypeInfo = new JsonTypeInfo(typeof(object));

        internal static JsonTypeInfo GetOrAddTypeInfo(Type type)
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
                    return TypesInfoCache.GetOrAdd(type, (_) =>
                    {
                        var eligibleMembers = BindingFlags.Public | BindingFlags.Instance;

                        var properties = type
                            .GetProperties(eligibleMembers)
                            .Where(p => p.CanWrite)
                            .Where(p => !p.GetIndexParameters().Any());

                        var fields = type
                            .GetFields(eligibleMembers)
                            .Where(f => !f.IsInitOnly);

                        var typeInfo = new JsonTypeInfo(type);

                        foreach (var prop in properties)
                        {
                            typeInfo.TypeMembers.Add(prop.Name, new TypeMemberInfo(prop.PropertyType, (obj, value) => prop.SetValue(obj, value)));
                        }

                        foreach (var field in fields)
                        {
                            typeInfo.TypeMembers.Add(field.Name, new TypeMemberInfo(field.FieldType, (obj, value) => field.SetValue(obj, value)));
                        }

                        MethodInfo addMethod;
                        if (type.IsCollection() && (addMethod = type.GetMethod("Add")) != null)
                        {
                            typeInfo.CollectionAddMethod = new TypeMethodInfo((obj, value) => addMethod.Invoke(obj, value));
                        }

                        return typeInfo;
                    });
            }
        }

        internal static JsonTypeInfo GetOrAddTypeInfo(JsonTypeInfo ownerTypeInfo, string propertyName)
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
