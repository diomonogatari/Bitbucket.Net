using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net.Common;

/// <summary>
/// Central registry of all enum-to-string mappings used by the Bitbucket API.
/// Each <see cref="EnumMap{TEnum}"/> is the single source of truth for both
/// JSON converters and query-parameter serialization.
/// </summary>
public static class BitbucketEnumMaps
{
    /// <summary>Mapping for <see cref="BranchOrderBy"/>.</summary>
    public static EnumMap<BranchOrderBy> BranchOrderBy { get; } = new(new Dictionary<BranchOrderBy, string>
    {
        [Bitbucket.Net.Models.Core.Projects.BranchOrderBy.Alphabetical] = "ALPHABETICAL",
        [Bitbucket.Net.Models.Core.Projects.BranchOrderBy.Modification] = "MODIFICATION",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="PullRequestDirections"/>.</summary>
    public static EnumMap<PullRequestDirections> PullRequestDirections { get; } = new(new Dictionary<PullRequestDirections, string>
    {
        [Bitbucket.Net.Models.Core.Projects.PullRequestDirections.Incoming] = "INCOMING",
        [Bitbucket.Net.Models.Core.Projects.PullRequestDirections.Outgoing] = "OUTGOING",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="PullRequestStates"/>.</summary>
    public static EnumMap<PullRequestStates> PullRequestStates { get; } = new(new Dictionary<PullRequestStates, string>
    {
        [Bitbucket.Net.Models.Core.Projects.PullRequestStates.Open] = "OPEN",
        [Bitbucket.Net.Models.Core.Projects.PullRequestStates.Declined] = "DECLINED",
        [Bitbucket.Net.Models.Core.Projects.PullRequestStates.Merged] = "MERGED",
        [Bitbucket.Net.Models.Core.Projects.PullRequestStates.All] = "ALL",
    });

    /// <summary>Mapping for <see cref="PullRequestOrders"/>.</summary>
    public static EnumMap<PullRequestOrders> PullRequestOrders { get; } = new(new Dictionary<PullRequestOrders, string>
    {
        [Bitbucket.Net.Models.Core.Projects.PullRequestOrders.Newest] = "NEWEST",
        [Bitbucket.Net.Models.Core.Projects.PullRequestOrders.Oldest] = "OLDEST",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="PullRequestFromTypes"/>.</summary>
    public static EnumMap<PullRequestFromTypes> PullRequestFromTypes { get; } = new(new Dictionary<PullRequestFromTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.PullRequestFromTypes.Comment] = "COMMENT",
        [Bitbucket.Net.Models.Core.Projects.PullRequestFromTypes.Activity] = "ACTIVITY",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="Permissions"/>.</summary>
    public static EnumMap<Permissions> Permissions { get; } = new(new Dictionary<Permissions, string>
    {
        [Bitbucket.Net.Models.Core.Admin.Permissions.Admin] = "ADMIN",
        [Bitbucket.Net.Models.Core.Admin.Permissions.LicensedUser] = "LICENSED_USER",
        [Bitbucket.Net.Models.Core.Admin.Permissions.ProjectAdmin] = "PROJECT_ADMIN",
        [Bitbucket.Net.Models.Core.Admin.Permissions.ProjectCreate] = "PROJECT_CREATE",
        [Bitbucket.Net.Models.Core.Admin.Permissions.ProjectRead] = "PROJECT_READ",
        [Bitbucket.Net.Models.Core.Admin.Permissions.ProjectView] = "PROJECT_VIEW",
        [Bitbucket.Net.Models.Core.Admin.Permissions.ProjectWrite] = "PROJECT_WRITE",
        [Bitbucket.Net.Models.Core.Admin.Permissions.RepoAdmin] = "REPO_ADMIN",
        [Bitbucket.Net.Models.Core.Admin.Permissions.RepoRead] = "REPO_READ",
        [Bitbucket.Net.Models.Core.Admin.Permissions.RepoWrite] = "REPO_WRITE",
        [Bitbucket.Net.Models.Core.Admin.Permissions.SysAdmin] = "SYS_ADMIN",
    });

    /// <summary>Mapping for <see cref="MergeCommits"/>.</summary>
    public static EnumMap<MergeCommits> MergeCommits { get; } = new(new Dictionary<MergeCommits, string>
    {
        [Bitbucket.Net.Models.Core.Projects.MergeCommits.Exclude] = "exclude",
        [Bitbucket.Net.Models.Core.Projects.MergeCommits.Include] = "include",
        [Bitbucket.Net.Models.Core.Projects.MergeCommits.Only] = "only",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="Roles"/>.</summary>
    public static EnumMap<Roles> Roles { get; } = new(new Dictionary<Roles, string>
    {
        [Bitbucket.Net.Models.Core.Projects.Roles.Author] = "AUTHOR",
        [Bitbucket.Net.Models.Core.Projects.Roles.Reviewer] = "REVIEWER",
        [Bitbucket.Net.Models.Core.Projects.Roles.Participant] = "PARTICIPANT",
    });

    /// <summary>Mapping for <see cref="LineTypes"/>.</summary>
    public static EnumMap<LineTypes> LineTypes { get; } = new(new Dictionary<LineTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.LineTypes.Added] = "ADDED",
        [Bitbucket.Net.Models.Core.Projects.LineTypes.Removed] = "REMOVED",
        [Bitbucket.Net.Models.Core.Projects.LineTypes.Context] = "CONTEXT",
    });

    /// <summary>Mapping for <see cref="FileTypes"/>.</summary>
    public static EnumMap<FileTypes> FileTypes { get; } = new(new Dictionary<FileTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.FileTypes.From] = "FROM",
        [Bitbucket.Net.Models.Core.Projects.FileTypes.To] = "TO",
    });

    /// <summary>Mapping for <see cref="ChangeScopes"/>.</summary>
    public static EnumMap<ChangeScopes> ChangeScopes { get; } = new(new Dictionary<ChangeScopes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.ChangeScopes.All] = "ALL",
        [Bitbucket.Net.Models.Core.Projects.ChangeScopes.Unreviewed] = "UNREVIEWED",
        [Bitbucket.Net.Models.Core.Projects.ChangeScopes.Range] = "RANGE",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="LogLevels"/>.</summary>
    public static EnumMap<LogLevels> LogLevels { get; } = new(new Dictionary<LogLevels, string>
    {
        [Bitbucket.Net.Models.Core.Logs.LogLevels.Trace] = "TRACE",
        [Bitbucket.Net.Models.Core.Logs.LogLevels.Debug] = "DEBUG",
        [Bitbucket.Net.Models.Core.Logs.LogLevels.Info] = "INFO",
        [Bitbucket.Net.Models.Core.Logs.LogLevels.Warn] = "WARN",
        [Bitbucket.Net.Models.Core.Logs.LogLevels.Error] = "ERROR",
    });

    /// <summary>Mapping for <see cref="ParticipantStatus"/>.</summary>
    public static EnumMap<ParticipantStatus> ParticipantStatus { get; } = new(new Dictionary<ParticipantStatus, string>
    {
        [Bitbucket.Net.Models.Core.Projects.ParticipantStatus.Approved] = "APPROVED",
        [Bitbucket.Net.Models.Core.Projects.ParticipantStatus.NeedsWork] = "NEEDS_WORK",
        [Bitbucket.Net.Models.Core.Projects.ParticipantStatus.Unapproved] = "UNAPPROVED",
    });

    /// <summary>Mapping for <see cref="HookTypes"/>.</summary>
    public static EnumMap<HookTypes> HookTypes { get; } = new(new Dictionary<HookTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.HookTypes.PreReceive] = "PRE_RECEIVE",
        [Bitbucket.Net.Models.Core.Projects.HookTypes.PostReceive] = "POST_RECEIVE",
        [Bitbucket.Net.Models.Core.Projects.HookTypes.PrePullRequestMerge] = "PRE_PULL_REQUEST_MERGE",
    });

    /// <summary>Mapping for <see cref="ScopeTypes"/>.</summary>
    public static EnumMap<ScopeTypes> ScopeTypes { get; } = new(new Dictionary<ScopeTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.ScopeTypes.Global] = "GLOBAL",
        [Bitbucket.Net.Models.Core.Projects.ScopeTypes.Project] = "PROJECT",
        [Bitbucket.Net.Models.Core.Projects.ScopeTypes.Repository] = "REPOSITORY",
    });

