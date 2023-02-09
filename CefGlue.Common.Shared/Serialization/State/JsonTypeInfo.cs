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
            GenericObject, // typeof(object)
            Object, // class & structs
            Array, // arrays
            Collection // Lists, Set, Stack, ...
        }

        private record InternalTypeInfo(
            Kind ObjectKind,
            TypeMemberInfoMap TypeMembers,
            TypeMethodInfo CollectionAddMethod,
            JsonTypeInfo CollectionElementTypeInfo,
            JsonTypeInfo ArrayElementTypeInfo);

        private const BindingFlags EligibleMembers = BindingFlags.Public | BindingFlags.Instance;

        private readonly Lazy<InternalTypeInfo> _internalTypeInfo;
        
        private Dictionary<string, JsonTypeInfo> _propertiesTypeInfo;

        public JsonTypeInfo(Type type, TypeCode typeCode)
        {
            _internalTypeInfo = new Lazy<InternalTypeInfo>(() => LoadTypeInfo(type));
            ObjectType = type;
            TypeCode = typeCode;
        }
        
        public Type ObjectType { get; }

        public TypeCode TypeCode { get; }

        public Kind ObjectKind => _internalTypeInfo.Value.ObjectKind;

        public TypeMemberInfoMap TypeMembers => _internalTypeInfo.Value.TypeMembers;
        
        public TypeMethodInfo CollectionAddMethod => _internalTypeInfo.Value.CollectionAddMethod;

        public JsonTypeInfo CollectionElementTypeInfo => _internalTypeInfo.Value.CollectionElementTypeInfo;

        public JsonTypeInfo ArrayElementTypeInfo => _internalTypeInfo.Value.ArrayElementTypeInfo;

        public JsonTypeInfo GetPropertyTypeInfo(string propertyName)
        {
            // Return the type itself when no propertyName was passed OR it's a primitive valuetype (e.g. int32, byte, etc).
            // The IsPrimitive is to exclude Structs, which are also ValueTypes but that contain proerties and fields
            if (string.IsNullOrEmpty(propertyName) || (ObjectType.IsPrimitive && ObjectType.IsValueType))
            {
                return this;
            }

            _propertiesTypeInfo ??= new();

            if (!_propertiesTypeInfo.TryGetValue(propertyName, out var propertyTypeInfo))
            {
                if (TypeMembers.TryGetValue(propertyName, out var memberInfo))
                {
                    propertyTypeInfo = JsonTypeInfoCache.GetOrAddTypeInfo(memberInfo.Type);
                }

                propertyTypeInfo ??= JsonTypeInfoCache.DefaultTypeInfo;
                _propertiesTypeInfo.Add(propertyName, propertyTypeInfo);
            }

            return propertyTypeInfo;
        }

        private static InternalTypeInfo LoadTypeInfo(Type objectType)
        {
            JsonTypeInfo arrayElementTypeInfo = null;
            TypeMethodInfo collectionAddMethod = null;
            JsonTypeInfo collectionElementTypeInfo = null;
            if (objectType.IsArray)
            {
                arrayElementTypeInfo = JsonTypeInfoCache.GetOrAddTypeInfo(objectType.GetElementType());
            }
            else
            {
                collectionAddMethod =
                    GetMethodFromType(typeof(ICollection<>), objectType, nameof(ICollection<object>.Add)) ??
                    GetMethodFromType(typeof(IList), objectType, nameof(IList.Add)) ??
                    GetMethodFromType(typeof(IDictionary), objectType, nameof(IDictionary.Add));
            }

            var objectKind = objectType switch
            {
                { IsArray: true } => Kind.Array,
                _ when collectionAddMethod != null => Kind.Collection,
                _ when objectType == typeof(object) => Kind.GenericObject,
                _ => Kind.Object
            };

            if (objectKind == Kind.Collection)
            {
                collectionElementTypeInfo = GetCollectionElementTypeInfo(objectType);
            }

            var properties = objectType
                .GetProperties(EligibleMembers)
                .Where(p => p.CanWrite)
                .Where(p => !p.GetIndexParameters().Any());

            var fields = objectType
                .GetFields(EligibleMembers)
                .Where(f => !f.IsInitOnly);

            var typeMembers = new Dictionary<string, TypeMemberInfo>();
            foreach (var prop in properties)
            {
                typeMembers.Add(prop.Name, new TypeMemberInfo(prop.PropertyType, (obj, value) => prop.SetValue(obj, value)));
            }

            foreach (var field in fields)
            {
                typeMembers.Add(field.Name, new TypeMemberInfo(field.FieldType, (obj, value) => field.SetValue(obj, value)));
            }

            return new InternalTypeInfo(objectKind, typeMembers, collectionAddMethod, collectionElementTypeInfo, arrayElementTypeInfo);
        }

        private static JsonTypeInfo GetCollectionElementTypeInfo(Type collectionType)
        {
            var interfaces = collectionType.GetInterfaces();
            var collectionGenericInterface = interfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
            if (collectionGenericInterface != null)
            {
                var tmpGenericInterfaceArgument = collectionGenericInterface.GetGenericArguments().Single();
                if (tmpGenericInterfaceArgument.IsGenericType &&
                    tmpGenericInterfaceArgument.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return JsonTypeInfoCache.GetOrAddTypeInfo(tmpGenericInterfaceArgument.GenericTypeArguments[1]);
                }
                return JsonTypeInfoCache.GetOrAddTypeInfo(tmpGenericInterfaceArgument);
            }

            var collectionInterface = interfaces.FirstOrDefault(i => i == typeof(ICollection));
            if (collectionInterface != null)
            {
                return JsonTypeInfoCache.DefaultTypeInfo;
            }

            throw new InvalidOperationException($"{nameof(GetCollectionElementTypeInfo)} called for a non collection type: '{collectionType.Name}'!");
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
