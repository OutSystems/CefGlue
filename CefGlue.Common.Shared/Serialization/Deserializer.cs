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

        private record ReferenceInfo(object Object, JsonTypeInfo TypeInfo);
        
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
                throw new ArgumentException("At least one target type must be specified.", nameof(jsonString));
            }

            if (jsonString == null)
            {
                return targetTypes.Length > 1 ? Array.Empty<object>() : Activator.CreateInstance(targetTypes.First(), nonPublic: true);
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
                        currentState.SetValue(reader.Deserialize(currentState.GetPropertyType()));
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
                            state.Peek().SetValue(currentState.ObjectHolder);
                        }
                        break;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        // The value is only set at the EndObject so it is be able to properly deserialize not only classes, but also Structs, which are immutable so,
                        // they can only be stored in an object after all of the properties/fields that were set in the deserialization
                        state.Peek().SetValue(currentState.ObjectHolder);
                        break;
                }
            } while (reader.Read());

            if (state.Count > 1 && state.Pop().ObjectHolder != root)
            {
                throw new InvalidOperationException("Invalid json format - missing enclosing EndArray or EndObject token(s).");
            }

            return root.First();
        }

        private static object ReadComplexObject(ref Utf8JsonReader reader, Stack<IDeserializerState> state, IDictionary<string, ReferenceInfo> referencesMap)
        {
            var tempReader = reader;
            IDeserializerState newState;
            
            tempReader.ReadToken(JsonTokenType.StartObject);

            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                // empty object (e.g. deserializing a new object() results in "{}")
                newState = new ReadonlyDeserializerState(new object(), JsonTypeInfoCache.GetOrAddTypeInfo(typeof(object)));
            }
            else
            {
                // peek first field name
                var propName = tempReader.ReadPropertyName();
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
                        if (!referencesMap.TryGetValue(refId, out var refInfo))
                        {
                            throw new InvalidOperationException($"Invalid json format - can resolve $ref - '{refId}'.");
                        }
                        newState = new ReadonlyDeserializerState(refInfo.Object, refInfo.TypeInfo);
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

                            var currentState = state.Peek();
                            state.Push(ListWrapperMarker);
                            newState = CreateNewDeserializerState(reader, currentState, currentState.PropertyName);
                        }
                        else
                        {
                            newState = CreateNewDeserializerState(reader, state.Peek(), string.Empty);
                        }
                        referencesMap.Add(id, new ReferenceInfo(newState.ObjectHolder, newState.ObjectTypeInfo));
                        break;

                    default:
                        { 
                            var currentState = state.Peek();
                            newState = CreateNewDeserializerState(reader, currentState, currentState.PropertyName);
                            break;
                        }
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
                return new ArrayDeserializerState(reader, currentState.ObjectTypesInfo);
            }
            
            var newTypeInfo = string.IsNullOrEmpty(newStatePropertyName) ?
                currentState.ObjectTypeInfo :
                JsonTypeInfoCache.GetOrAddTypeInfo(currentState.ObjectTypeInfo, newStatePropertyName);

            return newTypeInfo.ObjectKind switch
            {
                JsonTypeInfo.Kind.Collection => new CollectionDeserializerState(newTypeInfo, newStatePropertyName),
                _ when reader.TokenType == JsonTokenType.StartArray => new ArrayDeserializerState(reader, newTypeInfo, newStatePropertyName),
                JsonTypeInfo.Kind.GenericObject => new DynamicDeserializerState(newTypeInfo),
                _ => new ObjectDeserializerState(newTypeInfo)
            };
        }
    }
}