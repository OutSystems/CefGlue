using System;
using System.Linq;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : IDeserializerState<Array>
    {
        private readonly JsonTypeInfo[] _arrayElementsTypeInfo;

        private long _arrayIndex;

        public ArrayDeserializerState(Utf8JsonReader reader, JsonTypeInfo arrayTypeInfo) :
            this(
                CreateArray(reader, arrayTypeInfo, out var arrayElementTypeInfo),
                new[] { arrayElementTypeInfo }
                ) { }

        public ArrayDeserializerState(Utf8JsonReader reader, JsonTypeInfo[] arrayElementsTypeInfo) :
            this(
                CreateArray(reader, arrayElementsTypeInfo.Length > 1 ? JsonTypeInfoCache.DefaultTypeInfo : arrayElementsTypeInfo[0]),
                arrayElementsTypeInfo
                ) { }

        private ArrayDeserializerState(Array objectHolder, JsonTypeInfo[] arrayElementsTypeInfo)
        {
            ObjectHolder = objectHolder;
            _arrayElementsTypeInfo = arrayElementsTypeInfo;
        }

        public Array ObjectHolder { get; }

        public string PropertyName { set => throw new NotImplementedException(); }

        public JsonTypeInfo CurrentElementTypeInfo => 
            _arrayIndex < _arrayElementsTypeInfo.Length ? _arrayElementsTypeInfo[_arrayIndex] : _arrayElementsTypeInfo.Last();

        public void SetValue(object value)
        {
            ObjectHolder.SetValue(value, _arrayIndex);
            _arrayIndex++;
        }

        private static Array CreateArray(Utf8JsonReader reader, JsonTypeInfo arrayTypeInfo, out JsonTypeInfo arrayElementTypeInfo)
        {
            arrayElementTypeInfo = 
                arrayTypeInfo.ArrayElementTypeInfo ?? 
                (arrayTypeInfo.ObjectKind == JsonTypeInfo.Kind.GenericObject ? JsonTypeInfoCache.DefaultTypeInfo : null);

            if (arrayElementTypeInfo == null)
            {
                throw new InvalidCastException($"Cannot deserialize an array to a non array type: '{arrayTypeInfo.ObjectType.Name}'.");
            }
            
            return CreateArray(reader, arrayElementTypeInfo);
        }

        private static Array CreateArray(Utf8JsonReader reader, JsonTypeInfo arrayElementTypeInfo)
        {
            var arraySize = reader.PeekAndCalculateArraySize();
            return Array.CreateInstance(arrayElementTypeInfo.ObjectType, arraySize);
        }
    }
}
