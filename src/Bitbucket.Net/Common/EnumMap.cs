using System.Collections.Frozen;

namespace Bitbucket.Net.Common;

/// <summary>
/// Holds the canonical enum-to-string and string-to-enum mappings for a Bitbucket API enum type.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
public sealed class EnumMap<TEnum> where TEnum : struct, Enum
{
    /// <summary>
    /// Enum-to-API-string lookup (forward mapping).
    /// </summary>
    public FrozenDictionary<TEnum, string> Forward { get; }

    /// <summary>
    /// API-string-to-enum lookup (reverse mapping). May be empty for query-param-only enums.
    /// </summary>
    public FrozenDictionary<string, TEnum> Reverse { get; }

    /// <summary>
    /// Creates a new enum map with bidirectional lookup.
    /// </summary>
    /// <param name="mappings">The enum-to-string mapping dictionary.</param>
    public EnumMap(Dictionary<TEnum, string> mappings)
    {
        Forward = mappings.ToFrozenDictionary();
        Reverse = mappings.ToFrozenDictionary(
            kv => kv.Value,
            kv => kv.Key,
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a new enum map with forward-only lookup (no reverse).
    /// </summary>
    /// <param name="mappings">The enum-to-string mapping dictionary.</param>
    /// <param name="createReverse">When false, no reverse dictionary is created.</param>
    public EnumMap(Dictionary<TEnum, string> mappings, bool createReverse)
    {
        Forward = mappings.ToFrozenDictionary();
        Reverse = createReverse
            ? mappings.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            : FrozenDictionary<string, TEnum>.Empty;
    }

    /// <summary>
    /// Converts an enum value to its API string representation.
    /// </summary>
    /// <exception cref="ArgumentException">The enum value has no known mapping.</exception>
    public string ToApiString(TEnum value)
    {
        if (!Forward.TryGetValue(value, out string? result))
        {
            throw new ArgumentException($"Unknown {typeof(TEnum).Name} value: {value}");
        }

        return result;
    }

    /// <summary>
    /// Converts a nullable enum value to its API string representation.
    /// </summary>
    public string? ToApiString(TEnum? value) => value.HasValue ? ToApiString(value.Value) : null;

    /// <summary>
    /// Converts an API string to its enum value.
    /// </summary>
    /// <exception cref="ArgumentException">The string has no known reverse mapping.</exception>
    public TEnum FromApiString(string value)
    {
        if (!Reverse.TryGetValue(value, out TEnum result))
        {
            throw new ArgumentException($"Unknown {typeof(TEnum).Name} string: {value}");
        }

        return result;
    }
}