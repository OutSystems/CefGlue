using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object?>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? propertyName = null;
            var objectsStack = new Stack<object>();
            var root = new object[1];
            var arraysIndexesStack = new Stack<int>();
            arraysIndexesStack.Push(0);
            objectsStack.Push(root);

            do
            {
                object value = null;
                var currentObject = objectsStack.Peek();

                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        value = null;
                        break;

                    case JsonTokenType.Number:
                        value = reader.TryGetInt64(out var number)
                            ? number
                            : (object)reader.GetDouble();
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
                        propertyName = reader.GetString();
                        continue;

                    case JsonTokenType.StartArray:
                        value = new object[GetArraySize(reader)];
                        objectsStack.Push(value);
                        break;

                    case JsonTokenType.EndArray:
                        objectsStack.Pop();
                        arraysIndexesStack.Pop();
                        continue;

                    case JsonTokenType.StartObject:
                        value = new Dictionary<string, object>();
                        objectsStack.Push(value);
                        break;

                    case JsonTokenType.EndObject:
                        propertyName = null;
                        objectsStack.Pop();
                        continue;
                }

                SetObjectPropertyOrArrayIndex(currentObject, ref propertyName, arraysIndexesStack, value);

                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    arraysIndexesStack.Push(0);
                }
            } while (reader.Read());

            if (objectsStack.Count > 1 && objectsStack.Pop() != root)
            {
                throw new InvalidOperationException();
            }

            return root.First();
        }

        private static void SetObjectPropertyOrArrayIndex(object obj, ref string? propertyName, Stack<int> arraysIndexesStack, object value)
        {
            if (propertyName != null)
            {
                ((Dictionary<string, object>)obj)[propertyName] = value;
                propertyName = null;
            }
            else if (obj is object[] array)
            {
                var arrayIndex = arraysIndexesStack.Pop();
                array[arrayIndex] = value;
                arraysIndexesStack.Push(++arrayIndex);
            }
        }

        private int GetArraySize(Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidOperationException();
            }

            int nestedArrays = 0;
            int arraySize = 0;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray:
                        if (nestedArrays == 0)
                        {
                            arraySize++;
                        }
                        nestedArrays++;
                        break;

                    case JsonTokenType.EndArray:
                        if (nestedArrays == 0)
                        {
                            return arraySize;
                        }
                        nestedArrays--;
                        break;

                    case JsonTokenType.EndObject:
                    case JsonTokenType.PropertyName:
                        break;

                    default:
                        if (nestedArrays == 0)
                        {
                            arraySize++;
                        }
                        break;
                }
            }

            return arraySize;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}