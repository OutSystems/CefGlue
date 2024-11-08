using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Xilium.CefGlue.Common.Shared.Serialization.Json.JsonConverters;

namespace Xilium.CefGlue.Common.Shared.Serialization.Json
{
    internal class CefJsonSerializer : ISerializer
    {
        private const int SerializerMaxDepth = int.MaxValue;
        
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions()
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

        public byte[] Serialize(object value)
        {
            try
            {
                return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, _jsonSerializerOptions));
            }
            catch (JsonException e)
            {
                // wrap the json exception
                throw new InvalidOperationException(e.Message);
            }
        }
    }
}
