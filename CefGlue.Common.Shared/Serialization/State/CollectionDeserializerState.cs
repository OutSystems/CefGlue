using System;
using System.Dynamic;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class CollectionDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _collectionTypeInfo;
        private readonly JsonTypeInfo _collectionElementTypeInfo;
        
        private string _propertyName;

        public CollectionDeserializerState(JsonTypeInfo collectionTypeInfo)
        {
            if (collectionTypeInfo.CollectionAddMethod == null)
            {
                throw new ArgumentException("Argument must contain an Add method.", nameof(collectionTypeInfo));
            }

            Value = Activator.CreateInstance(collectionTypeInfo.ObjectType, nonPublic: true);
            _collectionTypeInfo = collectionTypeInfo;
            _collectionElementTypeInfo = collectionTypeInfo.EnumerableElementTypeInfo;
        }

        public object Value { get; }

        public void SetCurrentPropertyName(string value) => _propertyName = value;
        
        public JsonTypeInfo CurrentElementTypeInfo => _collectionElementTypeInfo;

        public void SetCurrentElementValue(object value)
        {
            var parameters = string.IsNullOrEmpty(_propertyName) ?
                new[] { value } :
                new[] { _propertyName, value };
            _collectionTypeInfo.CollectionAddMethod.Invoke(Value, parameters);
        }
    }
}
