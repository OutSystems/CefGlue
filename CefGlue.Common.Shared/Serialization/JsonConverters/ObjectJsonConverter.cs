using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object?>
    {
        internal const string JsonAttributeIdPropName = "$id";
        internal const string JsonAttributeRefPropName = "$ref";
        private const string JsonAttributeValuesPropName = "$values";

        private class ReadState
        {
            public ReadState(string propertyName, int? arrayIndex, bool hasReference, object objectHolder)
            {
                PropertyName = propertyName;
                ArrayIndex = arrayIndex;
                HasReference = hasReference;
                ObjectHolder = objectHolder;
            }

            public readonly bool HasReference;
            public readonly object ObjectHolder;

            public string PropertyName;
            public int? ArrayIndex;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stateStack = new Stack<ReadState>();
            var root = new object[1];
            var referencesMap = new Dictionary<string, object>();

            stateStack.Push(new ReadState(null, 0, false, root));

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
                        var stringValue = reader.GetString();
                        value = stringValue;
                        if (stringValue.Length >= DataMarkers.MarkerLength)
                        {
                            switch (stringValue.Substring(0, DataMarkers.MarkerLength))
                            {
                                case DataMarkers.StringMarker:
                                    value = stringValue.Substring(DataMarkers.MarkerLength);
                                    break;

                                case DataMarkers.DateTimeMarker:
                                    value = JsonSerializer.Deserialize<DateTime>("\"" + stringValue.Substring(DataMarkers.MarkerLength) + "\"");
                                    break;

                                case DataMarkers.BinaryMarker:
                                    value = Convert.FromBase64String(stringValue.Substring(DataMarkers.MarkerLength));
                                    break;
                            }
                        }
                        break;

                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        value = reader.GetBoolean();
                        break;

                    case JsonTokenType.PropertyName:
                        currentState.PropertyName = reader.GetString();
                        continue;

                    case JsonTokenType.StartArray:
                        value = new object[PeekAndCalculateArraySize(reader)];
                        stateStack.Push(new ReadState(null, 0, false, value));
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
                        stateStack.Push(new ReadState(null, isNewArray ? 0 : null, true, value));
                        break;

                    case JsonTokenType.EndObject:
                        stateStack.Pop();
                        continue;
                }

                SetObjectPropertyOrArrayIndex(currentState, value);
            } while (reader.Read());

            if (stateStack.Count > 1 && stateStack.Pop().ObjectHolder != root)
            {
                throw new InvalidOperationException();
            }

            return root.First();
        }

        private static void SetObjectPropertyOrArrayIndex(
            ReadState state,
            object value)
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
            var propName = tempReader.ReadPropertyName();

            var isId = propName == JsonAttributeIdPropName;
            var isRef = !isId && propName == JsonAttributeRefPropName;
            if (!isId && !isRef)
            {
                value = null;
                return false;
            }

            reader.Read(); // StartObject
            reader.Read(); // PropertyName
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
            if (propName == JsonAttributeValuesPropName)
            {
                reader.Read(); // objId
                reader.Read(); // $values PropertyName
                value = new object[PeekAndCalculateArraySize(reader)];
                isNewArray = true;
            }
            else
            {
                value = new Dictionary<string, object>();
            }
            referencesMap.Add(objId, value);
            return true;
        }

        private int PeekAndCalculateArraySize(Utf8JsonReader reader)
        {
            var arraySize = 0;
            var startDepth = reader.CurrentDepth;
            reader.ReadToken(JsonTokenType.StartArray);
            while (reader.CurrentDepth > startDepth || reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.CurrentDepth == startDepth + 1
                    && reader.TokenType != JsonTokenType.EndArray
                    && reader.TokenType != JsonTokenType.EndObject
                    && reader.TokenType != JsonTokenType.PropertyName
                    && reader.TokenType != JsonTokenType.Comment)
                {
                    arraySize++;
                }
                reader.Read();
            }
            return arraySize;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}