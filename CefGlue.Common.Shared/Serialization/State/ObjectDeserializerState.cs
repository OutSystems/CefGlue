using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ObjectDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _objectTypeInfo;

        public ObjectDeserializerState(JsonTypeInfo objectTypeInfo) : this(CreateObjectInstance(objectTypeInfo.ObjectType), objectTypeInfo) { }

        public ObjectDeserializerState(object objectHolder, JsonTypeInfo objectTypeInfo)
        {
            ObjectHolder = objectHolder;
            _objectTypeInfo = objectTypeInfo;
        }

        public object ObjectHolder { get; }

        public string PropertyName { private get; set; }

        public JsonTypeInfo CurrentElementTypeInfo => _objectTypeInfo.GetPropertyTypeInfo(PropertyName);

        public void SetValue(object value)
        {
            if (ObjectHolder == null)
            {
                throw new InvalidOperationException($"Cannot set value for a null {nameof(ObjectHolder)}.");
            }

            if (_objectTypeInfo.TypeMembers.TryGetValue(PropertyName, out var typeMemberInfo))
            {
                typeMemberInfo.SetValue(ObjectHolder, value);
            }
            else
            {
                throw new InvalidOperationException($"Property or Field '{PropertyName}' does not exist in objectType '{_objectTypeInfo.ObjectType.Name}'.");
            }
        }

        internal static object CreateObjectInstance(Type type)
        {
            if (type == typeof(object))
            {
                throw new InvalidOperationException($"Use the {nameof(DynamicDeserializerState)} instead.");
            }
            return Activator.CreateInstance(type, nonPublic: true);
        }
    }
}
