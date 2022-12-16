using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Serialization.State;
using static Xilium.CefGlue.Common.Shared.Serialization.State.ParametersDeserializerState;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class JsonDeserializer
    {
        // TODO - bcs - consider expanding the DeserlizerMaxDepth to integer.MaxValue
        private const int DeserializerMaxDepth = int.MaxValue; //255;

        private record ReferenceInfo
        {
            public ReferenceInfo(object obj, JsonTypeInfo typeInfo)
            {
                Object = obj;
                TypeInfo = typeInfo;
            }

            public object Object { get; private set; }
            public JsonTypeInfo TypeInfo { get; private set; }
        }

        private static readonly IDeserializerState ListWrapperMarker = new ObjectDeserializerState(objectTypeInfo: default, objectHolder: null);

        /// <summary>
        /// Deserializes the passed jsonString argument to an instance of the TargetType.
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="jsonString"></param>
        /// <param name="parametersTypes">When deserializing an argumentsArray string, it's possible to specify the specific typeInfo of each of the arguments.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static TargetType Deserialize<TargetType>(string jsonString, ParametersTypes parametersTypes = null)
        {
            try
            {
                if (jsonString == null)
                {
                    return default(TargetType);
                }

                return (TargetType)Deserialize(jsonString, typeof(TargetType), parametersTypes);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }

        private static object Deserialize(string jsonString, Type typeToConvert, ParametersTypes parametersTypes = null)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions() { MaxDepth = DeserializerMaxDepth });

            reader.Read();
            return Deserialize(ref reader, typeToConvert, parametersTypes);
        }

        private static object Deserialize(ref Utf8JsonReader reader, Type typeToConvert, ParametersTypes parametersTypes = null)
        {
            var state = new Stack<IDeserializerState>();
            var root = new object[1];
            var referencesMap = new Dictionary<string, ReferenceInfo>();

            //System.Diagnostics.Debugger.Launch();
            state.Push(new ArrayDeserializerState(root, typeToConvert));

            do
            {
                object value = null;
                var currentState = state.Peek();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        value = null;
                        break;

                    case JsonTokenType.Number:
                        value = reader.GetNumber(currentState.ObjectTypeInfo.GetPropertyType(currentState.PropertyName));
                        break;

                    case JsonTokenType.String:
                        value = reader.GetObjectFromString(currentState.ObjectTypeInfo.GetPropertyType(currentState.PropertyName));
                        break;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        value = reader.GetBoolean();
                        break;

                    case JsonTokenType.PropertyName:
                        currentState.PropertyName = reader.GetString();
                        continue;

                    case JsonTokenType.StartArray:
                        var isRootObjectHolder = currentState.ObjectHolder == root;
                        var newState = CreateNewDeserializerState(reader, currentState, isRootObjectHolder, parametersTypes);
                        value = newState.ObjectHolder;
                        state.Push(newState);
                        break;

                    case JsonTokenType.StartObject:
                        value = ReadComplexObject(ref reader, state, isRootObjectHolder: currentState.ObjectHolder == root, parametersTypes, referencesMap);
                        break;

                    case JsonTokenType.EndArray:
                        state.Pop();
                        continue;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        // Structs are immutable so, the properties/fields that were set in the deserialization need to be propagated to the parent object
                        var parentState = state.Peek();
                        if (currentState.IsStructObjectType && parentState.ObjectHolder != root)
                        {
                            // TODO - bcs - what about arrays that contain Structs? create a test for this and check how they behave
                            parentState.SetValue(currentState.ObjectHolder);
                        }
                        continue;
                }

                currentState.SetValue(value);
            } while (reader.Read());

            if (state.Count > 1 && state.Pop().ObjectHolder != root)
            {
                throw new InvalidOperationException("Invalid json format - missing enclosing EndArray or EndObject token(s).");
            }

            return root.First();
        }

        private static object ReadComplexObject(ref Utf8JsonReader reader, Stack<IDeserializerState> state, bool isRootObjectHolder, ParametersTypes parametersTypes, IDictionary<string, ReferenceInfo> referencesMap)
        {
            var tempReader = reader;
            object obj;
            IDeserializerState newState = null;
            JsonTypeInfo newTypeInfo = null;
            
            tempReader.ReadToken(JsonTokenType.StartObject);

            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                // empty object
                obj = new object();
            }
            else
            {
                // peek first field name
                var propName = tempReader.ReadPropertyName();
                IDeserializerState currentState;
                switch (propName)
                {
                    case JsonAttributes.Ref:
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName();  // skip the $ref
                        var refId = reader.GetString();
                        var refInfo = referencesMap[refId];
                        obj = refInfo.Object;
                        newTypeInfo = refInfo.TypeInfo;
                        newState = CreateNewDeserializerState(obj, newTypeInfo);
                        break;

                    case JsonAttributes.Id:
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName(); // skip the $id
                        var id = tempReader.ReadString();
                        propName = tempReader.ReadPropertyName();
                        if (propName == JsonAttributes.Values)
                        {
                            // it's a list
                            reader.Read(); // skip the $id
                            reader.ReadPropertyName(); // advance reader to the beginning of the list

                            currentState = state.Peek();
                            state.Push(ListWrapperMarker);
                            newState = CreateNewDeserializerState(reader, currentState, isRootObjectHolder, parametersTypes);
                            obj = newState.ObjectHolder;
                            newTypeInfo = newState.ObjectTypeInfo;
                        }
                        else
                        {
                            currentState = state.Peek();
                            newTypeInfo = currentState.ObjectTypeInfo;
                            newState = CreateNewDerializerState(reader, newTypeInfo, string.Empty, isRootObjectHolder, parametersTypes);
                            obj = newState.ObjectHolder;
                        }
                        referencesMap.Add(id, new ReferenceInfo(obj, newTypeInfo));
                        break;

                    default:
                        currentState = state.Peek();
                        newTypeInfo = JsonTypeInfoCache.GetOrAddTypeInfo(currentState.ObjectTypeInfo, currentState.PropertyName);
                        newState = CreateNewDerializerState(reader, newTypeInfo, currentState.PropertyName, isRootObjectHolder, parametersTypes);
                        obj = newState.ObjectHolder;
                        break;
                }
            }

            state.Push(newState);

            return obj;
        }

        private static IDeserializerState CreateNewDeserializerState(
            Utf8JsonReader reader, 
            IDeserializerState currentState, 
            bool isRootObjectHolder, 
            ParametersTypes parametersTypes)
        {
            return CreateNewDerializerState(reader, currentState.ObjectTypeInfo, currentState.PropertyName, isRootObjectHolder, parametersTypes);
        }

        private static IDeserializerState CreateNewDerializerState(
            Utf8JsonReader reader,
            JsonTypeInfo objectTypeInfo,
            string propertyName,
            bool isRootObjectHolder,
            ParametersTypes parametersTypes)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                if (isRootObjectHolder && parametersTypes != null)
                {
                    //return ArrayDeserializerState.Create(reader, objectTypeInfo, propertyName);
                    return ParametersDeserializerState.Create(reader, parametersTypes);
                }
                if (!objectTypeInfo.ObjectType.IsArray && objectTypeInfo.ObjectType.IsCollection())
                {
                    return CollectionDeserializerState.Create(objectTypeInfo, propertyName);
                }
                return ArrayDeserializerState.Create(reader, objectTypeInfo, propertyName);
            }

            if (objectTypeInfo.ObjectType.IsCollection())
            {
                return CollectionDeserializerState.Create(objectTypeInfo, propertyName);
            }

            if (objectTypeInfo.ObjectType == typeof(object))
            {
                return DictionaryDeserializerState.Create(objectTypeInfo);
            }

            return ObjectDeserializerState.Create(objectTypeInfo);
        }

        private static IDeserializerState CreateNewDeserializerState(object obj, JsonTypeInfo newTypeInfo)
        {
            if (obj is IDictionary<string, object> dictionaryObj)
            {
                return new DictionaryDeserializerState(dictionaryObj, newTypeInfo);
            }

            if (obj is Array arrayObj)
            {
                return new ArrayDeserializerState(arrayObj, newTypeInfo.ObjectType);
            }

            return new ObjectDeserializerState(obj, newTypeInfo);
        }
    }
}