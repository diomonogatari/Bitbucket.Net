using Bitbucket.Net.Builders;
using Bitbucket.Net.Common.Models.Search;
using Bitbucket.Net.Models.Audit;
using Bitbucket.Net.Models.Branches;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Models.Builds.Requests;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using Bitbucket.Net.Models.DefaultReviewers;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.Jira;
using Bitbucket.Net.Models.PersonalAccessTokens;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;
using Bitbucket.Net.Models.Ssh;

namespace Bitbucket.Net;

/// <summary>
/// Abstraction over the Bitbucket Server REST API client, enabling
/// dependency injection, unit testing with mocks, and decorator patterns.
/// </summary>
public interface IBitbucketClient : IDisposable
{
    // ── Audit ────────────────────────────────────────────────────────

    Task<IReadOnlyList<AuditEvent>> GetProjectAuditEventsAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditEvent>> GetProjectRepoAuditEventsAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);

    // ── Branches ─────────────────────────────────────────────────────

    Task<IReadOnlyList<BranchBase>> GetCommitBranchInfoAsync(string projectKey, string repositorySlug, string fullSha, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<BranchModel> GetRepoBranchModelAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<Branch> CreateRepoBranchAsync(string projectKey, string repositorySlug, string branchName, string startPoint, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepoBranchAsync(string projectKey, string repositorySlug, string branchName, bool dryRun, string? endPoint = null, CancellationToken cancellationToken = default);

    // ── Builders ─────────────────────────────────────────────────────

    PullRequestQueryBuilder PullRequests(string projectKey, string repositorySlug);
    CommitQueryBuilder Commits(string projectKey, string repositorySlug, string until);
    BranchQueryBuilder Branches(string projectKey, string repositorySlug);
    ProjectQueryBuilder Projects();

    // ── Builds ───────────────────────────────────────────────────────

    Task<BuildStats> GetBuildStatsForCommitAsync(string commitId, bool includeUnique = false, CancellationToken cancellationToken = default);
    Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(CancellationToken cancellationToken, params string[] commitIds);
    Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(params string[] commitIds);
    Task<IReadOnlyList<BuildStatus>> GetBuildStatusForCommitAsync(string commitId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> AssociateBuildStatusWithCommitAsync(string commitId, AssociateBuildStatusRequest request, CancellationToken cancellationToken = default);

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

    // ── Git ──────────────────────────────────────────────────────────

    Task<RebasePullRequestCondition> GetCanRebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default);
    Task<PullRequest> RebasePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version, CancellationToken cancellationToken = default);
    Task<Tag> CreateTagAsync(string projectKey, string repositorySlug, TagTypes tagType, string tagName, string startPoint, CancellationToken cancellationToken = default);
    Task<bool> DeleteTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default);

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

    // ── Ref Restrictions ─────────────────────────────────────────────

    Task<IReadOnlyList<RefRestriction>> GetProjectRefRestrictionsAsync(string projectKey, RefRestrictionTypes? type = null, RefMatcherTypes? matcherType = null, string? matcherId = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions);
    Task<IReadOnlyList<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, params RefRestrictionCreate[] refRestrictions);
    Task<RefRestriction> CreateProjectRefRestrictionAsync(string projectKey, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default);
    Task<RefRestriction> GetProjectRefRestrictionAsync(string projectKey, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRefRestrictionAsync(string projectKey, int refRestrictionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> GetRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, RefRestrictionTypes? type = null, RefMatcherTypes? matcherType = null, string? matcherId = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions);
    Task<IReadOnlyList<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, params RefRestrictionCreate[] refRestrictions);
    Task<RefRestriction> CreateRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default);
    Task<RefRestriction> GetRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, CancellationToken cancellationToken = default);

    // ── Ref Sync ─────────────────────────────────────────────────────

    Task<RepositorySynchronizationStatus> GetRepositorySynchronizationStatusAsync(string projectKey, string repositorySlug, string? at = null, CancellationToken cancellationToken = default);
    Task<RepositorySynchronizationStatus> EnableRepositorySynchronizationAsync(string projectKey, string repositorySlug, bool enabled, CancellationToken cancellationToken = default);
    Task<FullRef> SynchronizeRepositoryAsync(string projectKey, string repositorySlug, Synchronize synchronize, CancellationToken cancellationToken = default);

    // ── SSH ──────────────────────────────────────────────────────────

    Task<bool> DeleteProjectsReposKeysAsync(int keyId, CancellationToken cancellationToken, params string[] projectsOrRepos);
    Task<bool> DeleteProjectsReposKeysAsync(int keyId, params string[] projectsOrRepos);
    Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(int keyId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(string projectKey, string? filter = null, Permissions? permission = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<ProjectKey> CreateProjectKeyAsync(string projectKey, string keyText, Permissions permission, CancellationToken cancellationToken = default);
    Task<ProjectKey> GetProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default);
    Task<ProjectKey> UpdateProjectKeyPermissionAsync(string projectKey, int keyId, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(int keyId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(string projectKey, string repositorySlug, string? filter = null, bool? effective = null, Permissions? permission = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<RepositoryKey> CreateRepoKeyAsync(string projectKey, string repositorySlug, string keyText, Permissions permission, CancellationToken cancellationToken = default);
    Task<RepositoryKey> GetRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default);
    Task<RepositoryKey> UpdateRepoKeyPermissionAsync(string projectKey, string repositorySlug, int keyId, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Key>> GetUserKeysAsync(string? userSlug = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Key> CreateUserKeyAsync(string keyText, string? userSlug = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserKeysAsync(string? userSlug = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserKeyAsync(int keyId, CancellationToken cancellationToken = default);
    Task<SshSettings> GetSshSettingsAsync(CancellationToken cancellationToken = default);

    // ── Admin ────────────────────────────────────────────────────────

    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminGroupsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<DeletableGroupOrUser> CreateAdminGroupAsync(string name, CancellationToken cancellationToken = default);
    Task<DeletableGroupOrUser> DeleteAdminGroupAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> AddAdminGroupUsersAsync(GroupUsers groupUsers, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminGroupMoreMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminGroupMoreNonMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminUsersAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> CreateAdminUserAsync(string name, string password, string displayName, string emailAddress, bool addToDefaultGroup = true, string notify = "false", CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateAdminUserAsync(string? name = null, string? displayName = null, string? emailAddress = null, CancellationToken cancellationToken = default);
    Task<UserInfo> DeleteAdminUserAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> AddAdminUserGroupsAsync(UserGroups userGroups, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminUserCaptcha(string name, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminUserCredentialsAsync(Models.Core.Admin.PasswordChange passwordChange, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminUserMoreMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminUserMoreNonMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> RemoveAdminUserFromGroupAsync(string userName, string groupName, CancellationToken cancellationToken = default);
    Task<UserInfo> RenameAdminUserAsync(UserRename userRename, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<Cluster> GetAdminClusterAsync(CancellationToken cancellationToken = default);
    Task<LicenseDetails> GetAdminLicenseAsync(CancellationToken cancellationToken = default);
    Task<LicenseDetails> UpdateAdminLicenseAsync(LicenseInfo licenseInfo, CancellationToken cancellationToken = default);
    Task<MailServerConfiguration> GetAdminMailServerAsync(CancellationToken cancellationToken = default);
    Task<MailServerConfiguration> UpdateAdminMailServerAsync(MailServerConfiguration mailServerConfiguration, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminMailServerAsync(CancellationToken cancellationToken = default);
    Task<string> GetAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default);
    Task<string> UpdateAdminMailServerSenderAddressAsync(string senderAddress, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetAdminGroupPermissionsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminGroupPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminGroupPermissionsAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminGroupPermissionsNoneAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetAdminUserPermissionsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminUserPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminUserPermissionsAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAdminUserPermissionsNoneAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<MergeStrategies> GetAdminPullRequestsMergeStrategiesAsync(string scmId, CancellationToken cancellationToken = default);
    Task<MergeStrategies> UpdateAdminPullRequestsMergeStrategiesAsync(string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default);

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

    // ── Repos ────────────────────────────────────────────────────────

    Task<IReadOnlyList<Repository>> GetRepositoriesAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, string? projectName = null, Permissions? permission = null, bool isPublic = false, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Repository> GetRepositoriesStreamAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, string? projectName = null, Permissions? permission = null, bool isPublic = false, CancellationToken cancellationToken = default);

    // ── Search ───────────────────────────────────────────────────────

    Task<CodeSearchResponse> SearchCodeAsync(string query, int primaryLimit = 25, int secondaryLimit = 10, CancellationToken cancellationToken = default);
    Task<bool> IsSearchAvailableAsync(CancellationToken cancellationToken = default);

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

    // ── Projects ─────────────────────────────────────────────────────

    Task<IReadOnlyList<Project>> GetProjectsAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, Permissions? permission = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Project> GetProjectsStreamAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, Permissions? permission = null, CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<Project> UpdateProjectAsync(string projectKey, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Project> GetProjectAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetProjectUserPermissionsAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectUserPermissionsAsync(string projectKey, string userName, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectUserPermissionsAsync(string projectKey, string userName, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LicensedUser>> GetProjectUserPermissionsNoneAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetProjectGroupPermissionsAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectGroupPermissionsAsync(string projectKey, string groupName, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectGroupPermissionsAsync(string projectKey, string groupName, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LicensedUser>> GetProjectGroupPermissionsNoneAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> IsProjectDefaultPermissionAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);
    Task<bool> GrantProjectPermissionToAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);
    Task<bool> RevokeProjectPermissionFromAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);

    // ── Repositories ─────────────────────────────────────────────────

    Task<IReadOnlyList<Repository>> GetProjectRepositoriesAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Repository> GetProjectRepositoriesStreamAsync(string projectKey, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Repository> CreateProjectRepositoryAsync(string projectKey, CreateRepositoryRequest request, CancellationToken cancellationToken = default);
    Task<Repository> GetProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<RepositoryFork> CreateProjectRepositoryForkAsync(string projectKey, string repositorySlug, ForkRepositoryRequest? request = null, CancellationToken cancellationToken = default);
    Task<bool> ScheduleProjectRepositoryForDeletionAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<Repository> UpdateProjectRepositoryAsync(string projectKey, string repositorySlug, string? targetName = null, bool? isForkable = null, string? targetProjectKey = null, bool? isPublic = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryFork>> GetProjectRepositoryForksAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Repository> RecreateProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryFork>> GetRelatedProjectRepositoriesAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<byte[]> GetProjectRepositoryArchiveAsync(string projectKey, string repositorySlug, string at, string fileName, ArchiveFormats archiveFormat, string path, string prefix, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetProjectRepositoryGroupPermissionsNoneAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, string name, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetProjectRepositoryUserPermissionsNoneAsync(string projectKey, string repositorySlug, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);

    // ── Repository Settings ──────────────────────────────────────────

    Task<Stream> RetrieveRawContentAsync(string projectKey, string repositorySlug, string path, string? at = null, bool markup = false, bool hardWrap = true, bool htmlEscape = true, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> GetProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> UpdateProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug, PullRequestSettings pullRequestSettings, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Hook>> GetProjectRepositoryHooksSettingsAsync(string projectKey, string repositorySlug, HookTypes? hookType = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Hook> GetProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Hook> EnableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, object? hookSettings = null, CancellationToken cancellationToken = default);
    Task<Hook> DisableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object?>> GetProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default);
    Task<Dictionary<string, object?>> UpdateProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey, Dictionary<string, object?> allSettings, CancellationToken cancellationToken = default);
    Task<PullRequestSettings> GetProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, CancellationToken cancellationToken = default);
    Task<MergeStrategies> UpdateProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tag>> GetProjectRepositoryTagsAsync(string projectKey, string repositorySlug, string filterText, BranchOrderBy orderBy, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Tag> GetProjectRepositoryTagsStreamAsync(string projectKey, string repositorySlug, string filterText, BranchOrderBy orderBy, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Tag> CreateProjectRepositoryTagAsync(string projectKey, string repositorySlug, string name, string startPoint, string message, CancellationToken cancellationToken = default);
    Task<Tag> GetProjectRepositoryTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WebHook>> GetProjectRepositoryWebHooksAsync(string projectKey, string repositorySlug, string? @event = null, bool statistics = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<WebHook> CreateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, CreateWebHookRequest request, CancellationToken cancellationToken = default);
    Task<WebHookTestRequestResponse> TestProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string url, CancellationToken cancellationToken = default);
    Task<WebHook> GetProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, bool statistics = false, CancellationToken cancellationToken = default);
    Task<WebHook> UpdateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, UpdateWebHookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string webHookId, CancellationToken cancellationToken = default);
    Task<string> GetProjectRepositoryWebHookLatestAsync(string projectKey, string repositorySlug, string webHookId, string? @event = null, WebHookOutcomes? outcome = null, CancellationToken cancellationToken = default);
    Task<WebHookStatistics> GetProjectRepositoryWebHookStatisticsAsync(string projectKey, string repositorySlug, string webHookId, string? @event = null, CancellationToken cancellationToken = default);
    Task<Dictionary<string, WebHookStatisticsCounts>> GetProjectRepositoryWebHookStatisticsSummaryAsync(string projectKey, string repositorySlug, string webHookId, CancellationToken cancellationToken = default);

    // ── Compare ──────────────────────────────────────────────────────

    Task<IReadOnlyList<Change>> GetRepositoryCompareChangesAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetRepositoryCompareDiffAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, string? srcPath = null, int contextLines = -1, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetRepositoryCompareDiffStreamAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, string? srcPath = null, int contextLines = -1, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commit>> GetRepositoryCompareCommitsAsync(string projectKey, string repositorySlug, string from, string to, string? fromRepo = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetRepositoryDiffAsync(string projectKey, string repositorySlug, string until, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetRepositoryDiffStreamAsync(string projectKey, string repositorySlug, string until, int contextLines = -1, string? since = null, string? srcPath = null, string whitespace = "ignore-all", CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRepositoryFilesAsync(string projectKey, string repositorySlug, string? at = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<LastModified> GetProjectRepositoryLastModifiedAsync(string projectKey, string repositorySlug, string at, CancellationToken cancellationToken = default);

    // ── Commits ──────────────────────────────────────────────────────

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

    // ── Branches (project-level) ─────────────────────────────────────

    Task<IReadOnlyList<Branch>> GetBranchesAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, string? baseBranchOrTag = null, bool? details = null, string? filterText = null, BranchOrderBy? orderBy = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Branch> GetBranchesStreamAsync(string projectKey, string repositorySlug, int? maxPages = null, int? limit = null, int? start = null, string? baseBranchOrTag = null, bool? details = null, string? filterText = null, BranchOrderBy? orderBy = null, CancellationToken cancellationToken = default);
    Task<Branch> CreateBranchAsync(string projectKey, string repositorySlug, CreateBranchRequest request, CancellationToken cancellationToken = default);
    Task<Branch> GetDefaultBranchAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultBranchAsync(string projectKey, string repositorySlug, BranchRef branchRef, CancellationToken cancellationToken = default);
    Task<BrowseItem> BrowseProjectRepositoryAsync(string projectKey, string repositorySlug, string at, bool type = false, bool blame = false, bool noContent = false, CancellationToken cancellationToken = default);
    Task<BrowsePathItem> BrowseProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path, string at, bool type = false, bool blame = false, bool noContent = false, CancellationToken cancellationToken = default);
    Task<Stream> GetRawFileContentStreamAsync(string projectKey, string repositorySlug, string path, string? at = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetRawFileContentLinesStreamAsync(string projectKey, string repositorySlug, string path, string? at = null, CancellationToken cancellationToken = default);
    Task<Commit> UpdateProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path, string fileName, string branch, string? message = null, string? sourceCommitId = null, string? sourceBranch = null, CancellationToken cancellationToken = default);

    // ── Pull Requests ────────────────────────────────────────────────

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

    // ── Pull Request Details ─────────────────────────────────────────

    Task<IReadOnlyList<PullRequestActivity>> GetPullRequestActivitiesAsync(string projectKey, string repositorySlug, long pullRequestId, long? fromId = null, PullRequestFromTypes? fromType = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<PullRequestActivity> GetPullRequestActivitiesStreamAsync(string projectKey, string repositorySlug, long pullRequestId, long? fromId = null, PullRequestFromTypes? fromType = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Change>> GetPullRequestChangesAsync(string projectKey, string repositorySlug, long pullRequestId, ChangeScopes changeScope = ChangeScopes.All, string? sinceId = null, string? untilId = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Change> GetPullRequestChangesStreamAsync(string projectKey, string repositorySlug, long pullRequestId, ChangeScopes changeScope = ChangeScopes.All, string? sinceId = null, string? untilId = null, bool withComments = true, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commit>> GetPullRequestCommitsAsync(string projectKey, string repositorySlug, long pullRequestId, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Commit> GetPullRequestCommitsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, bool withCounts = false, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Differences> GetPullRequestDiffAsync(string projectKey, string repositorySlug, long pullRequestId, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Diff> GetPullRequestDiffStreamAsync(string projectKey, string repositorySlug, long pullRequestId, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);
    Task<Differences> GetPullRequestDiffPathAsync(string projectKey, string repositorySlug, long pullRequestId, string path, int contextLines = -1, DiffTypes diffType = DiffTypes.Effective, string? sinceId = null, string? srcPath = null, string? untilId = null, string whitespace = "ignore-all", bool withComments = true, CancellationToken cancellationToken = default);

    // ── Pull Request Comments ────────────────────────────────────────

    Task<CommentRef> CreatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, string text, string? parentId = null, DiffTypes? diffType = null, string? fromHash = null, string? path = null, string? srcPath = null, string? toHash = null, int? line = null, FileTypes? fileType = null, LineTypes? lineType = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentRef>> GetPullRequestCommentsAsync(string projectKey, string repositorySlug, long pullRequestId, string path, AnchorStates anchorState = AnchorStates.Active, DiffTypes diffType = DiffTypes.Effective, string? fromHash = null, string? toHash = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<CommentRef> GetPullRequestCommentsStreamAsync(string projectKey, string repositorySlug, long pullRequestId, string path, AnchorStates anchorState = AnchorStates.Active, DiffTypes diffType = DiffTypes.Effective, string? fromHash = null, string? toHash = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<CommentRef> GetPullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<CommentRef> UpdatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int version, string text, CancellationToken cancellationToken = default);
    Task<bool> DeletePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId, int version = -1, CancellationToken cancellationToken = default);

    // ── Pull Request Tasks ───────────────────────────────────────────

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