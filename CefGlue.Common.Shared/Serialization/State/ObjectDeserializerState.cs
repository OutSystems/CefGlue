using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ObjectDeserializerState : BaseDeserializerState<object>
    {
        public ObjectDeserializerState(JsonTypeInfo objectTypeInfo) : this(CreateObjectInstance(objectTypeInfo.ObjectType), objectTypeInfo) { }

        public ObjectDeserializerState(object objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        public override void SetValue(object value)
        {
            if (ObjectHolder == null)
            {
                throw new InvalidOperationException($"Cannot set value for a null ObjectHolder!");
            }

            if (ObjectTypeInfo.TypeMembers.TryGetValue(PropertyName, out var typeMemberInfo))
            {
                typeMemberInfo.SetValue(ObjectHolder, value);
            }
            else
            {
                throw new InvalidOperationException($"Property or Field '{PropertyName}' does not exist in collectionType '{ObjectTypeInfo.ObjectType.Name}'");
            }
        }

        internal static object CreateObjectInstance(Type type)
        {
            if (type == typeof(object))
            {
                throw new InvalidOperationException("Use the DictionaryDeserializerState instead.");
            }
            return Activator.CreateInstance(type, nonPublic: true);
        }
    }
}
