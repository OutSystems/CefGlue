using System;
using System.Linq;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : BaseDeserializerState<Array>
    {
        private readonly JsonTypeInfo[] _arrayElementsTypeInfo;

        private long _arrayIndex;

        public ArrayDeserializerState(Utf8JsonReader reader, JsonTypeInfo[] arrayElementsTypeInfo) :
            this(
                CreateArray(reader, arrayElementsTypeInfo.Length > 1 ? typeof(object) : arrayElementsTypeInfo[0].ObjectType),
                arrayElementsTypeInfo
                ) { }

        public ArrayDeserializerState(Utf8JsonReader reader, JsonTypeInfo objectTypeInfo, string propertyName) :
            this(
                CreateArray(reader, objectTypeInfo, propertyName, out var arrayElementType),
                arrayElementType
                ) { }

        public ArrayDeserializerState(Array objectHolder, Type arrayElementType) : 
            this(objectHolder, new[] { JsonTypeInfoCache.GetOrAddTypeInfo(arrayElementType) }) { }

        public ArrayDeserializerState(Array objectHolder, Type[] targetTypes) : 
            base(objectHolder, type: typeof(Type[])) { 
            _arrayElementsTypeInfo = targetTypes.Select(t => JsonTypeInfoCache.GetOrAddTypeInfo(t)).ToArray();
        }

        public ArrayDeserializerState(Array objectHolder, JsonTypeInfo[] arrayElementsTypeInfo) : 
            base(objectHolder)
        {
            _arrayElementsTypeInfo = arrayElementsTypeInfo;
        }

        public override JsonTypeInfo ObjectTypeInfo =>
            _arrayElementsTypeInfo[_arrayIndex < _arrayElementsTypeInfo.Length ? _arrayIndex : _arrayElementsTypeInfo.Length - 1];

        /// <summary>
        /// If the ArrayDeserializerState was instantiated with an array of TargetTypes, 
        /// this property returns the array of TypeInfo of its elements, otherwise returns null
        /// </summary>
        public override JsonTypeInfo[] ObjectTypesInfo => 
            base.ObjectTypeInfo?.ObjectType == typeof(Type[]) ? _arrayElementsTypeInfo : null;
        
        public override void SetValue(object value)
        {
            ObjectHolder.SetValue(value, _arrayIndex);
            _arrayIndex++;
        }

        private static Array CreateArray(Utf8JsonReader reader, JsonTypeInfo objectTypeInfo, string propertyName, out Type arrayElementType)
        {
            // is it an args array? (it is when the root object is an array and the parametersTypes argument was passed to the Deserializer function)
            arrayElementType = objectTypeInfo.GetArrayElementType(propertyName);
            return CreateArray(reader, arrayElementType);
        }

        private static Array CreateArray(Utf8JsonReader reader, Type arrayElementType)
        {
            var arraySize = reader.PeekAndCalculateArraySize();
            return Array.CreateInstance(arrayElementType, arraySize);
        }
    }
}
