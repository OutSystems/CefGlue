using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xilium.CefGlue.Common.Shared.Helpers;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class JsonTypeInfo
    {
        private readonly Dictionary<string, TypeMemberInfo> _typeMembers = new();
        public JsonTypeInfo(Type type)
        {
            ObjectType = type;
            LoadTypeInfo();
        }
        public Type ObjectType { get; set; }
        public IReadOnlyDictionary<string, TypeMemberInfo> TypeMembers => _typeMembers;
        public TypeMethodInfo CollectionAddMethod { get; private set; }

        private void LoadTypeInfo()
        {
            var eligibleMembers = BindingFlags.Public | BindingFlags.Instance;

            var properties = ObjectType
                .GetProperties(eligibleMembers)
                .Where(p => p.CanWrite)
                .Where(p => !p.GetIndexParameters().Any());

            var fields = ObjectType
                .GetFields(eligibleMembers)
                .Where(f => !f.IsInitOnly);

            foreach (var prop in properties)
            {
                _typeMembers.Add(prop.Name, new TypeMemberInfo(prop.PropertyType, (obj, value) => prop.SetValue(obj, value)));
            }

            foreach (var field in fields)
            {
                _typeMembers.Add(field.Name, new TypeMemberInfo(field.FieldType, (obj, value) => field.SetValue(obj, value)));
            }

            MethodInfo addMethod;
            if (ObjectType.IsCollection() && (addMethod = ObjectType.GetMethod("Add")) != null)
            {
                CollectionAddMethod = new TypeMethodInfo((obj, value) => addMethod.Invoke(obj, value));
            }
        }
    }
}
