using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal interface IDeserializerState
    {
        object ObjectHolder { get; }

        string PropertyName { set; }

        JsonTypeInfo CurrentElementTypeInfo { get; }

        void SetValue(object value);
    }

    internal interface IDeserializerState<ObjectHolderType> : IDeserializerState
    {
        object IDeserializerState.ObjectHolder => ObjectHolder;

        new ObjectHolderType ObjectHolder { get; }
    }
}
