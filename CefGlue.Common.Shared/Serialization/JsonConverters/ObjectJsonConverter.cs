using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object>
    {
        private record ReadState
        {
            public ReadState(object objectHolder) : this(arrayIndex: null, objectHolder) { }
            
            public ReadState(int? arrayIndex, object objectHolder)
            {
                ArrayIndex = arrayIndex;
                ObjectHolder = objectHolder;
            }

            public object ObjectHolder { get; }

            public string PropertyName { get; set; }

            public int? ArrayIndex { get; set; }
        }

        private static readonly ReadState ListWrapperMarker = new ReadState(objectHolder: null);

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var state = new Stack<ReadState>();
            var root = new object[1];
            var referencesMap = new Dictionary<string, object>();

            state.Push(new ReadState(0, root));

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
                        value = reader.GetNumber();
                        break;

                    case JsonTokenType.String:
                        value = reader.GetObjectFromString();
                        break;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        value = reader.GetBoolean();
                        break;

                    case JsonTokenType.PropertyName:
                        currentState.PropertyName = reader.GetString();
                        continue;

                    case JsonTokenType.StartArray:
                        value = new object[reader.PeekAndCalculateArraySize()];
                        state.Push(new ReadState(0, value));
                        break;

                    case JsonTokenType.EndArray:
                        state.Pop();
                        continue;

                    case JsonTokenType.StartObject:
                        value = ReadComplexObject(ref reader, state, referencesMap);
                        break;

                    case JsonTokenType.EndObject:
                        state.Pop();
                        continue;
                }

                SetObjectPropertyOrArrayIndex(currentState, value);
            } while (reader.Read());

            if (state.Count > 1 && state.Pop().ObjectHolder != root)
            {
                throw new InvalidOperationException("Invalid json format - missing enclosing EndArray or EndObject token(s).");
            }

            return root.First();
        }

        private static void SetObjectPropertyOrArrayIndex(ReadState state, object value)
        {
            var obj = state.ObjectHolder;
            if (state.PropertyName != null)
            {
                ((Dictionary<string, object>)obj)[state.PropertyName] = value;
            }
            else if (state.ArrayIndex != null)
            {
                ((object[])obj)[state.ArrayIndex.Value] = value;
                state.ArrayIndex++;
            }
        }

        private object ReadComplexObject(ref Utf8JsonReader reader, Stack<ReadState> state, IDictionary<string, object> referencesMap)
        {
            var tempReader = reader;
            object obj = null;
            ReadState newState = null;

            tempReader.ReadToken(JsonTokenType.StartObject);

            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                // empty object
                obj = new object();
            }
            else
            {
                // peek first prop name
                var propName = tempReader.ReadPropertyName();
                switch (propName)
                {
                    case JsonAttributes.Ref:
                        reader.ReadToken(JsonTokenType.StartObject);
                        reader.ReadPropertyName();  // skip the $ref
                        var refId = reader.GetString();
                        obj = referencesMap[refId];
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
                            obj = new object[reader.PeekAndCalculateArraySize()];
                            state.Push(ListWrapperMarker);
                            newState = new ReadState(arrayIndex: 0, objectHolder: obj);
                        }
                        obj ??= new Dictionary<string, object>();
                        referencesMap.Add(id, obj);
                        break;

                    default:
                        obj = new Dictionary<string, object>();
                        break;
                }
            }

            newState ??= new ReadState(obj);
            state.Push(newState);

            return obj;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}