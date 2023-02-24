using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ReadonlyDeserializerState : IDeserializerState<object>
    {
        public ReadonlyDeserializerState(object value) 
        {
            Value = value;
        }

        public static IDeserializerState Default => new ReadonlyDeserializerState(new object());

        public object Value { get; }

        public void SetCurrentPropertyName(string value) => throw new InvalidOperationException();
        
        public JsonTypeInfo CurrentElementTypeInfo => null;

        public void SetCurrentElementValue(object value)
        {
           throw new InvalidOperationException($"Cannot set value on a {nameof(ReadonlyDeserializerState)}.");
        }
    }
}
