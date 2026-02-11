using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Branches;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Text;
using System.Text.Json;

namespace Bitbucket.Net;

/// <summary>
/// Provides branch-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base URL for branch utilities.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> configured for branch utilities.</returns>
    private IFlurlRequest GetBranchUrl() => GetBaseUrl("/branch-utils");

    /// <summary>
    /// Gets the branch utilities URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the branch utilities root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the requested branch utilities path.</returns>
    private IFlurlRequest GetBranchUrl(string path) => GetBranchUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves branch information for a specific commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="fullSha">The full commit SHA to query.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of branch information entries for the commit.</returns>
    public async Task<IReadOnlyList<BranchBase>> GetCommitBranchInfoAsync(string projectKey, string repositorySlug, string fullSha,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullSha);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches/info/{fullSha}")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<BranchBase>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the branch model configuration for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The branch model configuration.</returns>
    public async Task<BranchModel> GetRepoBranchModelAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branchmodel")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BranchModel>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new branch in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="branchName">The name of the new branch.</param>
    /// <param name="startPoint">The commit or ref from which to branch.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created branch.</returns>
    public async Task<Branch> CreateRepoBranchAsync(string projectKey, string repositorySlug, string branchName, string startPoint, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);
        ArgumentException.ThrowIfNullOrWhiteSpace(startPoint);

        var data = new
        {
            name = branchName,
            startPoint,
        };

        var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Branch>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a branch from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="branchName">The name of the branch to delete.</param>
    /// <param name="dryRun">If true, performs validation without deleting.</param>
    /// <param name="endPoint">Optional endpoint ref to compare for merge checks.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the branch was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteRepoBranchAsync(string projectKey, string repositorySlug, string branchName, bool dryRun, string? endPoint = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(branchName);

        var data = new
        {
            name = branchName,
            dryRun = BitbucketHelpers.BoolToString(dryRun),
            endPoint,
        };

        var json = JsonSerializer.Serialize(data, s_writeJsonOptions);
        var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches")
            .WithHeader("Content-Type", "application/json")
            .SendAsync(HttpMethod.Delete, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}