using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Serialization.State;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class Deserializer
    {
        private const int DeserializerMaxDepth = int.MaxValue;

        private static readonly IDeserializerState ListWrapperMarker = new ReadonlyDeserializerState(value: default);

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
            if (string.IsNullOrEmpty(jsonString?.Trim()))
            {
                return default;
            }

            var reader = CreateJsonReader(jsonString);
            var rootState = new RootObjectDeserializerState(typeof(TargetType));
            Deserialize(reader, rootState);
            return (TargetType)rootState.Value;
        }

        /// <summary>
        /// Deserializes the passed jsonString argument to an instance of object[].
        /// </summary>
        /// <typeparam name="TargetTypes"></typeparam>
        /// <param name="jsonString"></param>
        /// <param name="targetTypes"></param> 
        /// <returns></returns>
        /// <exception cref="ArgumentException">When targetTypes is null or empty</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static object[] Deserialize(string jsonString, params Type[] targetTypes)
        {
            if (targetTypes == null || targetTypes.Length == 0)
            {
                throw new ArgumentException("At least one target type must be specified.", nameof(jsonString));
            }

            if (string.IsNullOrEmpty(jsonString?.Trim()))
            {
                return Array.Empty<object>();
            }

            var reader = CreateJsonReader(jsonString);
            reader.AssertToken(JsonTokenType.StartArray);
            var rootState = new ArrayDeserializerState(targetTypes.Select(t => JsonTypeInfoCache.GetOrAddTypeInfo(t)).ToList(), reader.PeekAndCalculateArraySize());
            reader.ReadToken(JsonTokenType.StartArray);

            // The Deserialize method expects a balanced state between the reader Position and number of state elements in the stack,
            // thus, we're passing 3 state objects
            Deserialize(
                reader,
                new RootObjectDeserializerState(typeof(object)),
                ListWrapperMarker,
                rootState);
            return (object[])rootState.Value;
        }

        private static Utf8JsonReader CreateJsonReader(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions() { MaxDepth = DeserializerMaxDepth });

            reader.Read();
            return reader;
        }

        private static void Deserialize(Utf8JsonReader reader, params IDeserializerState[] initialStates)
        {
            var state = new Stack<IDeserializerState>();
            var referencesMap = new Dictionary<string, object>();

            foreach (var item in initialStates)
            {
                state.Push(item);
            }
         
            do
            {
                var currentState = state.Peek();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        currentState.SetCurrentElementValue(null);
                        break;

                    case JsonTokenType.Number:
                        currentState.SetCurrentElementValue(reader.GetNumber(currentState.CurrentElementTypeInfo));
                        break;

                    case JsonTokenType.String:
                        currentState.SetCurrentElementValue(reader.Deserialize(currentState.CurrentElementTypeInfo));
                        break;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        currentState.SetCurrentElementValue(reader.GetBoolean());
                        break;

                    case JsonTokenType.PropertyName:
                        currentState.SetCurrentPropertyName(reader.GetString());
                        break;

                    case JsonTokenType.StartArray:
                        var newState = CreateNewDeserializerState(reader, currentState);
                        currentState.SetCurrentElementValue(newState.Value);
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
                            state.Peek().SetCurrentElementValue(currentState.Value);
                        }
                        break;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        // The value is only set at the EndObject so it is be able to properly deserialize not only classes, but also Structs, which are immutable so,
                        // they can only be stored in an object after all of the properties/fields that were set in the deserialization
                        state.Peek().SetCurrentElementValue(currentState.Value);
                        break;
                }
            } while (reader.Read());

            if (state.Count > 1 && state.Pop() != initialStates.First())
            {
                throw new InvalidOperationException("Invalid json format: missing enclosing EndArray or EndObject token(s).");
            }
        }

        private static object ReadComplexObject(ref Utf8JsonReader reader, Stack<IDeserializerState> state, IDictionary<string, object> referencesMap)
        {
            var tempReader = reader;
            IDeserializerState newState;

            tempReader.ReadToken(JsonTokenType.StartObject);

            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                // empty object (e.g. deserializing a new object() results in "{}")
                newState = ReadonlyDeserializerState.Default;
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
                        if (!referencesMap.TryGetValue(refId, out var refObj))
                        {
                            throw new InvalidOperationException($"Invalid json format: can't resolve $ref '{refId}'.");
                        }
                        newState = new ReadonlyDeserializerState(refObj);
                        break;

                    case JsonAttributes.Id:
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName(); // skip the $id
                        var id = tempReader.ReadString();
                        var currentState = state.Peek();
                        if (tempReader.ReadPropertyName() == JsonAttributes.Values)
                        {
                            // it's a list
                            reader.Read(); // skip the $id
                            reader.ReadPropertyName(); // advance reader to the beginning of the list

                            state.Push(ListWrapperMarker);
                        }
                        newState = CreateNewDeserializerState(reader, currentState);
                        referencesMap.Add(id, newState.Value);
                        break;

                    default:
                        newState = CreateNewDeserializerState(reader, state.Peek());
                        break;
                        
                }
            }

            state.Push(newState);

            return newState.Value;
        }

        private static IDeserializerState CreateNewDeserializerState(Utf8JsonReader reader, IDeserializerState currentState)
        {
            var targetTypeInfo = currentState.CurrentElementTypeInfo;

            return targetTypeInfo.ObjectKind switch
            {
                JsonTypeInfo.Kind.Collection => new CollectionDeserializerState(targetTypeInfo),
                _ when reader.TokenType == JsonTokenType.StartArray => new ArrayDeserializerState(targetTypeInfo.EnumerableElementTypeInfo ?? JsonTypeInfoCache.ObjectArrayTypeInfo.EnumerableElementTypeInfo, reader.PeekAndCalculateArraySize()),
                JsonTypeInfo.Kind.Object when targetTypeInfo.ObjectType == typeof(object) => new CollectionDeserializerState(JsonTypeInfoCache.ObjectDictionaryTypeInfo),
                _ => new ObjectDeserializerState(targetTypeInfo)
            };
        }
    }
}