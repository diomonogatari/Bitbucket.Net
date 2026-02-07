using Bitbucket.Net.Common.Models;
// Audit
using Bitbucket.Net.Models.Audit;
// Branches
using Bitbucket.Net.Models.Branches;
// Builds
using Bitbucket.Net.Models.Builds;
// Core - Admin
using Bitbucket.Net.Models.Core.Admin;
// Core - Logs
using Bitbucket.Net.Models.Core.Logs;
// Core - Projects
using Bitbucket.Net.Models.Core.Projects;
// Core - Tasks
using Bitbucket.Net.Models.Core.Tasks;
// Core - Users
using Bitbucket.Net.Models.Core.Users;
// DefaultReviewers
using Bitbucket.Net.Models.DefaultReviewers;
// Git
using Bitbucket.Net.Models.Git;
// Jira
using Bitbucket.Net.Models.Jira;
// PersonalAccessTokens
using Bitbucket.Net.Models.PersonalAccessTokens;
// RefRestrictions
using Bitbucket.Net.Models.RefRestrictions;
// RefSync
using Bitbucket.Net.Models.RefSync;
// Ssh
using Bitbucket.Net.Models.Ssh;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Serialization;

/// <summary>
/// Source-generated JSON serialization context for all Bitbucket model types.
/// Provides up to 3x faster serialization/deserialization and enables AOT/trimming support.
/// </summary>
/// <remarks>
/// <para>
/// This context is combined with <see cref="System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver"/>
/// to provide a fallback for any types not explicitly registered (edge cases, future additions).
/// </para>
/// <para>
/// Custom converters (UnixDateTimeOffsetConverter, etc.) continue to work with source generation.
/// </para>
/// </remarks>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization)]

// ============================================================================
// Common Models
// ============================================================================
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(PagedResultsBase))]

// ============================================================================
// PagedResults<T> Generic Instantiations
// These must be explicitly registered for source generation
// ============================================================================
[JsonSerializable(typeof(PagedResults<AccessToken>))]
[JsonSerializable(typeof(PagedResults<AuditEvent>))]
[JsonSerializable(typeof(PagedResults<BitbucketTask>))]
[JsonSerializable(typeof(PagedResults<BlockerComment>))]
[JsonSerializable(typeof(PagedResults<Branch>))]
[JsonSerializable(typeof(PagedResults<BranchBase>))]
[JsonSerializable(typeof(PagedResults<BuildStatus>))]
[JsonSerializable(typeof(PagedResults<Change>))]
[JsonSerializable(typeof(PagedResults<ChangeSet>))]
[JsonSerializable(typeof(PagedResults<Comment>))]
[JsonSerializable(typeof(PagedResults<CommentRef>))]
[JsonSerializable(typeof(PagedResults<Commit>))]
[JsonSerializable(typeof(PagedResults<ContentItem>))]
[JsonSerializable(typeof(PagedResults<DeletableGroupOrUser>))]
[JsonSerializable(typeof(PagedResults<GroupPermission>))]
[JsonSerializable(typeof(PagedResults<Hook>))]
[JsonSerializable(typeof(PagedResults<Identity>))]
[JsonSerializable(typeof(PagedResults<Key>))]
[JsonSerializable(typeof(PagedResults<LicensedUser>))]
[JsonSerializable(typeof(PagedResults<Participant>))]
[JsonSerializable(typeof(PagedResults<Project>))]
[JsonSerializable(typeof(PagedResults<ProjectKey>))]
[JsonSerializable(typeof(PagedResults<PullRequest>))]
[JsonSerializable(typeof(PagedResults<PullRequestActivity>))]
[JsonSerializable(typeof(PagedResults<PullRequestSuggestion>))]
[JsonSerializable(typeof(PagedResults<RefRestriction>))]
[JsonSerializable(typeof(PagedResults<Repository>))]
[JsonSerializable(typeof(PagedResults<RepositoryFork>))]
[JsonSerializable(typeof(PagedResults<RepositoryKey>))]
[JsonSerializable(typeof(PagedResults<string>))]
[JsonSerializable(typeof(PagedResults<Tag>))]
[JsonSerializable(typeof(PagedResults<User>))]
[JsonSerializable(typeof(PagedResults<UserInfo>))]
[JsonSerializable(typeof(PagedResults<UserPermission>))]
[JsonSerializable(typeof(PagedResults<WebHook>))]

// ============================================================================
// Audit Models
// ============================================================================
[JsonSerializable(typeof(AuditEvent))]

// ============================================================================
// Branches Models
// ============================================================================
[JsonSerializable(typeof(BranchModel))]

// ============================================================================
// Builds Models
// ============================================================================
[JsonSerializable(typeof(BuildStats))]
[JsonSerializable(typeof(BuildStatus))]
[JsonSerializable(typeof(KeyedUrl))]

