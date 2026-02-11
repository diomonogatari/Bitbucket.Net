using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides repository listing Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base repositories URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the repos endpoint.</returns>
    private IFlurlRequest GetReposUrl() => GetBaseUrl()
        .AppendPathSegment("/repos");

    /// <summary>
    /// Retrieves repositories accessible to the current user.
    /// </summary>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="name">Optional repository name filter.</param>
    /// <param name="projectName">Optional project name filter.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="isPublic">Whether to include only public repositories.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of repositories.</returns>
    public async Task<IReadOnlyList<Repository>> GetRepositoriesAsync(
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? name = null,
        string? projectName = null,
        Permissions? permission = null,
        bool isPublic = false,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["projectname"] = projectName,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["visibility"] = isPublic ? "public" : "private",
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetReposUrl()
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Repository>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all repositories accessible to the current user, yielding items as they are retrieved.
    /// </summary>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="name">Optional repository name filter.</param>
    /// <param name="projectName">Optional project name filter.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="isPublic">Whether to include only public repositories.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of repositories.</returns>
    public IAsyncEnumerable<Repository> GetRepositoriesStreamAsync(
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? name = null,
        string? projectName = null,
        Permissions? permission = null,
        bool isPublic = false,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["projectname"] = projectName,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["visibility"] = isPublic ? "public" : "private",
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetReposUrl()
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Repository>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }
}