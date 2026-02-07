using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.RefRestrictions;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides reference restriction (branch permissions) Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base ref restrictions URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the branch permissions root.</returns>
    private IFlurlRequest GetRefRestrictionsUrl() => GetBaseUrl("/branch-permissions", "2.0");

    /// <summary>
    /// Gets the ref restrictions URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the branch permissions root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the requested branch permissions path.</returns>
    private IFlurlRequest GetRefRestrictionsUrl(string path) => GetRefRestrictionsUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves reference restrictions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="type">Optional restriction type filter.</param>
    /// <param name="matcherType">Optional matcher type filter.</param>
    /// <param name="matcherId">Optional matcher identifier filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> GetProjectRefRestrictionsAsync(string projectKey,
        RefRestrictionTypes? type = null,
        RefMatcherTypes? matcherType = null,
        string? matcherId = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["type"] = BitbucketHelpers.RefRestrictionTypeToString(type),
            ["matcherType"] = BitbucketHelpers.RefMatcherTypeToString(matcherType),
            ["matcherId"] = matcherId,
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<RefRestriction>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple reference restrictions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="refRestrictions">The reference restrictions to create.</param>
    /// <returns>The created reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
            .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
            .SendAsync(HttpMethod.Post, CreateJsonContent(refRestrictions), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<IEnumerable<RefRestriction>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple reference restrictions for a project using the default cancellation token.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="refRestrictions">The reference restrictions to create.</param>
    /// <returns>The created reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, params RefRestrictionCreate[] refRestrictions)
    {
        return await CreateProjectRefRestrictionsAsync(projectKey, default, refRestrictions).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a single reference restriction for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="refRestriction">The reference restriction to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created reference restriction.</returns>
    public async Task<RefRestriction> CreateProjectRefRestrictionAsync(string projectKey, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
            .SendAsync(HttpMethod.Post, CreateJsonContent(refRestriction), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific project reference restriction.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="refRestrictionId">The restriction identifier.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested reference restriction.</returns>
    public async Task<RefRestriction> GetProjectRefRestrictionAsync(string projectKey, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions/{refRestrictionId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a project reference restriction.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="refRestrictionId">The restriction identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the restriction was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectRefRestrictionAsync(string projectKey, int refRestrictionId, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions/{refRestrictionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves reference restrictions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="type">Optional restriction type filter.</param>
    /// <param name="matcherType">Optional matcher type filter.</param>
    /// <param name="matcherId">Optional matcher identifier filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> GetRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug,
        RefRestrictionTypes? type = null,
        RefMatcherTypes? matcherType = null,
        string? matcherId = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["type"] = BitbucketHelpers.RefRestrictionTypeToString(type),
            ["matcherType"] = BitbucketHelpers.RefMatcherTypeToString(matcherType),
            ["matcherId"] = matcherId,
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<RefRestriction>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple reference restrictions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="refRestrictions">The reference restrictions to create.</param>
    /// <returns>The created reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
            .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
            .SendAsync(HttpMethod.Post, CreateJsonContent(refRestrictions), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<IEnumerable<RefRestriction>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates multiple reference restrictions for a repository using the default cancellation token.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="refRestrictions">The reference restrictions to create.</param>
    /// <returns>The created reference restrictions.</returns>
    public async Task<IEnumerable<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, params RefRestrictionCreate[] refRestrictions)
    {
        return await CreateRepositoryRefRestrictionsAsync(projectKey, repositorySlug, default, refRestrictions).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a single reference restriction for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="refRestriction">The reference restriction to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created reference restriction.</returns>
    public async Task<RefRestriction> CreateRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
            .SendAsync(HttpMethod.Post, CreateJsonContent(refRestriction), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific repository reference restriction.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="refRestrictionId">The restriction identifier.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested reference restriction.</returns>
    public async Task<RefRestriction> GetRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId,
        int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions/{refRestrictionId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a repository reference restriction.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="refRestrictionId">The restriction identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the restriction was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, CancellationToken cancellationToken = default)
    {
        var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions/{refRestrictionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}