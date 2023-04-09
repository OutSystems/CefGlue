using System;
using System.Reflection;
using System.Text.Json;
using Xilium.CefGlue.Common.Shared.Serialization.State;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class JsonReaderExtensions
    {
        public static object GetNumber(this Utf8JsonReader reader, JsonTypeInfo targetTypeInfo)
        {
            reader.AssertToken(JsonTokenType.Number);

            switch (targetTypeInfo.TypeCode)
            {
                case TypeCode.Byte:
                    return reader.GetByte();
                case TypeCode.SByte:
                    return reader.GetSByte();
                case TypeCode.Int16:
                    return reader.GetInt16();
                case TypeCode.Int32:
                    return reader.GetInt32();
                case TypeCode.Int64:
                    return reader.GetInt64();
                case TypeCode.UInt16:
                    return reader.GetUInt16();
                case TypeCode.UInt32:
                    return reader.GetUInt32();
                case TypeCode.UInt64:
                    return reader.GetUInt64();
                case TypeCode.Single:
                    return reader.GetSingle();
                case TypeCode.Double:
                    return reader.GetDouble();
                case TypeCode.Decimal:
                    return reader.GetDecimal();
                default:
                    // e.g. convert to the object type used in ExpandoObjects
                    return Convert.ChangeType(reader.GetDouble(), targetTypeInfo.ObjectType);
            }
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
        public static object Deserialize(this Utf8JsonReader reader, JsonTypeInfo targetTypeInfo)
        {
            var stringValue = reader.GetString();
            if (stringValue.Length >= DataMarkers.MarkerLength)
            {
                switch (stringValue.Substring(0, DataMarkers.MarkerLength))
                {
                    case DataMarkers.StringMarker:
                        stringValue = stringValue.Substring(DataMarkers.MarkerLength);
                        break;

                    case DataMarkers.DateTimeMarker:
                        return JsonSerializer.Deserialize<DateTime>("\"" + stringValue.Substring(DataMarkers.MarkerLength) + "\"");

                    case DataMarkers.BinaryMarker:
                        return Convert.FromBase64String(stringValue.Substring(DataMarkers.MarkerLength));
                }
            }

            return
                targetTypeInfo.TypeCode == TypeCode.String ?
                stringValue :
                Convert.ChangeType(stringValue, targetTypeInfo.ObjectType);
        }

        public static void AssertToken(this Utf8JsonReader reader, JsonTokenType token)
        {
            if (reader.TokenType != token)
            {
                throw new InvalidOperationException($"Expected token {token} but got {reader.TokenType} instead");
            }
        }

        public static void ReadToken(this ref Utf8JsonReader reader, JsonTokenType token)
        {
            reader.AssertToken(token);
            reader.Read();
        }

        public static string ReadPropertyName(this ref Utf8JsonReader reader)
        {
            reader.AssertToken(JsonTokenType.PropertyName);
            var propertyName = reader.GetString();
            reader.Read();
            return propertyName;
        }

        public static string ReadString(this ref Utf8JsonReader reader)
        {
            reader.AssertToken(JsonTokenType.String);
            var result = reader.GetString();
            reader.Read();
            return result;
        }

        public static int PeekAndCalculateArraySize(this Utf8JsonReader reader)
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