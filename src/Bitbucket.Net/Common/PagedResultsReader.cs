using System.Text.Json;

namespace Bitbucket.Net.Common;

/// <summary>
/// Zero-allocation reader for <c>PagedResults</c> pagination metadata.
/// Extracts <c>isLastPage</c>, <c>nextPageStart</c>, <c>start</c>, <c>limit</c>,
/// and <c>size</c> directly from UTF-8 bytes without full deserialization.
/// </summary>
internal static class PagedResultsReader
{
    /// <summary>
    /// Reads pagination metadata from a UTF-8 JSON span.
    /// </summary>
    public static PagedMetadata ReadMetadata(ReadOnlySpan<byte> json)
    {
        var reader = new Utf8JsonReader(json);
        bool isLastPage = true;
        int? nextPageStart = null;
        int? start = null;
        int? limit = null;
        int size = 0;

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            if (reader.ValueTextEquals("isLastPage"u8))
            {
                reader.Read();
                isLastPage = reader.GetBoolean();
            }
            else if (reader.ValueTextEquals("nextPageStart"u8))
            {
                reader.Read();
                nextPageStart = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("start"u8))
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.Number)
                    start = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("limit"u8))
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.Number)
                    limit = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("size"u8))
            {
                reader.Read();
                if (reader.TokenType == JsonTokenType.Number)
                    size = reader.GetInt32();
            }
            else if (reader.ValueTextEquals("values"u8))
            {
                reader.Read();
                reader.Skip();
            }
        }

        return new PagedMetadata(isLastPage, nextPageStart, start, limit, size);
    }
}