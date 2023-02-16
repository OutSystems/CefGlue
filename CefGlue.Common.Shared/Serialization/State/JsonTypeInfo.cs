using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeMemberInfoMap = System.Collections.Generic.IReadOnlyDictionary<string, Xilium.CefGlue.Common.Shared.Serialization.State.TypeMemberInfo>;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class JsonTypeInfo
    {
        public enum Kind
        {
            Object, // class & structs
            Array, // arrays
            Collection // Lists, Set, Stack, ...
        }

        private record InternalTypeInfo(
            Kind ObjectKind,
            TypeMemberInfoMap TypeMembers,
            TypeMethodInfo CollectionAddMethod,
            JsonTypeInfo EnumerableElementTypeInfo);

        private const BindingFlags EligibleMembers = BindingFlags.Public | BindingFlags.Instance;

        private readonly Lazy<InternalTypeInfo> _internalTypeInfo;

        public JsonTypeInfo(Type type, TypeCode typeCode)
        {
            _internalTypeInfo = new Lazy<InternalTypeInfo>(() => LoadTypeInfo(type));
            ObjectType = type;
            TypeCode = typeCode;
        }

        public Type ObjectType { get; }

        public TypeCode TypeCode { get; }

        public Kind ObjectKind => _internalTypeInfo.Value.ObjectKind;

        public TypeMethodInfo CollectionAddMethod => _internalTypeInfo.Value.CollectionAddMethod;

        /// <summary>
        /// If the ObjectKind is Collection or Array, it returns the type of the its elements.
        /// </summary>
        public JsonTypeInfo EnumerableElementTypeInfo => _internalTypeInfo.Value.EnumerableElementTypeInfo;

        public TypeMemberInfo GetTypeMemberInfo(string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                throw new ArgumentNullException(nameof(memberName));
            }
            return _internalTypeInfo.Value.TypeMembers?.GetValueOrDefault(memberName);
        }

        public JsonTypeInfo GetPropertyTypeInfo(string propertyName)
        {
            var memberInfo = GetTypeMemberInfo(propertyName);
            if (memberInfo != null)
            {
                return JsonTypeInfoCache.GetOrAddTypeInfo(memberInfo.Type);
            }

            return JsonTypeInfoCache.ObjectTypeInfo;
        }

        private static InternalTypeInfo LoadTypeInfo(Type objectType)
        {
            TypeMethodInfo collectionAddMethod = null;
            if (!objectType.IsArray)
            {
                collectionAddMethod =
                    GetMethodFromType(typeof(ICollection<>), objectType, nameof(ICollection<object>.Add)) ??
                    GetMethodFromType(typeof(IList), objectType, nameof(IList.Add)) ??
                    GetMethodFromType(typeof(IDictionary), objectType, nameof(IDictionary.Add)) ??
                    GetMethodFromType(typeof(IDictionary<string, object>), objectType, nameof(IDictionary<string, object>.Add));
            }

            var (objectKind, enumerableElementTypeInfo) = objectType switch
            {
                { IsArray: true } => (Kind.Array, JsonTypeInfoCache.GetOrAddTypeInfo(objectType.GetElementType())),
                _ when collectionAddMethod != null => (Kind.Collection, GetCollectionElementTypeInfo(objectType)),
                _ => (Kind.Object, null)
            };

            Dictionary<string, TypeMemberInfo> typeMembers = null;
            if (objectKind == Kind.Object && !objectType.IsPrimitive)
            {
                typeMembers = new();

                var properties = objectType
                .GetProperties(EligibleMembers)
                .Where(p => p.CanWrite && !p.GetIndexParameters().Any());

                var fields = objectType
                    .GetFields(EligibleMembers)
                    .Where(f => !f.IsInitOnly);

                foreach (var prop in properties)
                {
                    typeMembers.Add(prop.Name, new TypeMemberInfo(prop.PropertyType, (obj, value) => prop.SetValue(obj, value)));
                }

                foreach (var field in fields)
                {
                    typeMembers.Add(field.Name, new TypeMemberInfo(field.FieldType, (obj, value) => field.SetValue(obj, value)));
                }
            }
            
            return new InternalTypeInfo(objectKind, typeMembers, collectionAddMethod, enumerableElementTypeInfo);
        }

        private static JsonTypeInfo GetCollectionElementTypeInfo(Type collectionType)
        {
            var interfaces = collectionType.GetInterfaces();
            var collectionGenericInterface = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (collectionGenericInterface != null)
            {
                var collectionInterfaceGenericArgument = collectionGenericInterface.GetGenericArguments().Single();
                if (collectionInterfaceGenericArgument.IsGenericType &&
                    collectionInterfaceGenericArgument.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return JsonTypeInfoCache.GetOrAddTypeInfo(collectionInterfaceGenericArgument.GenericTypeArguments[1]);
                }
                return JsonTypeInfoCache.GetOrAddTypeInfo(collectionInterfaceGenericArgument);
            }

            var collectionInterface = interfaces.FirstOrDefault(i => i == typeof(ICollection));
            if (collectionInterface != null)
            {
                return JsonTypeInfoCache.ObjectTypeInfo;
            }

            throw new InvalidOperationException($"{collectionType.Name} is not a collection type.");
        }

        private static TypeMethodInfo GetMethodFromType(Type baseType, Type targetType, string methodName)
        {
            if (baseType.IsAssignableFrom(targetType))
            {
                var method = baseType.GetMethod(methodName);
                if (method != null)
                {
                    return new TypeMethodInfo((obj, value) => method.Invoke(obj, value));
                }
            }
            return null;
        }
    }
}
