using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Common.Converters;

/// <summary>
/// Converts Unix timestamps (seconds since epoch) to/from DateTimeOffset.
/// </summary>
public sealed class UnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => default,
            JsonTokenType.Number when reader.TryGetInt64(out long unixTime) => unixTime.FromUnixTimeSeconds(),
            JsonTokenType.String when long.TryParse(reader.GetString(), out long unixTime) => unixTime.FromUnixTimeSeconds(),
            _ => throw new JsonException($"Cannot convert {reader.TokenType} to {nameof(DateTimeOffset)}."),
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}

/// <summary>
/// Converts Unix timestamps (seconds since epoch) to/from nullable DateTimeOffset.
/// </summary>
public sealed class NullableUnixDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
{
    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.Number when reader.TryGetInt64(out long unixTime) => unixTime.FromUnixTimeSeconds(),
            JsonTokenType.String when string.IsNullOrEmpty(reader.GetString()) => null,
            JsonTokenType.String when long.TryParse(reader.GetString(), out long unixTime) => unixTime.FromUnixTimeSeconds(),
            _ => throw new JsonException($"Cannot convert {reader.TokenType} to nullable {nameof(DateTimeOffset)}."),
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}