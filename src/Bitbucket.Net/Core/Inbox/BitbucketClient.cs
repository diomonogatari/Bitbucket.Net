using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides inbox-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base inbox URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the inbox endpoint.</returns>
    private IFlurlRequest GetInboxUrl() => GetBaseUrl()
        .AppendPathSegment("/inbox");

    /// <summary>
    /// Gets the inbox URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the inbox endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the inbox path.</returns>
    private IFlurlRequest GetInboxUrl(string path) => GetInboxUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves pull requests in the user's inbox.
    /// </summary>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size (default 25).</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="role">The participant role filter (default reviewer).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of pull requests.</returns>
    public async Task<IEnumerable<PullRequest>> GetInboxPullRequestsAsync(
        int? maxPages = null,
        int? limit = 25,
        int? start = 0,
        Roles role = Roles.Reviewer,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["role"] = BitbucketHelpers.RoleToString(role),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetInboxUrl("/pull-requests")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<PullRequest>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the count of pull requests in the user's inbox.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The number of inbox pull requests.</returns>
    public async Task<int> GetInboxPullRequestsCountAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetInboxUrl("/pull-requests/count")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.GetProperty("count").GetInt32();
        }, cancellationToken)
            .ConfigureAwait(false);
    }
}