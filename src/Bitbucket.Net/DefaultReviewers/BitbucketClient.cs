using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Models.DefaultReviewers;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides default reviewer related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base default reviewers URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the default reviewers root.</returns>
    protected IFlurlRequest GetDefaultReviewersUrl() => GetBaseUrl("/default-reviewers");

    /// <summary>
    /// Gets the default reviewers URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the default reviewers root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the requested default reviewers path.</returns>
    protected IFlurlRequest GetDefaultReviewersUrl(string path) => GetDefaultReviewersUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves default reviewer conditions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of default reviewer conditions.</returns>
    public async Task<IReadOnlyList<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey,
        int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var items = await HandleResponseAsync<IEnumerable<DefaultReviewerPullRequestCondition>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        return items.ToList();
    }

    /// <summary>
    /// Creates a default reviewer condition for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="condition">The condition to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created default reviewer condition.</returns>
    public async Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions")
            .SendAsync(HttpMethod.Post, CreateJsonContent(condition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a default reviewer condition for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="defaultReviewerPullRequestConditionId">The condition identifier.</param>
    /// <param name="condition">The updated condition.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated default reviewer condition.</returns>
    public async Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultReviewerPullRequestConditionId);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions/{defaultReviewerPullRequestConditionId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(condition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a default reviewer condition from a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="defaultReviewerPullRequestConditionId">The condition identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the condition was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultReviewerPullRequestConditionId);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/conditions/{defaultReviewerPullRequestConditionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves default reviewer conditions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of default reviewer conditions.</returns>
    public async Task<IReadOnlyList<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey, string repositorySlug,
        int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var items = await HandleResponseAsync<IEnumerable<DefaultReviewerPullRequestCondition>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        return items.ToList();
    }

    /// <summary>
    /// Creates a default reviewer condition for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="condition">The condition to create.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created default reviewer condition.</returns>
    public async Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions")
            .SendAsync(HttpMethod.Post, CreateJsonContent(condition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a default reviewer condition for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="defaultReviewerPullRequestConditionId">The condition identifier.</param>
    /// <param name="condition">The updated condition.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated default reviewer condition.</returns>
    public async Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultReviewerPullRequestConditionId);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions/{defaultReviewerPullRequestConditionId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(condition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DefaultReviewerPullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a default reviewer condition from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="defaultReviewerPullRequestConditionId">The condition identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the condition was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultReviewerPullRequestConditionId);

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/conditions/{defaultReviewerPullRequestConditionId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves default reviewers for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="sourceRepoId">Optional source repository identifier.</param>
    /// <param name="targetRepoId">Optional target repository identifier.</param>
    /// <param name="sourceRefId">Optional source reference identifier.</param>
    /// <param name="targetRefId">Optional target reference identifier.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of default reviewers.</returns>
    public async Task<IReadOnlyList<User>> GetDefaultReviewersAsync(string projectKey, string repositorySlug,
        int? sourceRepoId = null,
        int? targetRepoId = null,
        string? sourceRefId = null,
        string? targetRefId = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["sourceRepoId"] = sourceRepoId,
            ["targetRepoId"] = targetRepoId,
            ["sourceRefId"] = sourceRefId,
            ["targetRefId"] = targetRefId,
            ["avatarSize"] = avatarSize,
        };

        var response = await GetDefaultReviewersUrl($"/projects/{projectKey}/repos/{repositorySlug}/reviewers")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var items = await HandleResponseAsync<IEnumerable<User>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        return items.ToList();
    }
}