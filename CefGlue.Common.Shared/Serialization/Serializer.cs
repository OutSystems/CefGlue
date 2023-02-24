using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Xilium.CefGlue.Common.Shared.Serialization
{
    internal static class Serializer
    {
        private const int SerializerMaxDepth = int.MaxValue;
        
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
        {
            Converters =
            {
                new StringJsonConverter(),
                new DateTimeJsonConverter(),
                new BinaryJsonConverter()
            },
            IncludeFields = true,
            MaxDepth = SerializerMaxDepth,
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        internal static string Serialize(object value)
        {
            try
            {
                return JsonSerializer.Serialize(value, _jsonSerializerOptions);
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
