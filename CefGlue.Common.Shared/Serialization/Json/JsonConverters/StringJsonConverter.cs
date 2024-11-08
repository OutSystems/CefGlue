using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xilium.CefGlue.Common.Shared.Serialization.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization.Json.JsonConverters
{
    internal class StringJsonConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new InvalidOperationException();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(DataMarkers.StringMarker + JsonSerializer.SerializeToNode(value));
        }
    }
}