using System;
using System.Collections.Generic;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ArrayDeserializerState : IDeserializerState<Array>
    {
        private readonly IReadOnlyList<JsonTypeInfo> _arrayElementsTypeInfo;

        private long _arrayIndex;

        public ArrayDeserializerState(JsonTypeInfo arrayElementTypeInfo, int arraySize)
        {
            Value = Array.CreateInstance(arrayElementTypeInfo.ObjectType, arraySize);
            _arrayElementsTypeInfo = new[] { arrayElementTypeInfo };
        }

        public ArrayDeserializerState(IReadOnlyList<JsonTypeInfo> arrayElementsTypeInfo, int arraySize)
        {
            Value = Array.CreateInstance(JsonTypeInfoCache.ObjectTypeInfo.ObjectType, arraySize);
            _arrayElementsTypeInfo = arrayElementsTypeInfo;
        }

        public Array Value { get; }

        public void SetCurrentPropertyName(string value) => throw new InvalidOperationException();

        public JsonTypeInfo CurrentElementTypeInfo => 
            _arrayElementsTypeInfo[(int)Math.Min(_arrayElementsTypeInfo.Count - 1, _arrayIndex)];

        public void SetCurrentElementValue(object value)
        {
            Value.SetValue(value, _arrayIndex);
            _arrayIndex++;
        }
    }
}
