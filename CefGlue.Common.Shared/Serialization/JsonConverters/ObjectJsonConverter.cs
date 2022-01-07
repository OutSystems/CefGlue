using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class ObjectJsonConverter : JsonConverter<object>
    {
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
                    if (value.StartsWith(DataMarkers.StringMarker))
                    {
                        return value.Substring(DataMarkers.StringMarker.Length);
                    }
                    if (value.StartsWith(DataMarkers.DateTimeMarker))
                    {
                        return JsonSerializer.Deserialize<DateTime>(value);
                    }
                    //if (value.StartsWith(DataMarkers.BinaryMarker))
                    {
                        throw new InvalidOperationException();
                    }
                    break;
                
                case JsonTokenType.StartArray:
                    return JsonDocument.ParseValue(ref reader).Deserialize<object[]>(options);

                case JsonTokenType.StartObject:
                    return JsonDocument.ParseValue(ref reader).Deserialize<Dictionary<string, object>>(options);

                default:
                    return JsonDocument.ParseValue(ref reader).RootElement.Clone();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}