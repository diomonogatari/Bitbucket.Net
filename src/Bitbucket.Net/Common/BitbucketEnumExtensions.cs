using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net.Common;

/// <summary>
/// Extension methods for converting Bitbucket API enums to their wire-format strings.
/// </summary>
public static class BitbucketEnumExtensions
{
    public static string ToApiString(this BranchOrderBy value)
        => BitbucketEnumMaps.BranchOrderBy.ToApiString(value);

    public static string ToApiString(this PullRequestDirections value)
        => BitbucketEnumMaps.PullRequestDirections.ToApiString(value);

    public static string ToApiString(this PullRequestStates value)
        => BitbucketEnumMaps.PullRequestStates.ToApiString(value);

    public static string? ToApiString(this PullRequestStates? value)
        => BitbucketEnumMaps.PullRequestStates.ToApiString(value);

    public static string ToApiString(this PullRequestOrders value)
        => BitbucketEnumMaps.PullRequestOrders.ToApiString(value);

    public static string? ToApiString(this PullRequestOrders? value)
        => BitbucketEnumMaps.PullRequestOrders.ToApiString(value);

    public static string ToApiString(this PullRequestFromTypes value)
        => BitbucketEnumMaps.PullRequestFromTypes.ToApiString(value);

    public static string? ToApiString(this PullRequestFromTypes? value)
        => BitbucketEnumMaps.PullRequestFromTypes.ToApiString(value);

    public static string ToApiString(this Permissions value)
        => BitbucketEnumMaps.Permissions.ToApiString(value);

    public static string? ToApiString(this Permissions? value)
        => BitbucketEnumMaps.Permissions.ToApiString(value);

    public static string ToApiString(this MergeCommits value)
        => BitbucketEnumMaps.MergeCommits.ToApiString(value);

    public static string ToApiString(this Roles value)
        => BitbucketEnumMaps.Roles.ToApiString(value);

    public static string? ToApiString(this Roles? value)
        => BitbucketEnumMaps.Roles.ToApiString(value);

    public static string ToApiString(this LineTypes value)
        => BitbucketEnumMaps.LineTypes.ToApiString(value);

    public static string? ToApiString(this LineTypes? value)
        => BitbucketEnumMaps.LineTypes.ToApiString(value);

    public static string ToApiString(this FileTypes value)
        => BitbucketEnumMaps.FileTypes.ToApiString(value);

    public static string? ToApiString(this FileTypes? value)
        => BitbucketEnumMaps.FileTypes.ToApiString(value);

    public static string ToApiString(this ChangeScopes value)
        => BitbucketEnumMaps.ChangeScopes.ToApiString(value);

    public static string ToApiString(this LogLevels value)
        => BitbucketEnumMaps.LogLevels.ToApiString(value);

    public static string ToApiString(this ParticipantStatus value)
        => BitbucketEnumMaps.ParticipantStatus.ToApiString(value);

    public static string ToApiString(this HookTypes value)
        => BitbucketEnumMaps.HookTypes.ToApiString(value);

    public static string ToApiString(this ScopeTypes value)
        => BitbucketEnumMaps.ScopeTypes.ToApiString(value);

    public static string ToApiString(this ArchiveFormats value)
        => BitbucketEnumMaps.ArchiveFormats.ToApiString(value);

    public static string ToApiString(this WebHookOutcomes value)
        => BitbucketEnumMaps.WebHookOutcomes.ToApiString(value);

    public static string? ToApiString(this WebHookOutcomes? value)
        => BitbucketEnumMaps.WebHookOutcomes.ToApiString(value);

    public static string ToApiString(this AnchorStates value)
        => BitbucketEnumMaps.AnchorStates.ToApiString(value);

    public static string ToApiString(this DiffTypes value)
        => BitbucketEnumMaps.DiffTypes.ToApiString(value);

    public static string? ToApiString(this DiffTypes? value)
        => BitbucketEnumMaps.DiffTypes.ToApiString(value);

    public static string ToApiString(this TagTypes value)
        => BitbucketEnumMaps.TagTypes.ToApiString(value);

    public static string ToApiString(this RefRestrictionTypes value)
        => BitbucketEnumMaps.RefRestrictionTypes.ToApiString(value);

    public static string? ToApiString(this RefRestrictionTypes? value)
        => BitbucketEnumMaps.RefRestrictionTypes.ToApiString(value);

    public static string ToApiString(this RefMatcherTypes value)
        => BitbucketEnumMaps.RefMatcherTypes.ToApiString(value);

    public static string? ToApiString(this RefMatcherTypes? value)
        => BitbucketEnumMaps.RefMatcherTypes.ToApiString(value);

    public static string ToApiString(this SynchronizeActions value)
        => BitbucketEnumMaps.SynchronizeActions.ToApiString(value);

    public static string ToApiString(this BlockerCommentState value)
        => BitbucketEnumMaps.BlockerCommentState.ToApiString(value);

    public static string? ToApiString(this BlockerCommentState? value)
        => BitbucketEnumMaps.BlockerCommentState.ToApiString(value);

    public static string ToApiString(this CommentSeverity value)
        => BitbucketEnumMaps.CommentSeverity.ToApiString(value);

    public static string? ToApiString(this CommentSeverity? value)
        => BitbucketEnumMaps.CommentSeverity.ToApiString(value);
}