// ============================================================================
// Core - Admin Models
// ============================================================================
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(Cluster))]
[JsonSerializable(typeof(DeletableGroupOrUser))]
[JsonSerializable(typeof(GroupPermission))]
[JsonSerializable(typeof(GroupUsers))]
[JsonSerializable(typeof(LicenseDetails))]
[JsonSerializable(typeof(LicenseInfo))]
[JsonSerializable(typeof(LicenseStatus))]
[JsonSerializable(typeof(MailServerConfiguration))]
[JsonSerializable(typeof(MergeStrategies))]
[JsonSerializable(typeof(MergeStrategy))]
[JsonSerializable(typeof(Node))]
[JsonSerializable(typeof(PasswordBasic))]
[JsonSerializable(typeof(Bitbucket.Net.Models.Core.Admin.PasswordChange))]
[JsonSerializable(typeof(UserGroups))]
[JsonSerializable(typeof(UserInfo))]
[JsonSerializable(typeof(UserPermission))]
[JsonSerializable(typeof(UserRename))]

// ============================================================================
// Core - Projects Models
// ============================================================================
[JsonSerializable(typeof(AheadBehindMetaData))]
[JsonSerializable(typeof(Author))]
[JsonSerializable(typeof(BlockerComment))]
[JsonSerializable(typeof(Branch))]
[JsonSerializable(typeof(BranchBase))]
[JsonSerializable(typeof(BranchInfo))]
[JsonSerializable(typeof(BranchMetaData))]
[JsonSerializable(typeof(BranchRef))]
[JsonSerializable(typeof(BrowseItem))]
[JsonSerializable(typeof(BrowsePathItem))]
[JsonSerializable(typeof(BuildStatusMetadata))]
[JsonSerializable(typeof(Change))]
[JsonSerializable(typeof(CloneLink))]
[JsonSerializable(typeof(CloneLinks))]
[JsonSerializable(typeof(Comment))]
[JsonSerializable(typeof(CommentAnchor))]
[JsonSerializable(typeof(CommentId))]
[JsonSerializable(typeof(CommentInfo))]
[JsonSerializable(typeof(CommentRef))]
[JsonSerializable(typeof(CommentText))]
[JsonSerializable(typeof(Commit))]
[JsonSerializable(typeof(CommitParent))]
[JsonSerializable(typeof(ContentItem))]
[JsonSerializable(typeof(Diff))]
[JsonSerializable(typeof(DiffHunk))]
[JsonSerializable(typeof(DiffInfo))]
[JsonSerializable(typeof(Differences))]
[JsonSerializable(typeof(FromToRef))]
[JsonSerializable(typeof(Hook))]
[JsonSerializable(typeof(HookDetails))]
[JsonSerializable(typeof(HookScope))]
[JsonSerializable(typeof(LastModified))]
[JsonSerializable(typeof(LicensedUser))]
[JsonSerializable(typeof(Line))]
[JsonSerializable(typeof(LineRef))]
[JsonSerializable(typeof(Link))]
[JsonSerializable(typeof(Links))]
[JsonSerializable(typeof(MergeCheckRequiredBuilds))]
[JsonSerializable(typeof(MergeCommits))]
[JsonSerializable(typeof(MergeHookRequiredApprovers))]
[JsonSerializable(typeof(Participant))]
[JsonSerializable(typeof(Path))]
[JsonSerializable(typeof(Permittedoperations))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectDefinition))]
[JsonSerializable(typeof(ProjectRef))]
[JsonSerializable(typeof(Properties))]
[JsonSerializable(typeof(PullRequest))]
[JsonSerializable(typeof(PullRequestActivity))]
[JsonSerializable(typeof(PullRequestInfo))]
[JsonSerializable(typeof(PullRequestMergeState))]
[JsonSerializable(typeof(PullRequestMetadata))]
[JsonSerializable(typeof(PullRequestSettings))]
[JsonSerializable(typeof(PullRequestSuggestion))]
[JsonSerializable(typeof(PullRequestUpdate))]
[JsonSerializable(typeof(Ref))]
[JsonSerializable(typeof(RefChange))]
[JsonSerializable(typeof(Repository))]
[JsonSerializable(typeof(RepositoryFork))]
[JsonSerializable(typeof(RepositoryOrigin))]
[JsonSerializable(typeof(RepositoryRef))]
[JsonSerializable(typeof(Reviewer))]
[JsonSerializable(typeof(Segment))]
[JsonSerializable(typeof(Tag))]
[JsonSerializable(typeof(TimeWindow))]
[JsonSerializable(typeof(VersionInfo))]
[JsonSerializable(typeof(Bitbucket.Net.Models.Core.Projects.Veto))]
[JsonSerializable(typeof(WebHook))]
[JsonSerializable(typeof(WebHookInvocation))]
[JsonSerializable(typeof(WebHookRequest))]
[JsonSerializable(typeof(WebHookResult))]
[JsonSerializable(typeof(WebHookStatistics))]
[JsonSerializable(typeof(WebHookStatisticsCounts))]
[JsonSerializable(typeof(WebHookStatisticsSummary))]
[JsonSerializable(typeof(WebHookTestRequest))]
[JsonSerializable(typeof(WebHookTestRequestResponse))]
[JsonSerializable(typeof(WebHookTestResponse))]
[JsonSerializable(typeof(WithId))]

