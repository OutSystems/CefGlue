using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class CollectionDeserializerState : IDeserializerState<object>
    {
        private readonly JsonTypeInfo _collectionTypeInfo;
        private readonly JsonTypeInfo _collectionElementTypeInfo;

        public CollectionDeserializerState(JsonTypeInfo collectionTypeInfo)
        {
            if (collectionTypeInfo.CollectionAddMethod == null)
            {
                throw new ArgumentException("Argument must contain an Add method.", nameof(collectionTypeInfo));
            }

            ObjectHolder = CreateCollection(collectionTypeInfo);
            _collectionTypeInfo = collectionTypeInfo;
            _collectionElementTypeInfo = collectionTypeInfo.CollectionElementTypeInfo;
        }

        public object ObjectHolder { get; }

        public string PropertyName { private get; set; }

        public JsonTypeInfo CurrentElementTypeInfo => _collectionElementTypeInfo;

        public void SetValue(object value)
        {
            var parameters = string.IsNullOrEmpty(PropertyName) ?
                new[] { value } :
                new[] { PropertyName, value };
            _collectionTypeInfo.CollectionAddMethod.Invoke(ObjectHolder, parameters);
        }

        private static object CreateCollection(JsonTypeInfo objectTypeInfo)
        {
            return Activator.CreateInstance(objectTypeInfo.ObjectType, nonPublic: true);
        }
    }
}
