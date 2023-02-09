using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class DynamicDeserializerState : BaseDeserializerState<IDictionary<string, object>>
    {
        public DynamicDeserializerState(JsonTypeInfo objectTypeInfo) : this(CreateObjectInstance(), objectTypeInfo) { }

        public DynamicDeserializerState(IDictionary<string, object> objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        public override void SetValue(object value)
        {
            ObjectHolder[PropertyName] = value;
        }

        private static IDictionary<string, object> CreateObjectInstance()
        {
            return new ExpandoObject();
        }
    }
}
