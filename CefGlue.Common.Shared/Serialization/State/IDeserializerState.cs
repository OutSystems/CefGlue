using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal interface IDeserializerState
    {
        object ObjectHolder { get; }

        string PropertyName { get; set; }

        JsonTypeInfo ObjectTypeInfo { get; }

        JsonTypeInfo[] ObjectTypesInfo { get; }

        Type GetPropertyType();

        void SetValue(object value);
    }

    internal interface IDeserializerState<ObjectHolderType> : IDeserializerState
    {
        new ObjectHolderType ObjectHolder { get; }
    }
}
