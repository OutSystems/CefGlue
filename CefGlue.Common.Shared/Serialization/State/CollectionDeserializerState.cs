using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    // TODO - bcs - create Interface for StateManagement (rename the 'DeserializerState') and create 3 concrete classes - for objects, arrays and dictionaries
    internal class CollectionDeserializerState : BaseDeserializerState<object>
    {
        private JsonTypeInfo _collectionTypeInfo;
    
        public CollectionDeserializerState(object objectHolder, JsonTypeInfo collectionTypeInfo, Type collectionElementType) : base(objectHolder, collectionElementType) 
        {
            _collectionTypeInfo = collectionTypeInfo;
            if (_collectionTypeInfo.CollectionAddMethod == null)
            {
                throw new ArgumentException($"CollectionTypeInfo argument must contain an Add method.");
            }
        }

        internal static CollectionDeserializerState Create(JsonTypeInfo objectTypeInfo, string propertyName)
        {
            var newCollection = CreateCollection(objectTypeInfo, propertyName, out var collectionElementType);
            return new CollectionDeserializerState(newCollection, objectTypeInfo, collectionElementType);
        }

        public override bool IsStructObjectType => false;

        public override void SetValue(object value)
        {
            var parameters = string.IsNullOrEmpty(PropertyName) ?
                new[] { value } :
                new[] { PropertyName, value };
            _collectionTypeInfo.CollectionAddMethod.Invoke(ObjectHolder, parameters);
        }

        public override object CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateCollection(ObjectTypeInfo, PropertyName, out var _);
        }

        private static object CreateCollection(JsonTypeInfo objectTypeInfo, string propertyName, out Type collectionElementType)
        {
            // is it an args array? (it is when the root object is an array and the parametersTypes argument was passed to the Deserializer function)
            collectionElementType = objectTypeInfo.GetCollectionElementType(propertyName);
            return Activator.CreateInstance(objectTypeInfo.ObjectType, nonPublic: true);
        }
    }
}
