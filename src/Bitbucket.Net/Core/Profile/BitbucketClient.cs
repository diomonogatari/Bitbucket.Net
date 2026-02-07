using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides profile-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base profile URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the profile endpoint.</returns>
    private IFlurlRequest GetProfileUrl() => GetBaseUrl()
        .AppendPathSegment("/profile");

    /// <summary>
    /// Gets the profile URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the profile endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the profile path.</returns>
    private IFlurlRequest GetProfileUrl(string path) => GetProfileUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves recent repositories for the current user.
    /// </summary>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of recent repositories.</returns>
    public async Task<IEnumerable<Repository>> GetRecentReposAsync(Permissions? permission = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProfileUrl("/recent/repos")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Repository>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }
}