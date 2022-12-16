using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class DynamicDeserializerState : BaseDeserializerState<IDictionary<string, object>>
    {
        public DynamicDeserializerState(IDictionary<string, object> objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        internal static DynamicDeserializerState Create(JsonTypeInfo objectTypeInfo)
        {
            var obj = CreateObjectInstance();
            return new DynamicDeserializerState(obj, objectTypeInfo);
        }

        public override void SetValue(object value)
        {
            ObjectHolder[PropertyName] = value;
        }

        public override IDictionary<string, object> CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateObjectInstance();
        }

        private static IDictionary<string, object> CreateObjectInstance()
        {
            return new ExpandoObject();
        }
    }
}
