using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ObjectDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _valueTypeInfo;

        private string _propertyName;

        public ObjectDeserializerState(JsonTypeInfo valueTypeInfo)
        {
            if (valueTypeInfo.ObjectType == typeof(object))
            {
                throw new InvalidOperationException($"Use the {nameof(CollectionDeserializerState)} instead.");
            }

            Value = Activator.CreateInstance(valueTypeInfo.ObjectType, nonPublic: true);
            _valueTypeInfo = valueTypeInfo;
        }

        public ObjectDeserializerState(JsonTypeInfo valueTypeInfo, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
            _valueTypeInfo = valueTypeInfo;
        }

        public object Value { get; }

        public void SetCurrentPropertyName(string value) => _propertyName = value;

        public JsonTypeInfo CurrentElementTypeInfo => _valueTypeInfo.GetPropertyTypeInfo(_propertyName);

        public void SetCurrentElementValue(object value)
        {
            var typeMemberInfo = _valueTypeInfo.GetTypeMemberInfo(_propertyName);

            // If the member does not exist in the target type, ignore it
            typeMemberInfo?.SetValue(Value, value);
        }
    }
}
