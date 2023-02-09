using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ReadonlyDeserializerState : IDeserializerState<object>
    {
        public ReadonlyDeserializerState(object objectHolder) 
        {
            ObjectHolder = objectHolder;
        }

        public object ObjectHolder { get; }

        public string PropertyName { private get; set; }

        public JsonTypeInfo CurrentElementTypeInfo => null;

        public void SetValue(object value)
        {
            throw new InvalidOperationException($"Cannot set value for a null {nameof(ReadonlyDeserializerState)}.");
        }
    }
}
