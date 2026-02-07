using Bitbucket.Net.Common;
using Flurl.Http;
using System.Text.Json;

namespace Bitbucket.Net;

/// <summary>
/// Provides markup preview Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base markup URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the markup endpoint.</returns>
    private IFlurlRequest GetMarkupUrl() => GetBaseUrl()
        .AppendPathSegment("/markup");

    /// <summary>
    /// Gets the markup URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the markup endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the markup path.</returns>
    private IFlurlRequest GetMarkupUrl(string path) => GetMarkupUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Previews markup text as HTML.
    /// </summary>
    /// <param name="text">The markup text to preview.</param>
    /// <param name="urlMode">Optional URL rendering mode.</param>
    /// <param name="hardWrap">Whether to hard wrap lines.</param>
    /// <param name="htmlEscape">Whether to HTML-escape content.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The rendered HTML.</returns>
    public async Task<string> PreviewMarkupAsync(string text,
        string? urlMode = null,
        bool? hardWrap = null,
        bool? htmlEscape = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["urlMode"] = urlMode,
            ["hardWrap"] = BitbucketHelpers.BoolToString(hardWrap),
            ["htmlEscape"] = BitbucketHelpers.BoolToString(htmlEscape),
        };

        var response = await GetMarkupUrl("/preview")
            .WithHeader("X-Atlassian-Token", "no-check")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, new StringContent(text), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.GetProperty("html").GetString()!;
        }, cancellationToken)
            .ConfigureAwait(false);
    }
}