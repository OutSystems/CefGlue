using System;
using System.Linq;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class RootObjectDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _valueTypeInfo;

        public RootObjectDeserializerState(Type targetType)
        {
            _valueTypeInfo = JsonTypeInfoCache.GetOrAddTypeInfo(targetType);
        }

        public object Value { get; private set; }

        public void SetCurrentPropertyName(string value) => throw new InvalidOperationException();

        public JsonTypeInfo CurrentElementTypeInfo => _valueTypeInfo;

        public void SetCurrentElementValue(object value)
        {
            if (Value != null)
            {
                throw new InvalidOperationException("The root shouldn't be set multiple times.");
            }

            Value = 
                value == null && _valueTypeInfo.ObjectType.IsValueType ?
                Activator.CreateInstance(_valueTypeInfo.ObjectType, nonPublic: true) : 
                value;
        }
    }
}
