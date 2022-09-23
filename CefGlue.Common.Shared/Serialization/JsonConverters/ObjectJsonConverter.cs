using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object>
    {
        private const string JsonAttributeIdPropName = "$id";
        private const string JsonAttributeRefPropName = "$ref";
        private const string JsonAttributeValuesPropName = "$values";
        private HashSet<string> _listObjectReferenceIds;

        internal void ResetState() => _listObjectReferenceIds = null;

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var number))
                    {
                        return number;
                    }

                    return reader.GetDouble();

                case JsonTokenType.String:
                    var value = reader.GetString();
                    if (value.Length >= DataMarkers.MarkerLength)
                    {
                        switch (value.Substring(0, DataMarkers.MarkerLength))
                        {
                            case DataMarkers.StringMarker:
                                return value.Substring(DataMarkers.MarkerLength);

                            case DataMarkers.DateTimeMarker:
                                return JsonSerializer.Deserialize<DateTime>("\"" + value.Substring(DataMarkers.MarkerLength) + "\"");

                            case DataMarkers.BinaryMarker:
                                return Convert.FromBase64String(value.Substring(DataMarkers.MarkerLength));
                        }
                    }
                    return value;

                case JsonTokenType.StartArray:
                    return JsonDocument.ParseValue(ref reader).Deserialize<object[]>(options);

                case JsonTokenType.StartObject:
                    return CheckIfObjectIsListWithReferences(ref reader)
                        ? (object)JsonDocument.ParseValue(ref reader).Deserialize<List<object>>(options)
                        : JsonDocument.ParseValue(ref reader).Deserialize<Dictionary<string, object>>(options);

                default:
                    return JsonDocument.ParseValue(ref reader).RootElement.Clone();
            }
        }

        /// <summary>
        /// Peeks the reader so we can look ahead and check if we're dealing with a List<object> - {"$id"="N","$values"="[....]"}
        /// </summary>
        /// <param name="reader">Despite receiving the 'reader' by ref, this method doesn't change it.
        /// It's by ref only because this way it uses less resources, namely, the deserialization is able to process deeper structures without hitting a StackOverflowException.
        /// For reference, having the reader passed as ref allows a List<object> with 248 levels deepness to be processed Versus 226 levels if it's passed by value.</param>
        /// <returns></returns>
        private bool CheckIfObjectIsListWithReferences(ref Utf8JsonReader reader)
        {
            // create a copy of the reader argument to work with it inside of this method without affecting the ref
            // Utf8JsonReader is a structure so it's enough to pass it to another variable to create a separate instance
            // What's the purpose of the readerClone variable? so the method can look ahead and check if it's dealing with a List<object>,
            // which is represented by the pattern - {"$id"="N","$values"="[....]"}
            var readerClone = reader;

            // this is only possible because the Utf8JsonReader is a structure
            if (readerClone.TokenType != JsonTokenType.StartObject)
            {
                return false;
            }

            // check for the $id
            var propertyName = String.Empty;
            var propertyValue = string.Empty;
            if (!readerClone.Read()
                || readerClone.TokenType != JsonTokenType.PropertyName
                || !TryReadStringValue(ref readerClone, JsonTokenType.PropertyName, ref propertyName)
                || propertyName != JsonAttributeIdPropName)
            {
                // read through and check the "$ref" value
                if (propertyName == JsonAttributeRefPropName
                    && readerClone.Read()
                    && TryReadStringValue(ref readerClone, JsonTokenType.String,  ref propertyValue))
                {
                    return IsReferenceIdForListObject(propertyValue);
                }
                return false;
            }

            // read through the "$id" value
            readerClone.Read();
            if (!TryReadStringValue(ref readerClone, JsonTokenType.String,  ref propertyValue) || string.IsNullOrEmpty(propertyValue))
            {
                return false;
            }

            // check if next token is the $values propertyName
            if (!readerClone.Read() || readerClone.TokenType != JsonTokenType.PropertyName || readerClone.GetString() != JsonAttributeValuesPropName)
            {
                return false;
            }

            // reaching here, means that this object matches the pattern it's looking for - {"$id"="N","$values"="[....]"}
            StoreListObjectReferenceId(propertyValue);
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

        private void StoreListObjectReferenceId(string id)
        {
            if (_listObjectReferenceIds == null)
            {
                _listObjectReferenceIds = new HashSet<string>();
            }

            _listObjectReferenceIds.Add(id);
        }

        private bool IsReferenceIdForListObject(string referenceId)
        {
            return _listObjectReferenceIds?.Contains(referenceId) == true;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}