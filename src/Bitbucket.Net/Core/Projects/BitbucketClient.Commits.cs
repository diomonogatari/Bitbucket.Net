using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Runtime.CompilerServices;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Retrieves changes for a repository between two refs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The target ref.</param>
    /// <param name="since">Optional starting ref.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes.</returns>
    public Task<IReadOnlyList<Change>> GetChangesAsync(string projectKey, string repositorySlug, string until, string? since = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["since"] = since,
            ["until"] = until,
        };

        return GetPagedAsync<Change>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/changes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Streams changes for a repository between refs, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The target ref.</param>
    /// <param name="since">Optional starting ref.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of changes.</returns>
    public IAsyncEnumerable<Change> GetChangesStreamAsync(string projectKey, string repositorySlug, string until, string? since = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["since"] = since,
            ["until"] = until,
        };

        return GetPagedStreamAsync<Change>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/changes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves commits for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The ref to retrieve commits until.</param>
    /// <param name="followRenames">Whether to follow renames.</param>
    /// <param name="ignoreMissing">Whether to ignore missing commits.</param>
    /// <param name="merges">Merge commit inclusion policy.</param>
    /// <param name="path">Optional path filter.</param>
    /// <param name="since">Optional starting ref.</param>
    /// <param name="withCounts">Whether to include commit counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of commits.</returns>
    public Task<IReadOnlyList<Commit>> GetCommitsAsync(string projectKey, string repositorySlug,
        string until,
        bool followRenames = false,
        bool ignoreMissing = false,
        MergeCommits merges = MergeCommits.Exclude,
        string? path = null,
        string? since = null,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["followRenames"] = BitbucketHelpers.BoolToString(followRenames),
            ["ignoreMissing"] = BitbucketHelpers.BoolToString(ignoreMissing),
            ["merges"] = BitbucketHelpers.MergeCommitsToString(merges),
            ["path"] = path,
            ["since"] = since,
            ["until"] = until,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return GetPagedAsync<Commit>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/commits"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Streams all commits for a repository as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<Commit> GetCommitsStreamAsync(string projectKey, string repositorySlug,
        string until,
        bool followRenames = false,
        bool ignoreMissing = false,
        MergeCommits merges = MergeCommits.Exclude,
        string? path = null,
        string? since = null,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["followRenames"] = BitbucketHelpers.BoolToString(followRenames),
            ["ignoreMissing"] = BitbucketHelpers.BoolToString(ignoreMissing),
            ["merges"] = BitbucketHelpers.MergeCommitsToString(merges),
            ["path"] = path,
            ["since"] = since,
            ["until"] = until,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return GetPagedStreamAsync<Commit>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/commits"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves a commit by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="path">Optional path filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested commit.</returns>
    public async Task<Commit> GetCommitAsync(string projectKey, string repositorySlug, string commitId, string? path = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["path"] = path,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Commit>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the list of file changes for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="since">Optional starting commit ID.</param>
    /// <param name="withComments">Whether to include comment counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes.</returns>
    public Task<IReadOnlyList<Change>> GetCommitChangesAsync(string projectKey, string repositorySlug, string commitId,
        string? since = null,
        bool withComments = true,
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
            ["since"] = since,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return GetPagedAsync<Change>(
            GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/changes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Streams changes for a specific commit, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="since">Optional starting commit ID.</param>
    /// <param name="withComments">Whether to include comment counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of changes.</returns>
    public IAsyncEnumerable<Change> GetCommitChangesStreamAsync(string projectKey, string repositorySlug, string commitId,
        string? since = null,
        bool withComments = true,
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
            ["since"] = since,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return GetPagedStreamAsync<Change>(
            GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/changes"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves comments for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="path">The file path within the commit.</param>
    /// <param name="since">Optional starting comment ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of comments.</returns>
    public Task<IReadOnlyList<Comment>> GetCommitCommentsAsync(string projectKey, string repositorySlug, string commitId,
        string path,
        string? since = null,
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
            ["path"] = path,
            ["since"] = since,
        };

        return GetPagedAsync<Comment>(
            GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates a comment on a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentInfo">The comment payload.</param>
    /// <param name="since">Optional starting comment ID for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment reference.</returns>
    public async Task<CommentRef> CreateCommitCommentAsync(string projectKey, string repositorySlug, string commitId,
        CommentInfo commentInfo, string? since = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["since"] = since,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, CreateJsonContent(commentInfo), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific commit comment by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested comment reference.</returns>
    public async Task<CommentRef> GetCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the text of a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="commentText">The updated comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated comment reference.</returns>
    public async Task<CommentRef> UpdateCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        CommentText commentText, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(commentText), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">Optional comment version for concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        int version = -1,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .SetQueryParams(queryParamValues)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a diff for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="autoSrcPath">Whether to auto-detect source path.</param>
    /// <param name="contextLines">Context lines to include.</param>
    /// <param name="since">Optional since commit.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="whitespace">Whitespace handling strategy.</param>
    /// <param name="withComments">Whether to include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The diff result.</returns>
    public async Task<Differences> GetCommitDiffAsync(string projectKey, string repositorySlug, string commitId,
        bool autoSrcPath = false,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["autoSrcPath"] = BitbucketHelpers.BoolToString(autoSrcPath),
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["whitespace"] = whitespace,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Differences>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Streams the diff for a specific commit, yielding individual diff entries as they are parsed.
    /// This is more memory-efficient for large diffs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="autoSrcPath">Auto source path.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="since">Since commit.</param>
    /// <param name="srcPath">Source path filter.</param>
    /// <param name="whitespace">Whitespace handling.</param>
    /// <param name="withComments">Include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of diffs.</returns>
    public async IAsyncEnumerable<Diff> GetCommitDiffStreamAsync(string projectKey, string repositorySlug, string commitId,
        bool autoSrcPath = false,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["autoSrcPath"] = BitbucketHelpers.BoolToString(autoSrcPath),
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["whitespace"] = whitespace,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        var responseStream = await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            await foreach (var diff in DeserializeDiffsFromStreamAsync(responseStream, cancellationToken).ConfigureAwait(false))
            {
                yield return diff;
            }
        }
    }

    /// <summary>
    /// Starts watching a commit for notifications.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if watch was created; otherwise, <c>false</c>.</returns>
    public async Task<bool> CreateCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/watch")
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops watching a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commitId);

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/watch")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}