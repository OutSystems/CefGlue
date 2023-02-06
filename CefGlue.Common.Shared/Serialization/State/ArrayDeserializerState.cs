using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : BaseDeserializerState<Array>
    {
        private readonly Type[] _targetTypes;

        private long _arrayIndex;

        public ArrayDeserializerState(Array objectHolder, Type arrayElementType) : this(objectHolder, new[] { arrayElementType }) { }

        public ArrayDeserializerState(Array objectHolder, Type[] targetTypes) : base(objectHolder)
        {
            _targetTypes = targetTypes;
        }

        internal static ArrayDeserializerState Create(Utf8JsonReader reader, Type[] targetTypes)
        {
            var newArray = CreateArray(reader, targetTypes.Length > 1 ? typeof(object) : targetTypes[0]);
            return new ArrayDeserializerState(newArray, targetTypes);
        }

        internal static ArrayDeserializerState Create(Utf8JsonReader reader, JsonTypeInfo objectTypeInfo, string propertyName)
        {
            var newArray = CreateArray(reader, objectTypeInfo, propertyName, out var arrayElementType);
            return new ArrayDeserializerState(newArray, arrayElementType);
        }

        public override JsonTypeInfo ObjectTypeInfo =>
            JsonTypeInfoCache.GetOrAddTypeInfo(
                _targetTypes[_arrayIndex < _targetTypes.Length ? _arrayIndex : _targetTypes.Length - 1]
                );

        public override void SetValue(object value)
        {
            ObjectHolder.SetValue(value, _arrayIndex);
            _arrayIndex++;
        }

        public override Array CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateArray(reader, ObjectTypeInfo, PropertyName, out var _);
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
