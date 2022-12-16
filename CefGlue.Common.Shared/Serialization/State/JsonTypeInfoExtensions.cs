using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal static class JsonTypeInfoExtensions
    {
        internal static Type GetPropertyType(this JsonTypeInfo ownerTypeInfo, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return ownerTypeInfo.ObjectType;
            }

            if (ownerTypeInfo.TypeMembers.TryGetValue(propertyName, out var memberInfo))
            {
                return memberInfo.Type;
            }

            return JsonTypeInfoCache.DefaultTypeInfo.ObjectType;
        }

        internal static Type GetArrayElementType(this JsonTypeInfo ownerTypeInfo, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return GetArrayElementType(ownerTypeInfo.ObjectType);
            }
            else if (!ownerTypeInfo.TypeMembers.TryGetValue(propertyName, out TypeMemberInfo memberInfo))
            {
                return JsonTypeInfoCache.DefaultTypeInfo.ObjectType;
            }
            else
            {
                return GetArrayElementType(memberInfo.Type);
            }
        }

        private static Type GetArrayElementType(Type arrayType)
        {
            if (arrayType == typeof(object))
            {
                return JsonTypeInfoCache.DefaultTypeInfo.ObjectType;
            }

            if (arrayType.IsArray)
            {
                return arrayType.GetElementType();
            }

            throw new InvalidCastException($"Cannot deserialize an array to a non array type: '{arrayType.Name}'!");
        }

        internal static Type GetCollectionElementType(this JsonTypeInfo ownerTypeInfo, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return GetCollectionElementType(ownerTypeInfo.ObjectType);
            }
            else if (!ownerTypeInfo.TypeMembers.TryGetValue(propertyName, out TypeMemberInfo memberInfo))
            {
                return JsonTypeInfoCache.DefaultTypeInfo.ObjectType;
            }
            else
            {
                return GetCollectionElementType(memberInfo.Type);
            }
        }

        private static Type GetCollectionElementType(Type collectionType)
        {
            // TODO - bcs - create unit test with a class that inherits from ICollection in JavascriptToNativeType
            var interfaces = collectionType.GetInterfaces();
            var collectionGenericInterface = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (collectionGenericInterface != null)
            {
                var tmpGenericInterfaceArgument = collectionGenericInterface.GetGenericArguments().Single();
                if (tmpGenericInterfaceArgument.IsGenericType &&
                    tmpGenericInterfaceArgument.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return tmpGenericInterfaceArgument.GenericTypeArguments[1];
                }
                return tmpGenericInterfaceArgument;
            }

            var collectionInterface = interfaces.FirstOrDefault(i => i == typeof(ICollection));
            if (collectionInterface != null)
            {
                return JsonTypeInfoCache.DefaultTypeInfo.ObjectType;
            }

            throw new InvalidOperationException($"GetCollectionElementType() called for a non collection type: '{collectionType.Name}'!");
        }
    }
}
