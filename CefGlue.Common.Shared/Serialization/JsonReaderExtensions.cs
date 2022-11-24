using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class JsonReaderExtensions
    {
        internal static object GetNumber(this Utf8JsonReader reader)
        {
            reader.AssertToken(JsonTokenType.Number);

            if (reader.TryGetInt64(out var longVal))
            {
                return longVal;
            }

            return reader.GetDouble();
        }

        /// <summary>
        /// This method gets a string from the reader, checks for a DataMarker as prefix of the string and returns the correct object if a DataMarker is found.
        /// The possible DataMarkers are StringMarker, DateTimeMarker and BinaryMarker.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>
        /// A String if no DataMarker or StringMarker is found.
        /// A DateTime if DateTimeMarker is found.
        /// A byte[] if BinaryMarker is found.
        /// </returns>
        internal static object GetObjectFromString(this Utf8JsonReader reader)
        {
            var stringValue = reader.GetString();
            if (stringValue.Length >= DataMarkers.MarkerLength)
            {
                switch (stringValue.Substring(0, DataMarkers.MarkerLength))
                {
                    case DataMarkers.StringMarker:
                        return stringValue.Substring(DataMarkers.MarkerLength);

                    case DataMarkers.DateTimeMarker:
                        return JsonSerializer.Deserialize<DateTime>("\"" + stringValue.Substring(DataMarkers.MarkerLength) + "\"");

                    case DataMarkers.BinaryMarker:
                        return Convert.FromBase64String(stringValue.Substring(DataMarkers.MarkerLength));
                }
            }

            return stringValue;
        }

        private static void AssertToken(this Utf8JsonReader reader, JsonTokenType token)
        {
            if (reader.TokenType != token)
            {
                throw new InvalidOperationException($"Expected token {token} but got {reader.TokenType} instead");
            }
        }

        internal static void ReadToken(this ref Utf8JsonReader reader, JsonTokenType token)
        {
            reader.AssertToken(token);
            reader.Read();
        }

        internal static string ReadPropertyName(this ref Utf8JsonReader reader)
        {
            reader.AssertToken(JsonTokenType.PropertyName);
            var propertyName = reader.GetString();
            reader.Read();
            return propertyName;
        }

        internal static string ReadString(this ref Utf8JsonReader reader)
        {
            reader.AssertToken(JsonTokenType.String);
            var result = reader.GetString();
            reader.Read();
            return result;
        }

        internal static int PeekAndCalculateArraySize(this Utf8JsonReader reader)
        {
            var arraySize = 0;
            var startDepth = reader.CurrentDepth;
            reader.ReadToken(JsonTokenType.StartArray);
            while (reader.CurrentDepth > startDepth || reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.CurrentDepth == startDepth + 1 &&
                    reader.TokenType != JsonTokenType.EndArray &&
                    reader.TokenType != JsonTokenType.EndObject &&
                    reader.TokenType != JsonTokenType.Comment)
                {
                    arraySize++;
                }
                reader.Read();
            }
            return arraySize;
        }
    }
}