    /// <summary>Mapping for <see cref="ArchiveFormats"/>.</summary>
    public static EnumMap<ArchiveFormats> ArchiveFormats { get; } = new(new Dictionary<ArchiveFormats, string>
    {
        [Bitbucket.Net.Models.Core.Projects.ArchiveFormats.Zip] = "zip",
        [Bitbucket.Net.Models.Core.Projects.ArchiveFormats.Tar] = "tar",
        [Bitbucket.Net.Models.Core.Projects.ArchiveFormats.TarGz] = "tar.gz",
        [Bitbucket.Net.Models.Core.Projects.ArchiveFormats.Tgz] = "tgz",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="WebHookOutcomes"/>.</summary>
    public static EnumMap<WebHookOutcomes> WebHookOutcomes { get; } = new(new Dictionary<WebHookOutcomes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.WebHookOutcomes.Success] = "SUCCESS",
        [Bitbucket.Net.Models.Core.Projects.WebHookOutcomes.Failure] = "FAILURE",
        [Bitbucket.Net.Models.Core.Projects.WebHookOutcomes.Error] = "ERROR",
    });

    /// <summary>Mapping for <see cref="AnchorStates"/>.</summary>
    public static EnumMap<AnchorStates> AnchorStates { get; } = new(new Dictionary<AnchorStates, string>
    {
        [Bitbucket.Net.Models.Core.Projects.AnchorStates.Active] = "ACTIVE",
        [Bitbucket.Net.Models.Core.Projects.AnchorStates.Orphaned] = "ORPHANED",
        [Bitbucket.Net.Models.Core.Projects.AnchorStates.All] = "ALL",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="DiffTypes"/>.</summary>
    public static EnumMap<DiffTypes> DiffTypes { get; } = new(new Dictionary<DiffTypes, string>
    {
        [Bitbucket.Net.Models.Core.Projects.DiffTypes.Effective] = "EFFECTIVE",
        [Bitbucket.Net.Models.Core.Projects.DiffTypes.Range] = "RANGE",
        [Bitbucket.Net.Models.Core.Projects.DiffTypes.Commit] = "COMMIT",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="TagTypes"/>.</summary>
    public static EnumMap<TagTypes> TagTypes { get; } = new(new Dictionary<TagTypes, string>
    {
        [Bitbucket.Net.Models.Git.TagTypes.LightWeight] = "LIGHTWEIGHT",
        [Bitbucket.Net.Models.Git.TagTypes.Annotated] = "ANNOTATED",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="RefRestrictionTypes"/>.</summary>
    public static EnumMap<RefRestrictionTypes> RefRestrictionTypes { get; } = new(new Dictionary<RefRestrictionTypes, string>
    {
        [Bitbucket.Net.Models.RefRestrictions.RefRestrictionTypes.AllChanges] = "read-only",
        [Bitbucket.Net.Models.RefRestrictions.RefRestrictionTypes.RewritingHistory] = "fast-forward-only",
        [Bitbucket.Net.Models.RefRestrictions.RefRestrictionTypes.Deletion] = "no-deletes",
        [Bitbucket.Net.Models.RefRestrictions.RefRestrictionTypes.ChangesWithoutPullRequest] = "pull-request-only",
    });

    /// <summary>Mapping for <see cref="RefMatcherTypes"/>.</summary>
    public static EnumMap<RefMatcherTypes> RefMatcherTypes { get; } = new(new Dictionary<RefMatcherTypes, string>
    {
        [Bitbucket.Net.Models.RefRestrictions.RefMatcherTypes.Branch] = "BRANCH",
        [Bitbucket.Net.Models.RefRestrictions.RefMatcherTypes.Pattern] = "PATTERN",
        [Bitbucket.Net.Models.RefRestrictions.RefMatcherTypes.ModelCategory] = "MODEL_CATEGORY",
        [Bitbucket.Net.Models.RefRestrictions.RefMatcherTypes.ModelBranch] = "MODEL_BRANCH",
    }, createReverse: false);

    /// <summary>Mapping for <see cref="SynchronizeActions"/>.</summary>
    public static EnumMap<SynchronizeActions> SynchronizeActions { get; } = new(new Dictionary<SynchronizeActions, string>
    {
        [Bitbucket.Net.Models.RefSync.SynchronizeActions.Merge] = "MERGE",
        [Bitbucket.Net.Models.RefSync.SynchronizeActions.Discard] = "DISCARD",
    });

    /// <summary>Mapping for <see cref="BlockerCommentState"/>.</summary>
    public static EnumMap<BlockerCommentState> BlockerCommentState { get; } = new(new Dictionary<BlockerCommentState, string>
    {
        [Bitbucket.Net.Models.Core.Projects.BlockerCommentState.Open] = "OPEN",
        [Bitbucket.Net.Models.Core.Projects.BlockerCommentState.Resolved] = "RESOLVED",
    });

    /// <summary>Mapping for <see cref="CommentSeverity"/>.</summary>
    public static EnumMap<CommentSeverity> CommentSeverity { get; } = new(new Dictionary<CommentSeverity, string>
    {
        [Bitbucket.Net.Models.Core.Projects.CommentSeverity.Normal] = "NORMAL",
        [Bitbucket.Net.Models.Core.Projects.CommentSeverity.Blocker] = "BLOCKER",
    });
}