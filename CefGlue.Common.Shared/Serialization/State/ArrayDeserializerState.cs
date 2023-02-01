using System;
using static Xilium.CefGlue.Common.Shared.Serialization.State.ParametersDeserializerState;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : BaseDeserializerState<Array>
    {
        private long _arrayIndex;

        public ArrayDeserializerState(Array objectHolder, Type arrayElementType) : base(objectHolder, arrayElementType) { }

        internal static ArrayDeserializerState Create(Utf8JsonReader reader, JsonTypeInfo objectTypeInfo, string propertyName)
        {
            var newArray = CreateArray(reader, objectTypeInfo, propertyName, out var arrayElementType);
            return new ArrayDeserializerState(newArray, arrayElementType);
        }

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
            return Array.CreateInstance(arrayElementType, reader.PeekAndCalculateArraySize());
        }
    }

    internal class ArrayDeserializerState<ArrayElementType> : ArrayDeserializerState
    {
        public ArrayDeserializerState(Array objectHolder) : base(objectHolder, typeof(ArrayElementType)) { }
    }
}
