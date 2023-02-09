using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ReadonlyDeserializerState : BaseDeserializerState<object>
    {
        public ReadonlyDeserializerState(object objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        public override void SetValue(object value)
        {
            throw new InvalidOperationException($"Cannot set value for a null {nameof(ReadonlyDeserializerState)}!");
        }
    }
}
