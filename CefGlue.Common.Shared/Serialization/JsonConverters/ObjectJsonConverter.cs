using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object>
    {
        private class ReadState
        {
            public ReadState(int? arrayIndex, bool hasReference, object objectHolder)
            {
                ArrayIndex = arrayIndex;
                HasReference = hasReference;
                ObjectHolder = objectHolder;
            }

            public bool HasReference { get; }

            public object ObjectHolder { get; }
            
            public string PropertyName { get; set; }
            
            public int? ArrayIndex { get; set; }
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stateStack = new Stack<ReadState>();
            var root = new object[1];
            var referencesMap = new Dictionary<string, object>();

            stateStack.Push(new ReadState(0, false, root));

            do
            {
                object value = null;
                var currentState = stateStack.Peek();

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
                        stateStack.Push(new ReadState(0, false, value));
                        break;

                    case JsonTokenType.EndArray:
                        if (!currentState.HasReference)
                        {
                            stateStack.Pop();
                        }
                        continue;

                    case JsonTokenType.StartObject:
                        if (!HandleReferences(ref reader, referencesMap, out value, out var isNewArray))
                        {
                            value = new Dictionary<string, object>();
                        }
                        stateStack.Push(new ReadState(isNewArray ? 0 : null, true, value));
                        break;

                    case JsonTokenType.EndObject:
                        stateStack.Pop();
                        continue;
                }

                SetObjectPropertyOrArrayIndex(currentState, value);
            } while (reader.Read());

            if (stateStack.Count > 1 && stateStack.Pop().ObjectHolder != root)
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

        private bool HandleReferences(ref Utf8JsonReader reader, Dictionary<string, object> referencesMap, out object value, out bool isNewArray)
        {
            isNewArray = false;
            var tempReader = reader;
            tempReader.ReadToken(JsonTokenType.StartObject);
            
            if (tempReader.TokenType == JsonTokenType.EndObject)
            {
                value = new object();
                return true;
            }
            
            var propName = tempReader.ReadPropertyName();

            var isId = propName == JsonAttributes.Id;
            var isRef = !isId && propName == JsonAttributes.Ref;
            if (!isId && !isRef)
            {
                value = null;
                return false;
            }

            reader.ReadToken(JsonTokenType.StartObject);
            reader.ReadToken(JsonTokenType.PropertyName);
            var objId = reader.GetString();

            if (isRef)
            {
                value = referencesMap[objId];
                return true;
            }

            // it's $id="N"
            // check if it's followed by $values=[...]
            tempReader.Read(); // $id Value
            propName = tempReader.ReadPropertyName();
            if (propName == JsonAttributes.Values)
            {
                reader.Read(); // objId
                reader.Read(); // $values PropertyName
                value = new object[reader.PeekAndCalculateArraySize()];
                isNewArray = true;
            }
            else
            {
                value = new Dictionary<string, object>();
            }
            referencesMap.Add(objId, value);
            return true;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}