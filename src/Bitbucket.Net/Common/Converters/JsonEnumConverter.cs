using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// Abstract base class for custom enum converters that convert between enum values and their string representations.
/// </summary>
/// <typeparam name="TEnum">The enum type to convert.</typeparam>
public abstract class JsonEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Converts an enum value to its string representation.
    /// </summary>
    protected abstract string ConvertToString(TEnum value);

    /// <summary>
    /// Converts a string representation to its enum value.
    /// </summary>
    protected abstract TEnum ConvertFromString(string s);

    /// <inheritdoc />
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string value = reader.GetString()!;
            return ConvertFromString(value);
        }

        throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum {typeof(TEnum).Name}.");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ConvertToString(value));
    }
}

/// <summary>
/// Abstract base class for custom enum list converters that convert between lists of enum values and their JSON array representations.
/// </summary>
/// <typeparam name="TEnum">The enum type to convert.</typeparam>
public abstract class JsonEnumListConverter<TEnum> : JsonConverter<List<TEnum>?>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Converts an enum value to its string representation.
    /// </summary>
    protected abstract string ConvertToString(TEnum value);

    /// <summary>
    /// Converts a string representation to its enum value.
    /// </summary>
    protected abstract TEnum ConvertFromString(string s);

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
                items.Add(ConvertFromString(reader.GetString()!));
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
            writer.WriteStringValue(ConvertToString(item));
        }
        writer.WriteEndArray();
    }
}