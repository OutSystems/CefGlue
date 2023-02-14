namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal interface IDeserializerState
    {
        object Value { get; }

        void SetCurrentPropertyName(string value);

        JsonTypeInfo CurrentElementTypeInfo { get; }

        void SetCurrentElementValue(object value);
    }

    internal interface IDeserializerState<ValueType> : IDeserializerState
    {
        object IDeserializerState.Value => Value;

        new ValueType Value { get; }
    }
}
