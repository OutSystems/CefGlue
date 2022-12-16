using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class DictionaryDeserializerState : BaseDeserializerState<IDictionary<string, object>>
    {
        public DictionaryDeserializerState(IDictionary<string, object> objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        internal static DictionaryDeserializerState Create(JsonTypeInfo objectTypeInfo)
        {
            var obj = CreateObjectInstance();
            return new DictionaryDeserializerState(obj, objectTypeInfo);
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
