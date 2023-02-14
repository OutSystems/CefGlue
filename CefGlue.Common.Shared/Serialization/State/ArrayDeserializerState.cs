using System;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : IDeserializerState<Array>
    {
        private readonly JsonTypeInfo[] _arrayElementsTypeInfo;

        private long _arrayIndex;

        public ArrayDeserializerState(JsonTypeInfo arrayElementTypeInfo, int arraySize) :
        this(
                Array.CreateInstance(arrayElementTypeInfo.ObjectType, arraySize),
                new[] { arrayElementTypeInfo }
                ) { }

        public ArrayDeserializerState(JsonTypeInfo[] arrayElementsTypeInfo, int arraySize) :
            this(
                Array.CreateInstance(JsonTypeInfoCache.DefaultTypeInfo.ObjectType, arraySize),
                arrayElementsTypeInfo
                ) { }

        private ArrayDeserializerState(Array value, JsonTypeInfo[] arrayElementsTypeInfo)
        {
            Value = value;
            _arrayElementsTypeInfo = arrayElementsTypeInfo;
        }

        public Array Value { get; }

        public void SetCurrentPropertyName(string value) => throw new InvalidOperationException();

        public JsonTypeInfo CurrentElementTypeInfo => 
            _arrayElementsTypeInfo[Math.Min(_arrayElementsTypeInfo.Length - 1, _arrayIndex)];

        public void SetCurrentElementValue(object value)
        {
            Value.SetValue(value, _arrayIndex);
            _arrayIndex++;
        }
    }
}
