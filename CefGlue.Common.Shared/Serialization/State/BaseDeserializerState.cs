using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    // TODO - bcs - create Interface for StateManagement (rename the 'DeserializerState') and create 3 concrete classes - for objects, arrays and dictionaries
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

        public abstract bool IsStructObjectType { get; }

        public string PropertyName { get; set; }

        public virtual JsonTypeInfo ObjectTypeInfo => _objectTypeInfo;

        public abstract void SetValue(object value);

        public abstract ObjectHolderType CreateObjectInstance(Utf8JsonReader reader);

        object IDeserializerState.CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateObjectInstance(reader);
        }
    }
}
