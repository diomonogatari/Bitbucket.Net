using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides Git-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base Git URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the Git root.</returns>
    protected IFlurlRequest GetGitUrl() => GetBaseUrl("/git");

    /// <summary>
    /// Gets the Git URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the Git root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the Git path.</returns>
    protected IFlurlRequest GetGitUrl(string path) => GetGitUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Determines whether a pull request can be rebased.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The rebase eligibility details.</returns>
    public async Task<RebasePullRequestCondition> GetCanRebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RebasePullRequestCondition>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Rebases a pull request to the latest target branch state.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="version">The pull request version for concurrency control.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated pull request.</returns>
    public async Task<PullRequest> RebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var data = new { version };
        var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/rebase")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a tag in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="tagType">The type of tag to create.</param>
    /// <param name="tagName">The name of the tag.</param>
    /// <param name="startPoint">The commit or ref where the tag should point.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created tag.</returns>
    public async Task<Tag> CreateTagAsync(string projectKey, string repositorySlug, TagTypes tagType, string tagName, string startPoint, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);
        ArgumentException.ThrowIfNullOrWhiteSpace(startPoint);

        var data = new
        {
            type = BitbucketHelpers.TagTypeToString(tagType),
            name = tagName,
            startPoint,
        };

        var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/tags")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Tag>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a tag from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="tagName">The name of the tag to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the tag was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(tagName);

        var response = await GetGitUrl($"/projects/{projectKey}/repos/{repositorySlug}/tags/{tagName}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}