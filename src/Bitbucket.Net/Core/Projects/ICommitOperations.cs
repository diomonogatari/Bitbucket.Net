using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net;

/// <summary>
/// Commit and compare operations.
/// </summary>
public interface ICommitOperations
{
    Task<IReadOnlyList<Change>> GetChangesAsync(string projectKey, string repositorySlug, string until, string? since = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Change> GetChangesStreamAsync(string projectKey, string repositorySlug, string until, string? since = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commit>> GetCommitsAsync(string projectKey, string repositorySlug, string until, bool followRenames = false, bool ignoreMissing = false, MergeCommits merges = MergeCommits.Exclude, string? path = null, string? since = null, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Commit> GetCommitsStreamAsync(string projectKey, string repositorySlug, string until, bool followRenames = false, bool ignoreMissing = false, MergeCommits merges = MergeCommits.Exclude, string? path = null, string? since = null, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Commit> GetCommitAsync(string projectKey, string repositorySlug, string commitId, string? path = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Change>> GetCommitChangesAsync(string projectKey, string repositorySlug, string commitId, string? since = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Change> GetCommitChangesStreamAsync(string projectKey, string repositorySlug, string commitId, string? since = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Comment>> GetCommitCommentsAsync(string projectKey, string repositorySlug, string commitId, string path, string? since = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<CommentRef> CreateCommitCommentAsync(string projectKey, string repositorySlug, string commitId, CommentInfo commentInfo, string? since = null, CancellationToken cancellationToken = default);
    Task<CommentRef> GetCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<CommentRef> UpdateCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId, CommentText commentText, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId, int version = -1, CancellationToken cancellationToken = default);
    Task<Differences> GetCommitDiffAsync(string projectKey, string repositorySlug, string commitId, bool autoSrcPath = false, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetCommitDiffStreamAsync(string projectKey, string repositorySlug, string commitId, bool autoSrcPath = false, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    Task<bool> CreateCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default);
    Task<bool> DeleteCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Change>> GetRepositoryCompareChangesAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetRepositoryCompareDiffAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, string? srcPath = null, int contextLines = -1, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetRepositoryCompareDiffStreamAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, string? srcPath = null, int contextLines = -1, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commit>> GetRepositoryCompareCommitsAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetRepositoryDiffAsync(string projectKey, string repositorySlug, string until, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetRepositoryDiffStreamAsync(string projectKey, string repositorySlug, string until, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRepositoryFilesAsync(string projectKey, string repositorySlug, string? at = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<LastModified> GetProjectRepositoryLastModifiedAsync(string projectKey, string repositorySlug, string at, CancellationToken cancellationToken = default);
}