using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xilium.CefGlue.Common.Shared.Serialization;

namespace CefGlue.Tests.Serialization
{
    internal class StringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var value = reader.GetString();
                    if (value.Length >= DataMarkers.MarkerLength)
                    {
                        switch (value.Substring(0, DataMarkers.MarkerLength))
                        {
                            case DataMarkers.StringMarker:
                                return value.Substring(DataMarkers.MarkerLength);

                            case DataMarkers.DateTimeMarker:
                                return JsonSerializer.Deserialize<DateTime>("\"" + value.Substring(DataMarkers.MarkerLength) + "\"").ToString();

                            case DataMarkers.BinaryMarker:
                                return Convert.FromBase64String(value.Substring(DataMarkers.MarkerLength)).ToString();
                        }
                    }
                    return value;

                default:
                    return JsonDocument.ParseValue(ref reader).RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException();
        }
    }
}