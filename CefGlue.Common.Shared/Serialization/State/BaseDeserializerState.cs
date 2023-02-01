using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal abstract class BaseDeserializerState<ObjectHolderType> : IDeserializerState<ObjectHolderType>
    {
        private readonly JsonTypeInfo _objectTypeInfo;

        protected BaseDeserializerState(ObjectHolderType objectHolder)
        {
            ObjectHolder = objectHolder;
        }

        protected BaseDeserializerState(ObjectHolderType objectHolder, JsonTypeInfo jsonTypeInfo) : this(objectHolder)
        {
            _objectTypeInfo = jsonTypeInfo;
        }

        protected BaseDeserializerState(ObjectHolderType objectHolder, Type type) :
            this(objectHolder, JsonTypeInfoCache.GetOrAddTypeInfo(type)) { }

        public ObjectHolderType ObjectHolder { get; }

        object IDeserializerState.ObjectHolder => ObjectHolder;

        public string PropertyName { get; set; }

        public virtual JsonTypeInfo ObjectTypeInfo => _objectTypeInfo;

        public Type GetPropertyType() => ObjectTypeInfo.GetPropertyType(PropertyName);

        public abstract void SetValue(object value);

        public abstract ObjectHolderType CreateObjectInstance(Utf8JsonReader reader);

        object IDeserializerState.CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateObjectInstance(reader);
        }
    }
}
