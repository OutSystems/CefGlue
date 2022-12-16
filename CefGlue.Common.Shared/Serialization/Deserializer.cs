using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Serialization.State;
using static Xilium.CefGlue.Common.Shared.Serialization.State.ParametersDeserializerState;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class Deserializer
    {
        private const int DeserializerMaxDepth = byte.MaxValue * 2;

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
            IDeserializerState previousStateIfEndArray = null;

            state.Push(new ArrayDeserializerState(root, typeToConvert));

            do
            {
                var currentState = state.Peek();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        currentState.SetValue(null);
                        break;

                    case JsonTokenType.Number:
                        currentState.SetValue(
                            reader.GetNumber(currentState.ObjectTypeInfo.GetPropertyType(currentState.PropertyName))
                            );
                        break;

                    case JsonTokenType.String:
                        currentState.SetValue(
                            reader.GetObjectFromString(currentState.ObjectTypeInfo.GetPropertyType(currentState.PropertyName))
                            );
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