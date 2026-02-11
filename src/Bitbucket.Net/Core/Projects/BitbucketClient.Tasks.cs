using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Tasks;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Gets tasks for a pull request using the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="avatarSize">Avatar size for user avatars.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of tasks.</returns>
    /// <remarks>
    /// <para>
    /// <b>Deprecation Notice:</b> This endpoint was deprecated in Bitbucket Server 9.0 and returns 404 Not Found on servers version 9.0+.
    /// </para>
    /// <para>
    /// For Bitbucket Server 9.0+, use <see cref="GetPullRequestBlockerCommentsAsync"/> instead.
    /// For cross-version compatibility, use <see cref="GetPullRequestTasksWithFallbackAsync"/>.
    /// </para>
    /// </remarks>
    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync for 9.0+ or GetPullRequestTasksWithFallbackAsync for cross-version compatibility.")]
    public async Task<IReadOnlyList<BitbucketTask>> GetPullRequestTasksAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/tasks")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<BitbucketTask>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams tasks for a pull request, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of tasks.</returns>
    /// <remarks>
    /// <para>
    /// <b>Deprecation Notice:</b> This endpoint was deprecated in Bitbucket Server 9.0 and returns 404 Not Found on servers version 9.0+.
    /// </para>
    /// <para>
    /// For Bitbucket Server 9.0+, use <see cref="GetPullRequestBlockerCommentsStreamAsync"/> instead.
    /// </para>
    /// </remarks>
    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsStreamAsync for 9.0+ compatibility.")]
    public IAsyncEnumerable<BitbucketTask> GetPullRequestTasksStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/tasks")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<BitbucketTask>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Gets the task count for a pull request using the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task count.</returns>
    /// <remarks>
    /// <para>
    /// <b>Deprecation Notice:</b> This endpoint was deprecated in Bitbucket Server 9.0 and may return 404 Not Found on servers version 9.0+.
    /// </para>
    /// <para>
    /// For Bitbucket Server 9.0+, use <see cref="GetPullRequestBlockerCommentsAsync"/> and count the results.
    /// </para>
    /// </remarks>
    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync and count the results for 9.0+ compatibility.")]
    public async Task<BitbucketTaskCount> GetPullRequestTaskCountAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/tasks/count")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BitbucketTaskCount>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #region Blocker Comments (Bitbucket Server 9.0+)

    /// <summary>
    /// Gets blocker comments (tasks) for a pull request.
    /// This endpoint is available in Bitbucket Server 9.0+ and replaces the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="state">Optional filter: <see cref="BlockerCommentState.Open"/>, <see cref="BlockerCommentState.Resolved"/>, or null for all.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of blocker comments.</returns>
    /// <remarks>
    /// <para>
    /// In Bitbucket Server 9.0+, tasks have been replaced by blocker comments.
    /// A blocker comment is a comment with <c>severity: 'BLOCKER'</c> that must be resolved before the pull request can be merged.
    /// </para>
    /// <para>
    /// For servers prior to 9.0, use <see cref="GetPullRequestTasksAsync"/> instead.
    /// </para>
    /// </remarks>
    public async Task<IReadOnlyList<BlockerComment>> GetPullRequestBlockerCommentsAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        BlockerCommentState? state = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["state"] = BitbucketHelpers.BlockerCommentStateToString(state),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/blocker-comments")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<BlockerComment>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams blocker comments for a pull request, yielding items as they are retrieved.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="state">Optional blocker comment state filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of blocker comments.</returns>
    public IAsyncEnumerable<BlockerComment> GetPullRequestBlockerCommentsStreamAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        BlockerCommentState? state = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["state"] = BitbucketHelpers.BlockerCommentStateToString(state),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/blocker-comments")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<BlockerComment>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Gets a single blocker comment by ID.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blocker comment.</returns>
    public async Task<BlockerComment> GetPullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a blocker comment (task) on a pull request.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="text">The blocker comment text.</param>
    /// <param name="anchor">Optional anchor for file/line-specific blockers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created blocker comment.</returns>
    public async Task<BlockerComment> CreatePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        string text,
        CommentAnchor? anchor = null,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            text,
            severity = "BLOCKER",
            anchor,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a blocker comment's text.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="text">The updated blocker comment text.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated blocker comment.</returns>
    public async Task<BlockerComment> UpdatePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        string text,
        int version,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            text,
            version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a blocker comment.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the blocker comment was deleted successfully.</returns>
    public async Task<bool> DeletePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .SetQueryParam("version", version)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resolves a blocker comment (marks the task as complete).
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The resolved blocker comment.</returns>
    public async Task<BlockerComment> ResolvePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}/resolve")
            .SetQueryParam("version", version)
            .PutAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reopens a resolved blocker comment.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reopened blocker comment.</returns>
    public async Task<BlockerComment> ReopenPullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}/reopen")
            .SetQueryParam("version", version)
            .PutAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets pull request tasks with automatic fallback for cross-version compatibility.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides backward compatibility across Bitbucket Server versions:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>Bitbucket Server 9.0+:</b> Uses the new <c>/blocker-comments</c> endpoint.</description></item>
    /// <item><description><b>Bitbucket Server &lt; 9.0:</b> Falls back to the legacy <c>/tasks</c> endpoint.</description></item>
    /// </list>
    /// <para>
    /// The method first tries the new blocker-comments endpoint. If it returns 404 (Not Found),
    /// it automatically falls back to the legacy tasks endpoint.
    /// </para>
    /// <para>
    /// For new code targeting Bitbucket Server 9.0+, prefer using 
    /// <see cref="GetPullRequestBlockerCommentsAsync"/> directly for better type safety.
    /// </para>
    /// </remarks>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A collection of blocker comments (<see cref="BlockerComment"/>) on Bitbucket 9.0+,
    /// or legacy tasks (<see cref="BitbucketTask"/>) on older versions.
    /// </returns>
    public async Task<IReadOnlyList<object>> GetPullRequestTasksWithFallbackAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try new blocker-comments endpoint first (Bitbucket 9.0+)
            var blockerComments = await GetPullRequestBlockerCommentsAsync(
                projectKey, repositorySlug, pullRequestId,
                maxPages: maxPages, limit: limit, start: start,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return blockerComments.Cast<object>().ToList();
        }
        catch (BitbucketNotFoundException)
        {
            // Fall back to legacy tasks endpoint (Bitbucket < 9.0)
#pragma warning disable CS0618 // Type or member is obsolete - intentional fallback
            var tasks = await GetPullRequestTasksAsync(
                projectKey, repositorySlug, pullRequestId,
                maxPages: maxPages, limit: limit, start: start,
                cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CS0618

            return tasks.Cast<object>().ToList();
        }
    }

    #endregion

    /// <summary>
    /// Subscribes the current user to watch a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was added; otherwise, <c>false</c>.</returns>
    public async Task<bool> WatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/watch")
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Unsubscribes the current user from watching a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnwatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/watch")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

}