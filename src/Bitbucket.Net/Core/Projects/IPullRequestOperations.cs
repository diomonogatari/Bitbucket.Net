using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net;

/// <summary>
/// Pull request operations.
/// </summary>
public interface IPullRequestOperations
{
    Task<IReadOnlyList<Identity>> GetRepositoryParticipantsAsync(string projectKey, string repositorySlug, PullRequestDirections direction = PullRequestDirections.Incoming, string? filter = null, Roles? role = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PullRequest>> GetPullRequestsAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, PullRequestDirections direction = PullRequestDirections.Incoming, string? branchId = null, PullRequestStates state = PullRequestStates.Open, PullRequestOrders order = PullRequestOrders.Newest, bool withAttributes = true, bool withProperties = true, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PullRequest> GetPullRequestsStreamAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, PullRequestDirections direction = PullRequestDirections.Incoming, string? branchId = null, PullRequestStates state = PullRequestStates.Open, PullRequestOrders order = PullRequestOrders.Newest, bool withAttributes = true, bool withProperties = true, CancellationToken cancellationToken = default);
    Task<PullRequest> CreatePullRequestAsync(string projectKey, string repositorySlug, CreatePullRequestRequest request, CancellationToken cancellationToken = default);
    Task<PullRequest> GetPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<PullRequest> UpdatePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, UpdatePullRequestRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeletePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, VersionInfo versionInfo, CancellationToken cancellationToken = default);
    Task<bool> DeclinePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default);
    Task<PullRequestMergeState> GetPullRequestMergeStateAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default);
    Task<Commit?> GetPullRequestMergeBaseAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<PullRequest> MergePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, MergePullRequestRequest? request = null, CancellationToken cancellationToken = default);
    Task<PullRequest> ReopenPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default);
    Task<Reviewer> ApprovePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<Reviewer> DeletePullRequestApprovalAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Participant>> GetPullRequestParticipantsAsync(string projectKey, string repositorySlug, long pullRequestId, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Participant> GetPullRequestParticipantsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<Participant> AssignUserRoleToPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, Named named, Roles role, CancellationToken cancellationToken = default);
    Task<bool> DeletePullRequestParticipantAsync(string projectKey, string repositorySlug, long pullRequestId, string userName, CancellationToken cancellationToken = default);
    Task<Participant> UpdatePullRequestParticipantStatus(string projectKey, string repositorySlug, long pullRequestId, string userSlug, Named named, bool approved, ParticipantStatus participantStatus, CancellationToken cancellationToken = default);
    Task<bool> UnassignUserFromPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, string userSlug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PullRequestActivity>> GetPullRequestActivitiesAsync(string projectKey, string repositorySlug, long pullRequestId, long? fromId = null, PullRequestFromTypes? fromType = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PullRequestActivity> GetPullRequestActivitiesStreamAsync(string projectKey, string repositorySlug, long pullRequestId, long? fromId = null, PullRequestFromTypes? fromType = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Change>> GetPullRequestChangesAsync(string projectKey, string repositorySlug, long pullRequestId, ChangeScopes changeScope = ChangeScopes.All, string? sinceId = null, string? untilId = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Change> GetPullRequestChangesStreamAsync(string projectKey, string repositorySlug, long pullRequestId, ChangeScopes changeScope = ChangeScopes.All, string? sinceId = null, string? untilId = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commit>> GetPullRequestCommitsAsync(string projectKey, string repositorySlug, long pullRequestId, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Commit> GetPullRequestCommitsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetPullRequestDiffAsync(string projectKey, string repositorySlug, long pullRequestId, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetPullRequestDiffStreamAsync(string projectKey, string repositorySlug, long pullRequestId, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    Task<Differences> GetPullRequestDiffPathAsync(string projectKey, string repositorySlug, long pullRequestId, string path, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    Task<CommentRef> CreatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, string text, string? parentId = null, DiffTypes? diffType = null, string? fromHash = null, string? path = null, string? srcPath = null, string? toHash = null, int? line = null, FileTypes? fileType = null, LineTypes? lineType = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentRef>> GetPullRequestCommentsAsync(string projectKey, string repositorySlug, long pullRequestId, string path, AnchorStates anchorState = AnchorStates.Active, DiffTypes diffType = DiffTypes.Effective, string? fromHash = null, string? toHash = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<CommentRef> GetPullRequestCommentsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, string path, AnchorStates anchorState = AnchorStates.Active, DiffTypes diffType = DiffTypes.Effective, string? fromHash = null, string? toHash = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<CommentRef> GetPullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<CommentRef> UpdatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int version, string text, CancellationToken cancellationToken = default);
    Task<bool> DeletePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int version = -1, CancellationToken cancellationToken = default);

    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync for 9.0+ or GetPullRequestTasksWithFallbackAsync for cross-version compatibility.")]
    Task<IReadOnlyList<BitbucketTask>> GetPullRequestTasksAsync(string projectKey, string repositorySlug, long pullRequestId, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);

    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsStreamAsync for 9.0+ compatibility.")]
    IAsyncEnumerable<BitbucketTask> GetPullRequestTasksStreamAsync(string projectKey, string repositorySlug, long pullRequestId, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);

    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync and count the results for 9.0+ compatibility.")]
    Task<BitbucketTaskCount> GetPullRequestTaskCountAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlockerComment>> GetPullRequestBlockerCommentsAsync(string projectKey, string repositorySlug, long pullRequestId, BlockerCommentState? state = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<BlockerComment> GetPullRequestBlockerCommentsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, BlockerCommentState? state = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<BlockerComment> GetPullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId, CancellationToken cancellationToken = default);
    Task<BlockerComment> CreatePullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, string text, CommentAnchor? anchor = null, CancellationToken cancellationToken = default);
    Task<BlockerComment> UpdatePullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId, string text, int version, CancellationToken cancellationToken = default);
    Task<bool> DeletePullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId, int version, CancellationToken cancellationToken = default);
    Task<BlockerComment> ResolvePullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId, int version, CancellationToken cancellationToken = default);
    Task<BlockerComment> ReopenPullRequestBlockerCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long blockerCommentId, int version, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<object>> GetPullRequestTasksWithFallbackAsync(string projectKey, string repositorySlug, long pullRequestId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> WatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<bool> UnwatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
}