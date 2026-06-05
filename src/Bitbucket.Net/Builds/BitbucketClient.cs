using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Models.Builds.Requests;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides build status related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base build status URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the build-status root.</returns>
    protected IFlurlRequest GetBuildsUrl() => GetBaseUrl("/build-status");

    /// <summary>
    /// Gets the build status URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the build-status root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the build-status path.</returns>
    protected IFlurlRequest GetBuildsUrl(string path) => GetBuildsUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves build statistics for a specific commit.
    /// </summary>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="includeUnique">Whether to include unique build statistics.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Build statistics for the commit.</returns>
    public async Task<BuildStats> GetBuildStatsForCommitAsync(string commitId, bool includeUnique = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var response = await GetBuildsUrl($"/commits/stats/{commitId}")
            .SetQueryParam("includeUnique", BitbucketHelpers.BoolToString(includeUnique))
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BuildStats>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves build statistics for multiple commits.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="commitIds">The commit identifiers.</param>
    /// <returns>A dictionary mapping commit IDs to build statistics.</returns>
    public async Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(CancellationToken cancellationToken, params string[] commitIds)
    {
        var response = await GetBuildsUrl("/commits/stats")
            .SendAsync(HttpMethod.Post, CreateJsonContent(commitIds), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, BuildStats>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves build statistics for multiple commits using the default cancellation token.
    /// </summary>
    /// <param name="commitIds">The commit identifiers.</param>
    /// <returns>A dictionary mapping commit IDs to build statistics.</returns>
    public async Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(params string[] commitIds)
    {
        return await GetBuildStatsForCommitsAsync(default, commitIds).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves build status entries for a specific commit.
    /// </summary>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of build status entries.</returns>
    public Task<IReadOnlyList<BuildStatus>> GetBuildStatusForCommitAsync(string commitId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedAsync<BuildStatus>(
            GetBuildsUrl($"/commits/{commitId}"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Associates a build status with a commit.
    /// </summary>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="request">The build status request to associate.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the association was successful; otherwise, <c>false</c>.</returns>
    public async Task<bool> AssociateBuildStatusWithCommitAsync(string commitId, AssociateBuildStatusRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        ArgumentNullException.ThrowIfNull(request);

        var response = await GetBuildsUrl($"/commits/{commitId}")
            .SendAsync(HttpMethod.Post, CreateJsonContent(request), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}