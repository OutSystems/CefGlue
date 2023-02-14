using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ObjectDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _valueTypeInfo;

        private string _propertyName;

        public ObjectDeserializerState(JsonTypeInfo valueTypeInfo) : this(CreateObjectInstance(valueTypeInfo.ObjectType), valueTypeInfo) { }

        public ObjectDeserializerState(object value, JsonTypeInfo valueTypeInfo)
        {
            Value = value;
            _valueTypeInfo = valueTypeInfo;
        }

        public object Value { get; }

        public void SetCurrentPropertyName(string value) => _propertyName = value;

        public JsonTypeInfo CurrentElementTypeInfo => _valueTypeInfo.GetPropertyTypeInfo(_propertyName);

        public void SetCurrentElementValue(object value)
        {
            if (Value == null)
            {
                throw new InvalidOperationException($"Cannot set value for a null {nameof(Value)}.");
            }

            var typeMemberInfo = _valueTypeInfo.GetTypeMemberInfo(_propertyName);
            if (typeMemberInfo != null)
            {
                typeMemberInfo.SetValue(Value, value);
            }
            else
            {
                throw new InvalidOperationException($"Property or Field '{_propertyName}' does not exist in objectType '{_valueTypeInfo.ObjectType.Name}'.");
            }
        }

        private static object CreateObjectInstance(Type type)
        {
            if (type == typeof(object))
            {
                throw new InvalidOperationException($"Use the {nameof(DynamicDeserializerState)} instead.");
            }
            return Activator.CreateInstance(type, nonPublic: true);
        }
    }
}
