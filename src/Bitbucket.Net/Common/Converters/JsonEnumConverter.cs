using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// JSON converter for enums that use a <see cref="EnumMap{TEnum}"/> for bidirectional mapping.
/// </summary>
public sealed class JsonEnumConverter<TEnum>(EnumMap<TEnum> map) : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return map.FromApiString(reader.GetString()!);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(map.ToApiString(value));
    }
}

/// <summary>
/// JSON converter for lists of enums using a <see cref="EnumMap{TEnum}"/> for bidirectional mapping.
/// </summary>
public sealed class JsonEnumListConverter<TEnum>(EnumMap<TEnum> map) : JsonConverter<List<TEnum>?>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public override List<TEnum>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Expected StartArray token, got {reader.TokenType}.");
        }

        var items = new List<TEnum>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return items;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                items.Add(map.FromApiString(reader.GetString()!));
            }
        }

        throw new JsonException("Unexpected end of JSON while reading array.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, List<TEnum>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (var item in value)
        {
            writer.WriteStringValue(map.ToApiString(item));
        }
        writer.WriteEndArray();
    }
}