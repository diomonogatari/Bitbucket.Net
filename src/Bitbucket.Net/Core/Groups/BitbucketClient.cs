using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides group-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base groups URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the groups endpoint.</returns>
    private IFlurlRequest GetGroupsUrl() => GetBaseUrl()
        .AppendPathSegment("/groups");

    /// <summary>
    /// Retrieves group names with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of group names.</returns>
    public async Task<IEnumerable<string>> GetGroupNamesAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetGroupsUrl()
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<string>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }
}