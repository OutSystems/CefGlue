using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Helpers;
using Xilium.CefGlue.Common.Shared.Serialization.State;

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
        public static TargetType Deserialize<TargetType>(string jsonString)
        {
            if (jsonString == null)
            {
                return default;
            }
            return (TargetType)InnerDeserialize(jsonString, typeof(TargetType));
        }

        /// <summary>
        /// Deserializes the passed jsonString argument to an instance of the TargetType.
        /// </summary>
        /// <typeparam name="TargetType"></typeparam>
        /// <param name="jsonString"></param>
        /// <param name="targetTypes">When deserializing an array, 
        /// it's possible to specify the specific type of each of its elements.
        /// When multiple targetTypes are passed, the result will always be an object[] instance.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">When targetTypes is null or empty</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Deserialize(string jsonString, params Type[] targetTypes)
        {
            if (targetTypes == null || targetTypes.Length == 0)
            {
                throw new ArgumentException("At least one target type must be specified.");
            }

            if (jsonString == null)
            {
                return Activator.CreateInstance(targetTypes[0], nonPublic: true);
            }

            return InnerDeserialize(jsonString, targetTypes);
        }

        private static object InnerDeserialize(string jsonString, params Type[] targetTypes)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions() { MaxDepth = DeserializerMaxDepth });

            reader.Read();
            return InnerDeserialize(ref reader, targetTypes);
        }

        private static object InnerDeserialize(ref Utf8JsonReader reader, Type[] targetTypes)
        {
            var state = new Stack<IDeserializerState>();
            var root = new object[1];
            var referencesMap = new Dictionary<string, ReferenceInfo>();

            state.Push(new ArrayDeserializerState(root, targetTypes));
            
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
                        var newState = CreateNewDeserializerState(reader, currentState, currentState.PropertyName);
                        currentState.SetValue(newState.ObjectHolder);
                        state.Push(newState);
                        break;

                    case JsonTokenType.StartObject:
                        ReadComplexObject(ref reader, state, referencesMap);
                        break;

                    case JsonTokenType.EndArray:
                        state.Pop();
                        if (state.Peek() == ListWrapperMarker)
                        {
                            state.Pop();
                            reader.ReadToken(JsonTokenType.EndArray);
                            SetParentStateValue(state, currentState);
                        }

                        break;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        // The value is only set at the EndObject so it is be able to properly deserialize not only classes, but also Structs, which are immutable so,
                        // they can only be stored in an object after all of the properties/fields that were set in the deserialization
                        SetParentStateValue(state, currentState);
                        break;
                }
            } while (reader.Read());

            if (state.Count > 1 && state.Pop().ObjectHolder != root)
            {
                throw new InvalidOperationException("Invalid json format - missing enclosing EndArray or EndObject token(s).");
            }

            return root.First();
        }

        private static void SetParentStateValue(Stack<IDeserializerState> state, IDeserializerState currentState)
        {
            var parentState = state.Peek();
            parentState.SetValue(currentState.ObjectHolder);
        }

        private static object ReadComplexObject(ref Utf8JsonReader reader, Stack<IDeserializerState> state, IDictionary<string, ReferenceInfo> referencesMap)
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
                            newState = CreateNewDeserializerState(reader, currentState, currentState.PropertyName);
                        }
                        else
                        {
                            currentState = state.Peek();
                            newState = CreateNewDeserializerState(reader, currentState, string.Empty);
                        }
                        referencesMap.Add(id, new ReferenceInfo(newState.ObjectHolder, newState.ObjectTypeInfo));
                        break;

                    default:
                        currentState = state.Peek();
                        newState = CreateNewDeserializerState(reader, currentState, currentState.PropertyName);
                        break;
                }
            }

            state.Push(newState);

            return newState.ObjectHolder;
        }

        private static IDeserializerState CreateNewDeserializerState(
            Utf8JsonReader reader,
            IDeserializerState currentState,
            string newStatePropertyName)
        {
            if (currentState.ObjectTypesInfo?.Length > 1)
            {
                return ArrayDeserializerState.Create(reader, currentState.ObjectTypesInfo);
            }
            
            var newTypeInfo = string.IsNullOrEmpty(newStatePropertyName) ?
                currentState.ObjectTypeInfo :
                JsonTypeInfoCache.GetOrAddTypeInfo(currentState.ObjectTypeInfo, newStatePropertyName);

            if (newTypeInfo.ObjectType.IsCollection() && !newTypeInfo.ObjectType.IsArray)
            {
                return CollectionDeserializerState.Create(newTypeInfo, newStatePropertyName);
            }

            if (reader.TokenType == JsonTokenType.StartArray || newTypeInfo.ObjectType.IsArray)
            {
                return ArrayDeserializerState.Create(reader, newTypeInfo, newStatePropertyName);
            }

            if (newTypeInfo.ObjectType == typeof(object))
            {
                return DynamicDeserializerState.Create(newTypeInfo);
            }

            return ObjectDeserializerState.Create(newTypeInfo);
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