using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.Serialization.State;
using static Xilium.CefGlue.Common.Shared.Serialization.State.ParametersDeserializerState;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class Deserializer
    {
        private const int DeserializerMaxDepth = int.MaxValue;

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
            if (jsonString == null)
            {
                return default(TargetType);
            }

            try
            {
                return InnerDeserialize<TargetType>(jsonString, parametersTypes);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }

        private static TargetType InnerDeserialize<TargetType>(string jsonString, ParametersTypes parametersTypes = null)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions() { MaxDepth = DeserializerMaxDepth });

            reader.Read();
            return Deserialize<TargetType>(ref reader, parametersTypes);
        }

        private static TargetType Deserialize<TargetType>(ref Utf8JsonReader reader, ParametersTypes parametersTypes = null)
        {
            var state = new Stack<IDeserializerState>();
            var root = new TargetType[1];
            var referencesMap = new Dictionary<string, ReferenceInfo>();
            IDeserializerState previousStateIfEndArray = null;

            state.Push(new ArrayDeserializerState<TargetType>(root));

            do
            {
                var currentState = state.Peek();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        currentState.SetValue(null);
                        break;

                    case JsonTokenType.Number:
                        currentState.SetValue(reader.GetNumber(currentState.GetPropertyType()));
                        break;

                    case JsonTokenType.String:
                        currentState.SetValue(reader.GetObjectFromString(currentState.GetPropertyType()));
                        break;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        currentState.SetValue(reader.GetBoolean());
                        break;

                    case JsonTokenType.PropertyName:
                        currentState.PropertyName = reader.GetString();
                        break;

                    case JsonTokenType.StartArray:
                        var isRootObjectHolder = currentState.ObjectHolder == root;
                        var newState = CreateNewDeserializerState(reader, currentState, isRootObjectHolder, parametersTypes);
                        currentState.SetValue(newState.ObjectHolder);
                        state.Push(newState);
                        break;

                    case JsonTokenType.StartObject:
                        ReadComplexObject(ref reader, state, isRootObjectHolder: currentState.ObjectHolder == root, parametersTypes, referencesMap);
                        break;

                    case JsonTokenType.EndArray:
                        state.Pop();
                        break;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        // The value is only set at the EndObject so it is be able to properly deserialize not only classes, but also Structs, which are immutable so,
                        // they can only be stored in an object after all of the properties/fields that were set in the deserialization
                        var stateHoldingArrayOrObject = currentState == ListWrapperMarker ? previousStateIfEndArray : currentState;
                        if (stateHoldingArrayOrObject == null)
                        {
                            throw new InvalidOperationException("A ListWrapperMarker must always be preceeded by a state that holds an array.");
                        }
                        var parentState = state.Peek();
                        parentState.SetValue(stateHoldingArrayOrObject.ObjectHolder);
                        break;
                }

                previousStateIfEndArray = reader.TokenType == JsonTokenType.EndArray ? currentState : null;
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
            IDeserializerState newState;
            
            tempReader.ReadToken(JsonTokenType.StartObject);

            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                // empty object (e.g. deserializing a new object() results in "{}")
                newState = CreateNewDeserializerState(new object(), JsonTypeInfoCache.GetOrAddTypeInfo(typeof(object)));
            }
            else
            {
                // peek first field name
                var propName = tempReader.ReadPropertyName();
                IDeserializerState currentState;
                switch (propName)
                {
                    // possible cases of json strings being deserialized that reach this point:
                    // {"$ref":"1"}
                    // {"$id":"1","$values":[...]}
                    // {"$id":"1","property":...}
                    // {"property":...}
                    case JsonAttributes.Ref:
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName();  // skip the $ref
                        var refId = reader.GetString();
                        var refInfo = referencesMap[refId];
                        newState = CreateNewDeserializerState(refInfo.Object, refInfo.TypeInfo);
                        break;

                    case JsonAttributes.Id:
                        JsonTypeInfo newTypeInfo;
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName(); // skip the $id
                        var id = tempReader.ReadString();
                        if (tempReader.ReadPropertyName() == JsonAttributes.Values)
                        {
                            // it's a list
                            reader.Read(); // skip the $id
                            reader.ReadPropertyName(); // advance reader to the beginning of the list

                            currentState = state.Peek();
                            state.Push(ListWrapperMarker);
                            newState = CreateNewDeserializerState(reader, currentState, isRootObjectHolder, parametersTypes);
                            newTypeInfo = newState.ObjectTypeInfo;
                        }
                        else
                        {
                            currentState = state.Peek();
                            newTypeInfo = currentState.ObjectTypeInfo;
                            newState = CreateNewDerializerState(reader, newTypeInfo, string.Empty, isRootObjectHolder, parametersTypes);
                        }
                        referencesMap.Add(id, new ReferenceInfo(newState.ObjectHolder, newTypeInfo));
                        break;

                    default:
                        currentState = state.Peek();
                        newTypeInfo = JsonTypeInfoCache.GetOrAddTypeInfo(currentState.ObjectTypeInfo, currentState.PropertyName);
                        newState = CreateNewDerializerState(reader, newTypeInfo, currentState.PropertyName, isRootObjectHolder, parametersTypes);
                        break;
                }
            }

            state.Push(newState);

            return newState.ObjectHolder;
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
            if (isRootObjectHolder && parametersTypes != null)
            {
                return ParametersDeserializerState.Create(reader, parametersTypes);
            }

            if (objectTypeInfo.ObjectType.IsCollection() && !objectTypeInfo.ObjectType.IsArray)
            {
                return CollectionDeserializerState.Create(objectTypeInfo, propertyName);
            }

            if (reader.TokenType == JsonTokenType.StartArray || objectTypeInfo.ObjectType.IsArray)
            {
                return ArrayDeserializerState.Create(reader, objectTypeInfo, propertyName);
            }

            if (objectTypeInfo.ObjectType == typeof(object))
            {
                return DynamicDeserializerState.Create(objectTypeInfo);
            }

            return ObjectDeserializerState.Create(objectTypeInfo);
        }

        private static IDeserializerState CreateNewDeserializerState(object obj, JsonTypeInfo newTypeInfo)
        {
            if (obj is IDictionary<string, object> dictionaryObj)
            {
                return new DynamicDeserializerState(dictionaryObj, newTypeInfo);
            }

            if (obj is Array arrayObj)
            {
                return new ArrayDeserializerState(arrayObj, newTypeInfo.ObjectType);
            }

            return new ObjectDeserializerState(obj, newTypeInfo);
        }
    }
}