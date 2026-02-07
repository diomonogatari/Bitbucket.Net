using Bitbucket.Net.Common;
using Flurl.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetMarkupUrl() => GetBaseUrl()
        .AppendPathSegment("/markup");

    private IFlurlRequest GetMarkupUrl(string path) => GetMarkupUrl()
        .AppendPathSegment(path);

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
            .PostJsonAsync(new StringContent(text), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.GetProperty("html").GetString()!;
        }, cancellationToken)
            .ConfigureAwait(false);
    }
}