using System;
using System.Text.Json;

namespace Xilium.CefGlue.Common.Shared.Serialization;

public static class JsonReaderExtensions
{
    public static object GetNumber(this Utf8JsonReader reader) {
        reader.AssertToken(JsonTokenType.Number);

        if (reader.TryGetInt64(out var longVal)) {
            return longVal;
        }

        return reader.GetDouble();
    }

    public static void AssertToken(this Utf8JsonReader reader, JsonTokenType token) {
        if (reader.TokenType != token) {
            throw new InvalidOperationException($"Expected token {token} but got {reader.TokenType} instead");
        }
    }

    public static void ReadToken(this ref Utf8JsonReader reader, JsonTokenType token) {
        reader.AssertToken(token);
        reader.Read();
    }

    public static string ReadString(this ref Utf8JsonReader reader) {
        var value = reader.GetString();
        reader.Read();
        return value;
    }

    public static string ReadPropertyName(this ref Utf8JsonReader reader) {
        reader.AssertToken(JsonTokenType.PropertyName);
        var propertyName = reader.GetString();
        reader.Read();
        return propertyName;
    }
}