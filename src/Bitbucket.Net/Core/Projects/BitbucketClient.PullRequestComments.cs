using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{

    /// <summary>
    /// Creates a comment on a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="text">The comment text.</param>
    /// <param name="parentId">Optional parent comment ID to create a reply.</param>
    /// <param name="diffType">Optional diff type.</param>
    /// <param name="fromHash">Optional from commit hash for anchoring.</param>
    /// <param name="path">Optional file path for anchoring.</param>
    /// <param name="srcPath">Optional source path for move/rename anchors.</param>
    /// <param name="toHash">Optional to commit hash for anchoring.</param>
    /// <param name="line">Optional line number for anchoring.</param>
    /// <param name="fileType">Optional file type for anchoring.</param>
    /// <param name="lineType">Optional line type for anchoring.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment reference.</returns>
    public async Task<CommentRef> CreatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId,
        string text,
        string? parentId = null,
        DiffTypes? diffType = null,
        string? fromHash = null,
        string? path = null,
        string? srcPath = null,
        string? toHash = null,
        int? line = null,
        FileTypes? fileType = null,
        LineTypes? lineType = null,
        CancellationToken cancellationToken = default)
    {
        // Build the comment payload dynamically to avoid sending empty anchor objects
        // which Bitbucket Server 9.0 rejects with HTTP 500.
        // See: BUG-003 - add_pull_request_comment returns 500 error
        var data = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["text"] = text,
        };

        if (!string.IsNullOrEmpty(parentId))
        {
            data["parent"] = new { id = parentId };
        }

        // Only include anchor if at least one anchor-related field is specified
        // Empty anchor objects cause HTTP 500 on Bitbucket Server 9.0
        var hasAnchorData = diffType.HasValue
            || !string.IsNullOrEmpty(fromHash)
            || !string.IsNullOrEmpty(path)
            || !string.IsNullOrEmpty(srcPath)
            || !string.IsNullOrEmpty(toHash)
            || line.HasValue
            || fileType.HasValue
            || lineType.HasValue;

        if (hasAnchorData)
        {
            data["anchor"] = new
            {
                diffType = BitbucketHelpers.DiffTypeToString(diffType),
                fromHash,
                path,
                srcPath,
                toHash,
                line,
                fileType = BitbucketHelpers.FileTypeToString(fileType),
                lineType = BitbucketHelpers.LineTypeToString(lineType),
            };
        }

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves comments for a pull request path.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="path">The file path to filter comments.</param>
    /// <param name="anchorState">Anchor state filter.</param>
    /// <param name="diffType">Diff type filter.</param>
    /// <param name="fromHash">Optional from commit hash.</param>
    /// <param name="toHash">Optional to commit hash.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of pull request comments.</returns>
    public async Task<IEnumerable<CommentRef>> GetPullRequestCommentsAsync(string projectKey, string repositorySlug, long pullRequestId,
        string path,
        AnchorStates anchorState = AnchorStates.Active,
        DiffTypes diffType = DiffTypes.Effective,
        string? fromHash = null,
        string? toHash = null,
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
            ["path"] = path,
            ["anchorState"] = BitbucketHelpers.AnchorStateToString(anchorState),
            ["diffType"] = BitbucketHelpers.DiffTypeToString(diffType),
            ["fromHash"] = fromHash,
            ["toHash"] = toHash,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/comments")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<CommentRef>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams comments for a pull request, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="path">The file path to filter comments for.</param>
    /// <param name="anchorState">The anchor state filter.</param>
    /// <param name="diffType">The diff type filter.</param>
    /// <param name="fromHash">Optional from commit hash.</param>
    /// <param name="toHash">Optional to commit hash.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of comment references.</returns>
    public IAsyncEnumerable<CommentRef> GetPullRequestCommentsStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        string path,
        AnchorStates anchorState = AnchorStates.Active,
        DiffTypes diffType = DiffTypes.Effective,
        string? fromHash = null,
        string? toHash = null,
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
            ["path"] = path,
            ["anchorState"] = BitbucketHelpers.AnchorStateToString(anchorState),
            ["diffType"] = BitbucketHelpers.DiffTypeToString(diffType),
            ["fromHash"] = fromHash,
            ["toHash"] = toHash,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/comments")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<CommentRef>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves a single pull request comment by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested comment reference.</returns>
    public async Task<CommentRef> GetPullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">The comment version for optimistic concurrency.</param>
    /// <param name="text">The updated comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated comment reference.</returns>
    public async Task<CommentRef> UpdatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int version, string text, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            version,
            text,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("version", version)
            .SendAsync(HttpMethod.Put, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the comment was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int version = -1,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("version", version)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

}