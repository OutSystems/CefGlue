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
            TypeMethodInfo CollectionAddMethod);

        private const BindingFlags EligibleMembers = BindingFlags.Public | BindingFlags.Instance;

        private readonly Lazy<InternalTypeInfo> _internalTypeInfo;

        public JsonTypeInfo(Type type)
        {
            _internalTypeInfo = new Lazy<InternalTypeInfo>(() => LoadTypeInfo(type));
            ObjectType = type;
        }
        
        public Type ObjectType { get; }

        public Kind ObjectKind => _internalTypeInfo.Value.ObjectKind;

        public TypeMemberInfoMap TypeMembers => _internalTypeInfo.Value.TypeMembers;
        
        public TypeMethodInfo CollectionAddMethod => _internalTypeInfo.Value.CollectionAddMethod;

        private static InternalTypeInfo LoadTypeInfo(Type objectType)
        {
            TypeMethodInfo collectionAddMethod = null;
            if (!objectType.IsArray)
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

            return new InternalTypeInfo(objectKind, typeMembers, collectionAddMethod);
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
