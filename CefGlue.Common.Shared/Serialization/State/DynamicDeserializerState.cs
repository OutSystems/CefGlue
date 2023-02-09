using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class DynamicDeserializerState : IDeserializerState<IDictionary<string, object>>
    {
        public DynamicDeserializerState() 
        {
            ObjectHolder = new ExpandoObject();
        }

        public IDictionary<string, object> ObjectHolder { get; }

        public string PropertyName { private get; set; }

        public JsonTypeInfo CurrentElementTypeInfo => JsonTypeInfoCache.DefaultTypeInfo;

        public void SetValue(object value)
        {
            ObjectHolder[PropertyName] = value;
        }
    }
}
