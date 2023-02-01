using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal interface IDeserializerState
    {
        object ObjectHolder { get; }

        string PropertyName { get; set; }

        JsonTypeInfo ObjectTypeInfo { get; }

        Type GetPropertyType();

        object CreateObjectInstance(Utf8JsonReader reader);

        void SetValue(object value);
    }

    internal interface IDeserializerState<ObjectHolderType> : IDeserializerState
    {
        new ObjectHolderType ObjectHolder { get; }

        new ObjectHolderType CreateObjectInstance(Utf8JsonReader reader);
    }
}
