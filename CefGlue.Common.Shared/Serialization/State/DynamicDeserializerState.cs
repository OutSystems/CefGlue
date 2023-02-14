using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class DynamicDeserializerState : IDeserializerState<IDictionary<string, object>>
    {
        private string _propertyName;

        public DynamicDeserializerState() 
        {
            Value = new ExpandoObject();
        }

        public IDictionary<string, object> Value { get; }

        public void SetCurrentPropertyName(string value) => _propertyName = value;

        public JsonTypeInfo CurrentElementTypeInfo => JsonTypeInfoCache.DefaultTypeInfo;

        public void SetCurrentElementValue(object value)
        {
            if (string.IsNullOrEmpty(_propertyName))
            {
                throw new InvalidOperationException($"Current PropertyName not set. {nameof(SetCurrentElementValue)} must be preceeded by {nameof(SetCurrentPropertyName)}.");
            }

            Value[_propertyName] = value;
        }
    }
}
