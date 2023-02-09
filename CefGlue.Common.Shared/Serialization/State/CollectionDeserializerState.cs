using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class CollectionDeserializerState : BaseDeserializerState<object>
    {
        private JsonTypeInfo _collectionTypeInfo;

        public CollectionDeserializerState(JsonTypeInfo objectTypeInfo, string propertyName) :
            this(
                CreateCollection(objectTypeInfo, propertyName, out var collectionElementType),
                objectTypeInfo,
                collectionElementType
                ) { }

        public CollectionDeserializerState(object objectHolder, JsonTypeInfo collectionTypeInfo, Type collectionElementType) : base(objectHolder, collectionElementType) 
        {
            _collectionTypeInfo = collectionTypeInfo;
            if (_collectionTypeInfo.CollectionAddMethod == null)
            {
                throw new ArgumentException($"CollectionTypeInfo argument must contain an Add method.");
            }
        }

        public override void SetValue(object value)
        {
            var parameters = string.IsNullOrEmpty(PropertyName) ?
                new[] { value } :
                new[] { PropertyName, value };
            _collectionTypeInfo.CollectionAddMethod.Invoke(ObjectHolder, parameters);
        }

        private static object CreateCollection(JsonTypeInfo objectTypeInfo, string propertyName, out Type collectionElementType)
        {
            collectionElementType = objectTypeInfo.GetCollectionElementType(propertyName);
            return Activator.CreateInstance(objectTypeInfo.ObjectType, nonPublic: true);
        }
    }
}
