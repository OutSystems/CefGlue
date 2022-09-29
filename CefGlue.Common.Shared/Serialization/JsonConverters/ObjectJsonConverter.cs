using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object?>
    {
        private const string JsonAttributeIdPropName = "$id";
        private const string JsonAttributeRefPropName = "$ref";
        private const string JsonAttributeValuesPropName = "$values";

        public override bool CanConvert(Type typeToConvert)
        {
            return true;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? propertyName = null;
            string? previousPropertyName = null;
            var objectsStack = new Stack<object>();
            var root = new object[1];
            var arraysIndexesStack = new Stack<int>();
            var objectReferences = new Dictionary<string, object>();
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
                        previousPropertyName = propertyName;
                        propertyName = reader.GetString();
                        continue;

                    case JsonTokenType.StartArray:
                        if (propertyName == JsonAttributeValuesPropName)
                        {
                            // entering here means that a pattern such as "{$id"="N","$values=[...]}" was found
                            // which means that the array (in this case List<object>) was already set in the StartObject token
                            // as such, disregard and continue
                            propertyName = null;
                            continue;
                        }
                        value = new object[PeekAndCalculateArraySize(reader)];
                        objectsStack.Push(value);
                        break;

                    case JsonTokenType.EndArray:
                        // check if the popped obj is an array, since it can also be a List<value>
                        // for which case no array index was pushed to the arraysIndexesStack
                        if (objectsStack.Pop() is object[])
                        {
                            arraysIndexesStack.Pop();
                        }
                        continue;

                    case JsonTokenType.StartObject:
                        if (PeekAndCheckIfIsListOfObject(reader, objectReferences, out var referencedObject))
                        {
                            // if the referencedObject is returned, it means it found a {"$ref"="N"}
                            // so, read through both values ("$ref" and "N") and proceed
                            if (referencedObject != null)
                            {
                                reader.Read();
                                reader.Read();
                            }
                            value = referencedObject ?? new List<object>();
                        }
                        else
                        {
                            value = new Dictionary<string, object>();
                        }
                        objectsStack.Push(value);
                        break;

                    case JsonTokenType.EndObject:
                        propertyName = null;
                        objectsStack.Pop();
                        continue;
                }

                SetObjectPropertyOrArrayIndex(objectReferences, currentObject, arraysIndexesStack, ref propertyName, ref previousPropertyName, value);

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

        private static void SetObjectPropertyOrArrayIndex(
            Dictionary<string, object> objectReferences,
            object obj,
            Stack<int> arraysIndexesStack,
            ref string? propertyName,
            ref string? previousPropertyName,
            object value)
        {
            if (propertyName != null)
            {
                SetObjectProperty(objectReferences, obj, ref propertyName, ref previousPropertyName, value);
            }
            else if (obj is object[] array)
            {
                var arrayIndex = arraysIndexesStack.Pop();
                array[arrayIndex] = value;
                arraysIndexesStack.Push(++arrayIndex);
            }
            else if (obj is List<object> list)
            {
                list.Add(value);
            }
        }

        private static void SetObjectProperty(
            Dictionary<string, object> objectReferences,
            object obj,
            ref string propertyName,
            ref string? previousPropertyName,
            object value)
        {
            if (propertyName == JsonAttributeIdPropName)
            {
                // check the "$id" value
                if (string.IsNullOrEmpty(value as string))
                {
                    throw new JsonException("Object id expected!");
                }

                if (objectReferences.ContainsKey((string)value))
                {
                    throw new JsonException("Duplicate reference id found!");
                }

                objectReferences.Add((string)value, obj);
                propertyName = null;

                return;
            }

            if (propertyName == JsonAttributeRefPropName)
            {
                throw new InvalidOperationException();
            }

            if (propertyName == JsonAttributeValuesPropName)
            {
                throw new InvalidOperationException();
            }

            ((Dictionary<string, object>)obj)[propertyName] = value;
            propertyName = null;
        }

        private int PeekAndCalculateArraySize(Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new InvalidOperationException();
            }

            int nestedArrays = 0;
            int arraySize = 0;
            string? propertyName = null;
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
                        break;

                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        break;

                    default:
                        if (reader.TokenType == JsonTokenType.String
                            && (propertyName == JsonAttributeIdPropName
                                || propertyName == JsonAttributeRefPropName
                                || propertyName == JsonAttributeValuesPropName))
                        {
                            propertyName = null;
                            break;
                        }
                        if (nestedArrays == 0)
                        {
                            arraySize++;
                        }
                        break;
                }
            }

            return arraySize;
        }

        /// <summary>
        /// Peeks the reader so we can look ahead and check if we're dealing with a List<object> - {"$id"="N","$values"="[....]"}
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectReferences"></param>
        /// <param name="referencedObject"></param>
        /// <returns></returns>
        private bool PeekAndCheckIfIsListOfObject(Utf8JsonReader reader, Dictionary<string, object> objectReferences, out object referencedObject)
        {
            // since the Utf8JsonReader is a structure, the reader argument to work with it inside of this method will be a new instance
            referencedObject = null;

            // this is only possible because the Utf8JsonReader is a structure
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            // check for the $id
            var propertyName = String.Empty;
            var propertyValue = string.Empty;
            if (!reader.Read()
                || reader.TokenType != JsonTokenType.PropertyName
                || !TryReadStringValue(ref reader, JsonTokenType.PropertyName, ref propertyName)
                || propertyName != JsonAttributeIdPropName)
            {
                // read through and check the "$ref" value - the pattern is - {"$ref":"1"}
                if (propertyName == JsonAttributeRefPropName
                    && reader.Read()
                    && TryReadStringValue(ref reader, JsonTokenType.String,  ref propertyValue))
                {
                    return objectReferences.TryGetValue(propertyValue, out referencedObject);
                }
                return false;
            }

            // read through the "$id" value
            reader.Read();
            if (!TryReadStringValue(ref reader, JsonTokenType.String,  ref propertyValue) || string.IsNullOrEmpty(propertyValue))
            {
                return false;
            }

            // check if next token is the $values propertyName
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != JsonAttributeValuesPropName)
            {
                return false;
            }

            // reaching here, means that this object matches the pattern it's looking for - {"$id"="N","$values"="[....]"}
            return true;
        }

        private static bool TryReadStringValue(ref Utf8JsonReader reader, JsonTokenType allowedTokenType, ref string value)
        {
            if (reader.TokenType != allowedTokenType)
            {
                return false;
            }

            value = reader.GetString();
            return true;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}