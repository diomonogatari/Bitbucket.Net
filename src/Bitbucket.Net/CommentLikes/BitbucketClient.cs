using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides comment-like related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base URL for comment likes.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the comment likes root.</returns>
    protected IFlurlRequest GetCommentLikesUrl() => GetBaseUrl("/comment-likes");

    /// <summary>
    /// Gets the comment likes URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the comment likes root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the comment likes path.</returns>
    protected IFlurlRequest GetCommentLikesUrl(string path) => GetCommentLikesUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves users who liked a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of users who liked the comment.</returns>
    public Task<IReadOnlyList<User>> GetCommitCommentLikesAsync(string projectKey, string repositorySlug, string commitId, string commentId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return GetPagedAsync<User>(
            GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Adds a like to a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the like was added; otherwise, <c>false</c>.</returns>
    public async Task<bool> LikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a like from a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the like was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnlikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/commits/{commitId}/comments/{commentId}/likes")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves users who liked a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of users who liked the pull request comment.</returns>
    public Task<IReadOnlyList<User>> GetPullRequestCommentLikesAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(pullRequestId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedAsync<User>(
            GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Adds a like to a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the like was added; otherwise, <c>false</c>.</returns>
    public async Task<bool> LikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(pullRequestId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a like from a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="commentId">The comment identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the like was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnlikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(pullRequestId);
        ArgumentException.ThrowIfNullOrWhiteSpace(commentId);

        var response = await GetCommentLikesUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/comments/{commentId}/likes")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}