using System;
using System.Dynamic;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.State
{
    internal class ObjectDeserializerState : BaseDeserializerState<object>
    {
        public ObjectDeserializerState(object objectHolder, JsonTypeInfo objectTypeInfo) : base(objectHolder, objectTypeInfo) { }

        internal static ObjectDeserializerState Create(JsonTypeInfo objectTypeInfo)
        {
            var obj = CreateObjectInstance(objectTypeInfo.ObjectType);
            return new ObjectDeserializerState(obj, objectTypeInfo);
        }

        public override bool IsStructObjectType => ObjectTypeInfo?.ObjectType?.IsValueType == true;

        public override void SetValue(object value)
        {
            if (ObjectHolder == null)
            {
                throw new InvalidOperationException($"Cannot value for a null ObjectHolder!");
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

        public override object CreateObjectInstance(Utf8JsonReader reader)
        {
            return CreateObjectInstance(ObjectTypeInfo.ObjectType);
        }

        internal static object CreateObjectInstance(Type type)
        {
            if (type == typeof(object))
            {
                throw new InvalidOperationException("Use the DictionaryDeserializerState instead.");
                //// When the deserializer receives 'dynamic' or object as the target collectionType,
                //// we need to instantiate an ExpandoObject, so that we can set the object's properties even considering the TypeInfo doesn't have them defined
                //return new ExpandoObject();
            }
            return Activator.CreateInstance(type, nonPublic: true);
        }
    }
}
