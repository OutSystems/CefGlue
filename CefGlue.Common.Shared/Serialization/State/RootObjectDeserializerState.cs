using System;
using System.Linq;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class RootObjectDeserializerState : IDeserializerState<object>, IMultiTypeDeserializerState
    {
        private readonly JsonTypeInfo[] _objectTypesInfo;
        private readonly JsonTypeInfo _objectTypeInfo;

        public RootObjectDeserializerState(Type[] targetTypes)
        {
            _objectTypesInfo = targetTypes.Select(t => JsonTypeInfoCache.GetOrAddTypeInfo(t)).ToArray();
            _objectTypeInfo = _objectTypesInfo.Length > 1 ? JsonTypeInfoCache.GetOrAddTypeInfo(typeof(Type[])) : _objectTypesInfo.First();
        }

        public object ObjectHolder { get; private set; }

        public string PropertyName { private get; set; }

        public JsonTypeInfo CurrentElementTypeInfo => _objectTypeInfo;

        public JsonTypeInfo[] ObjectTypesInfo => _objectTypesInfo;

        public void SetValue(object value)
        {
            if (ObjectHolder != null)
            {
                throw new InvalidOperationException("The root cannot be set multiple times.");
            }

            ObjectHolder = value;
        }
    }
}