// ============================================================================
// Core - Tasks Models
// ============================================================================
[JsonSerializable(typeof(BitbucketTask))]
[JsonSerializable(typeof(BitbucketTaskCount))]
[JsonSerializable(typeof(TaskAnchor))]
[JsonSerializable(typeof(TaskBasicAnchor))]
[JsonSerializable(typeof(TaskInfo))]
[JsonSerializable(typeof(TaskRef))]

// ============================================================================
// Core - Users Models
// ============================================================================
[JsonSerializable(typeof(Identity))]
[JsonSerializable(typeof(Named))]
[JsonSerializable(typeof(Bitbucket.Net.Models.Core.Users.PasswordChange))]
[JsonSerializable(typeof(User))]

// ============================================================================
// DefaultReviewers Models
// ============================================================================
[JsonSerializable(typeof(DefaultReviewerPullRequestCondition))]
[JsonSerializable(typeof(DefaultReviewerPullRequestConditionScope))]
[JsonSerializable(typeof(RefMatcher))]

// ============================================================================
// Git Models
// ============================================================================
[JsonSerializable(typeof(RebasePullRequestCondition))]
[JsonSerializable(typeof(Bitbucket.Net.Models.Git.Veto))]

// ============================================================================
// Jira Models
// ============================================================================
[JsonSerializable(typeof(ChangeSet))]
[JsonSerializable(typeof(Changes))]
[JsonSerializable(typeof(JiraIssue))]

// ============================================================================
// PersonalAccessTokens Models
// ============================================================================
[JsonSerializable(typeof(AccessToken))]
[JsonSerializable(typeof(AccessTokenCreate))]
[JsonSerializable(typeof(FullAccessToken))]

// ============================================================================
// RefRestrictions Models
// ============================================================================
[JsonSerializable(typeof(AccessKey))]
[JsonSerializable(typeof(Key))]
[JsonSerializable(typeof(RefRestriction))]
[JsonSerializable(typeof(RefRestrictionBase))]
[JsonSerializable(typeof(RefRestrictionCreate))]

// ============================================================================
// RefSync Models
// ============================================================================
[JsonSerializable(typeof(FullRef))]
[JsonSerializable(typeof(RepositorySynchronizationStatus))]
[JsonSerializable(typeof(Synchronize))]
[JsonSerializable(typeof(SynchronizeContext))]

// ============================================================================
// Ssh Models
// ============================================================================
[JsonSerializable(typeof(Accesskeys))]
[JsonSerializable(typeof(Fingerprint))]
[JsonSerializable(typeof(KeyBase))]
[JsonSerializable(typeof(ProjectKey))]
[JsonSerializable(typeof(RepositoryKey))]
[JsonSerializable(typeof(SshSettings))]

// ============================================================================
// Collection Types (for various API responses)
// ============================================================================
[JsonSerializable(typeof(IEnumerable<Error>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<CloneLink>))]
[JsonSerializable(typeof(List<Link>))]
[JsonSerializable(typeof(List<Participant>))]
[JsonSerializable(typeof(List<Reviewer>))]
[JsonSerializable(typeof(List<Comment>))]
[JsonSerializable(typeof(List<Commit>))]
[JsonSerializable(typeof(List<CommitParent>))]
[JsonSerializable(typeof(List<Line>))]
[JsonSerializable(typeof(List<Segment>))]
[JsonSerializable(typeof(List<DiffHunk>))]
[JsonSerializable(typeof(List<Diff>))]
[JsonSerializable(typeof(List<ContentItem>))]
[JsonSerializable(typeof(Dictionary<string, object>))]

/// <summary>
/// Provides the source-generated JSON serialization context for Bitbucket.Net.
/// This context enables AOT compilation, trimming support, and improved serialization performance.
/// </summary>
/// <remarks>
/// Consumers can use this context directly for advanced scenarios:
/// <code>
/// var options = new JsonSerializerOptions
/// {
///     TypeInfoResolver = BitbucketJsonContext.Default
/// };
/// </code>
/// </remarks>
public partial class BitbucketJsonContext : JsonSerializerContext
{
}