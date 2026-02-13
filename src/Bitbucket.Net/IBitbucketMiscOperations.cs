using Bitbucket.Net.Builders;
using Bitbucket.Net.Models.Audit;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Models.DefaultReviewers;
using Bitbucket.Net.Models.Jira;
using Bitbucket.Net.Models.PersonalAccessTokens;

namespace Bitbucket.Net;

/// <summary>
/// Miscellaneous operations not covered by domain-specific interfaces.
/// </summary>
public interface IBitbucketMiscOperations
{
    // ── Audit ────────────────────────────────────────────────────────

    Task<IReadOnlyList<AuditEvent>> GetProjectAuditEventsAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditEvent>> GetProjectRepoAuditEventsAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);

    // ── Builders ─────────────────────────────────────────────────────

    PullRequestQueryBuilder PullRequests(string projectKey, string repositorySlug);
    CommitQueryBuilder Commits(string projectKey, string repositorySlug, string until);
    BranchQueryBuilder Branches(string projectKey, string repositorySlug);
    ProjectQueryBuilder Projects();

    // ── Comment Likes ────────────────────────────────────────────────

    Task<IReadOnlyList<User>> GetCommitCommentLikesAsync(string projectKey, string repositorySlug, string commitId, string commentId, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> LikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default);
    Task<bool> UnlikeCommitCommentAsync(string projectKey, string repositorySlug, string commitId, string commentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetPullRequestCommentLikesAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> LikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default);
    Task<bool> UnlikePullRequestCommentAsync(string projectKey, string repositorySlug, string pullRequestId, string commentId, CancellationToken cancellationToken = default);

    // ── Default Reviewers ────────────────────────────────────────────

    Task<IReadOnlyList<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default);
    Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default);
    Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DefaultReviewerPullRequestCondition>> GetDefaultReviewerConditionsAsync(string projectKey, string repositorySlug, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<DefaultReviewerPullRequestCondition> CreateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default);
    Task<DefaultReviewerPullRequestCondition> UpdateDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, DefaultReviewerPullRequestCondition condition, CancellationToken cancellationToken = default);
    Task<bool> DeleteDefaultReviewerConditionAsync(string projectKey, string repositorySlug, string defaultReviewerPullRequestConditionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetDefaultReviewersAsync(string projectKey, string repositorySlug, int? sourceRepoId = null, int? targetRepoId = null, string? sourceRefId = null, string? targetRefId = null, int? avatarSize = null, CancellationToken cancellationToken = default);

    // ── Jira ─────────────────────────────────────────────────────────

    Task<IReadOnlyList<ChangeSet>> GetChangeSetsAsync(string issueKey, int maxChanges = 10, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<JiraIssue> CreateJiraIssueAsync(string pullRequestCommentId, string applicationId, string title, string type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<KeyedUrl>> GetJiraIssuesAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);

    // ── Personal Access Tokens ───────────────────────────────────────

    Task<IReadOnlyList<AccessToken>> GetUserAccessTokensAsync(string userSlug, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<FullAccessToken> CreateAccessTokenAsync(string userSlug, AccessTokenCreate accessToken, CancellationToken cancellationToken = default);
    Task<AccessToken> GetUserAccessTokenAsync(string userSlug, string tokenId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<AccessToken> ChangeUserAccessTokenAsync(string userSlug, string tokenId, AccessTokenCreate accessToken, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAccessTokenAsync(string userSlug, string tokenId, CancellationToken cancellationToken = default);

    // ── Application Properties ───────────────────────────────────────

    Task<IDictionary<string, object?>> GetApplicationPropertiesAsync(CancellationToken cancellationToken = default);

    // ── Dashboard ────────────────────────────────────────────────────

    Task<IReadOnlyList<PullRequest>> GetDashboardPullRequestsAsync(PullRequestStates? state = null, Roles? role = null, List<ParticipantStatus>? status = null, PullRequestOrders? order = PullRequestOrders.Newest, int? closedSinceSeconds = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PullRequest> GetDashboardPullRequestsStreamAsync(PullRequestStates? state = null, Roles? role = null, List<ParticipantStatus>? status = null, PullRequestOrders? order = PullRequestOrders.Newest, int? closedSinceSeconds = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PullRequestSuggestion>> GetDashboardPullRequestSuggestionsAsync(int changesSinceSeconds = 172800, int? maxPages = null, int? limit = 3, int? start = null, CancellationToken cancellationToken = default);

    // ── Groups ───────────────────────────────────────────────────────

    Task<IReadOnlyList<string>> GetGroupNamesAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);

    // ── Hooks ────────────────────────────────────────────────────────

    Task<byte[]> GetProjectHooksAvatarAsync(string hookKey, string? version = null, CancellationToken cancellationToken = default);

    // ── Inbox ────────────────────────────────────────────────────────

    Task<IReadOnlyList<PullRequest>> GetInboxPullRequestsAsync(int? maxPages = null, int? limit = 25, int? start = 0, Roles role = Roles.Reviewer, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PullRequest> GetInboxPullRequestsStreamAsync(int? maxPages = null, int? limit = 25, int? start = 0, Roles role = Roles.Reviewer, CancellationToken cancellationToken = default);
    Task<int> GetInboxPullRequestsCountAsync(CancellationToken cancellationToken = default);

    // ── Logs ─────────────────────────────────────────────────────────

    Task<LogLevels> GetLogLevelAsync(string loggerName, CancellationToken cancellationToken = default);
    Task<bool> SetLogLevelAsync(string loggerName, LogLevels logLevel, CancellationToken cancellationToken = default);
    Task<LogLevels> GetRootLogLevelAsync(CancellationToken cancellationToken = default);
    Task<bool> SetRootLogLevelAsync(LogLevels logLevel, CancellationToken cancellationToken = default);

    // ── Markup ───────────────────────────────────────────────────────

    Task<string> PreviewMarkupAsync(string text, string? urlMode = null, bool? hardWrap = null, bool? htmlEscape = null, CancellationToken cancellationToken = default);

    // ── Profile ──────────────────────────────────────────────────────

    Task<IReadOnlyList<Repository>> GetRecentReposAsync(Permissions? permission = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);

    // ── Tasks (global) ───────────────────────────────────────────────

    Task<BitbucketTask> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<BitbucketTask> GetTaskAsync(long taskId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<BitbucketTask> UpdateTaskAsync(long taskId, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(long taskId, CancellationToken cancellationToken = default);

    // ── Users ────────────────────────────────────────────────────────

    Task<IReadOnlyList<User>> GetUsersAsync(string? filter = null, string? group = null, string? permission = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default, params string[] permissionN);
    Task<User> UpdateUserAsync(string? email = null, string? displayName = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserCredentialsAsync(Models.Core.Users.PasswordChange passwordChange, CancellationToken cancellationToken = default);
    Task<User> GetUserAsync(string userSlug, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAvatarAsync(string userSlug, CancellationToken cancellationToken = default);
    Task<IDictionary<string, object?>> GetUserSettingsAsync(string userSlug, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserSettingsAsync(string userSlug, IDictionary<string, object?> userSettings, CancellationToken cancellationToken = default);

    // ── WhoAmI ───────────────────────────────────────────────────────

    Task<string?> GetWhoAmIAsync(CancellationToken cancellationToken = default);
}