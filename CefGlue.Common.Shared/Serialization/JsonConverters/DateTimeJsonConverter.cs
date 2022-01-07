using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // write date time with a special marker and the date value without quotes
            writer.WriteStringValue(DataMarkers.DateTimeMarker + JsonSerializer.SerializeToNode(value));
        }
    }
}