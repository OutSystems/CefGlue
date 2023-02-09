using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal interface IMultiTypeDeserializerState
    {
        JsonTypeInfo[] ObjectTypesInfo { get; }
    }
}
