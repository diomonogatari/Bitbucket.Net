using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net.Common;

/// <summary>
/// Helper methods for converting between Bitbucket enum values and their wire-format string representations.
/// Delegates to <see cref="BitbucketEnumMaps"/> for all mappings.
/// </summary>
public static class BitbucketHelpers
{
    #region Bool

    /// <summary>
    /// Converts a boolean value to the lowercase string expected by Bitbucket query parameters.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <returns><c>"true"</c> or <c>"false"</c>.</returns>
    public static string BoolToString(bool value) => value
        ? "true"
        : "false";

    /// <summary>
    /// Converts an optional boolean value to the lowercase string expected by Bitbucket query parameters.
    /// </summary>
    /// <param name="value">The optional boolean value to convert.</param>
    /// <returns><c>"true"</c>, <c>"false"</c>, or <see langword="null"/> when no value is supplied.</returns>
    public static string? BoolToString(bool? value) => value.HasValue
        ? BoolToString(value.Value)
        : null;

    /// <summary>
    /// Parses a case-insensitive boolean string returned by the Bitbucket API.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <returns><see langword="true"/> when the value is "true"; otherwise <see langword="false"/>.</returns>
    public static bool StringToBool(string value) => value.Equals("true", StringComparison.OrdinalIgnoreCase);

    #endregion

    #region BranchOrderBy

    /// <summary>
    /// Converts a <see cref="BranchOrderBy"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="orderBy">The ordering to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string BranchOrderByToString(BranchOrderBy orderBy)
        => BitbucketEnumMaps.BranchOrderBy.ToApiString(orderBy);

    #endregion

    #region PullRequestDirections

    /// <summary>
    /// Converts a <see cref="PullRequestDirections"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestDirectionToString(PullRequestDirections direction)
        => BitbucketEnumMaps.PullRequestDirections.ToApiString(direction);

    #endregion

    #region PullRequestStates

    /// <summary>
    /// Converts a <see cref="PullRequestStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestStateToString(PullRequestStates state)
        => BitbucketEnumMaps.PullRequestStates.ToApiString(state);

    /// <summary>
    /// Converts an optional <see cref="PullRequestStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no state is provided.</returns>
    public static string? PullRequestStateToString(PullRequestStates? state)
        => BitbucketEnumMaps.PullRequestStates.ToApiString(state);

    /// <summary>
    /// Parses a Bitbucket pull request state string into a <see cref="PullRequestStates"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed state.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static PullRequestStates StringToPullRequestState(string s)
        => BitbucketEnumMaps.PullRequestStates.FromApiString(s);

    #endregion

    #region PullRequestOrders

    /// <summary>
    /// Converts a <see cref="PullRequestOrders"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="order">The order to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestOrderToString(PullRequestOrders order)
        => BitbucketEnumMaps.PullRequestOrders.ToApiString(order);

    /// <summary>
    /// Converts an optional <see cref="PullRequestOrders"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="order">The order to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no order is provided.</returns>
    public static string? PullRequestOrderToString(PullRequestOrders? order)
        => BitbucketEnumMaps.PullRequestOrders.ToApiString(order);

    #endregion

    #region PullRequestFromTypes

    /// <summary>
    /// Converts a <see cref="PullRequestFromTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fromType">The source type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    private static string PullRequestFromTypeToString(PullRequestFromTypes fromType)
        => BitbucketEnumMaps.PullRequestFromTypes.ToApiString(fromType);

    /// <summary>
    /// Converts an optional <see cref="PullRequestFromTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fromType">The source type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no source is provided.</returns>
    public static string? PullRequestFromTypeToString(PullRequestFromTypes? fromType)
        => BitbucketEnumMaps.PullRequestFromTypes.ToApiString(fromType);

    #endregion

    #region Permissions

    /// <summary>
    /// Converts a <see cref="Permissions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="permission">The permission to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PermissionToString(Permissions permission)
        => BitbucketEnumMaps.Permissions.ToApiString(permission);

    /// <summary>
    /// Converts an optional <see cref="Permissions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="permission">The permission to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? PermissionToString(Permissions? permission)
        => BitbucketEnumMaps.Permissions.ToApiString(permission);

    /// <summary>
    /// Parses a Bitbucket permission string into a <see cref="Permissions"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed permission.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static Permissions StringToPermission(string s)
        => BitbucketEnumMaps.Permissions.FromApiString(s);

    #endregion

    #region MergeCommits

    /// <summary>
    /// Converts a <see cref="MergeCommits"/> preference to the Bitbucket API string.
    /// </summary>
    /// <param name="mergeCommits">The merge commit option.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string MergeCommitsToString(MergeCommits mergeCommits)
        => BitbucketEnumMaps.MergeCommits.ToApiString(mergeCommits);

    #endregion

    #region Roles

    /// <summary>
    /// Converts a pull request <see cref="Roles"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="role">The role to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string RoleToString(Roles role)
        => BitbucketEnumMaps.Roles.ToApiString(role);

    /// <summary>
    /// Converts an optional pull request <see cref="Roles"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="role">The role to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RoleToString(Roles? role)
        => BitbucketEnumMaps.Roles.ToApiString(role);

    /// <summary>
    /// Parses a pull request role string into a <see cref="Roles"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed role.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static Roles StringToRole(string s)
        => BitbucketEnumMaps.Roles.FromApiString(s);

    #endregion

    #region LineTypes

    /// <summary>
    /// Converts a <see cref="LineTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="lineType">The line type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string LineTypeToString(LineTypes lineType)
        => BitbucketEnumMaps.LineTypes.ToApiString(lineType);

    /// <summary>
    /// Converts an optional <see cref="LineTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="lineType">The line type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? LineTypeToString(LineTypes? lineType)
        => BitbucketEnumMaps.LineTypes.ToApiString(lineType);

    /// <summary>
    /// Parses a line type string into a <see cref="LineTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed line type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static LineTypes StringToLineType(string s)
        => BitbucketEnumMaps.LineTypes.FromApiString(s);

    #endregion

    #region FileTypes

    /// <summary>
    /// Converts a <see cref="FileTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fileType">The file type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string FileTypeToString(FileTypes fileType)
        => BitbucketEnumMaps.FileTypes.ToApiString(fileType);

    /// <summary>
    /// Converts an optional <see cref="FileTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fileType">The file type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? FileTypeToString(FileTypes? fileType)
        => BitbucketEnumMaps.FileTypes.ToApiString(fileType);

    /// <summary>
    /// Parses a file type string into a <see cref="FileTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed file type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static FileTypes StringToFileType(string s)
        => BitbucketEnumMaps.FileTypes.FromApiString(s);

    #endregion

    #region ChangeScopes

    /// <summary>
    /// Converts a <see cref="ChangeScopes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="changeScope">The change scope to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ChangeScopeToString(ChangeScopes changeScope)
        => BitbucketEnumMaps.ChangeScopes.ToApiString(changeScope);

    #endregion

    #region LogLevels

    /// <summary>
    /// Converts a <see cref="LogLevels"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="logLevel">The log level to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string LogLevelToString(LogLevels logLevel)
        => BitbucketEnumMaps.LogLevels.ToApiString(logLevel);

    /// <summary>
    /// Parses a log level string into a <see cref="LogLevels"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed log level.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static LogLevels StringToLogLevel(string s)
        => BitbucketEnumMaps.LogLevels.FromApiString(s);

    #endregion

    #region ParticipantStatus

    /// <summary>
    /// Converts a <see cref="ParticipantStatus"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="participantStatus">The status to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ParticipantStatusToString(ParticipantStatus participantStatus)
        => BitbucketEnumMaps.ParticipantStatus.ToApiString(participantStatus);

    /// <summary>
    /// Parses a participant status string into a <see cref="ParticipantStatus"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed status.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static ParticipantStatus StringToParticipantStatus(string s)
        => BitbucketEnumMaps.ParticipantStatus.FromApiString(s);

    #endregion

    #region HookTypes

    /// <summary>
    /// Converts a hook <see cref="HookTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="hookType">The hook type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string HookTypeToString(HookTypes hookType)
        => BitbucketEnumMaps.HookTypes.ToApiString(hookType);

    /// <summary>
    /// Parses a hook type string into a <see cref="HookTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed hook type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static HookTypes StringToHookType(string s)
        => BitbucketEnumMaps.HookTypes.FromApiString(s);

    #endregion

    #region ScopeTypes

    /// <summary>
    /// Converts a <see cref="ScopeTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="scopeType">The scope type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ScopeTypeToString(ScopeTypes scopeType)
        => BitbucketEnumMaps.ScopeTypes.ToApiString(scopeType);

    /// <summary>
    /// Parses a scope type string into a <see cref="ScopeTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed scope type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static ScopeTypes StringToScopeType(string s)
        => BitbucketEnumMaps.ScopeTypes.FromApiString(s);

    #endregion

    #region ArchiveFormats

    /// <summary>
    /// Converts an <see cref="ArchiveFormats"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="archiveFormat">The archive format to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ArchiveFormatToString(ArchiveFormats archiveFormat)
        => BitbucketEnumMaps.ArchiveFormats.ToApiString(archiveFormat);

    #endregion

    #region WebHookOutcomes

    /// <summary>
    /// Converts a <see cref="WebHookOutcomes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="webHookOutcome">The outcome to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string WebHookOutcomeToString(WebHookOutcomes webHookOutcome)
        => BitbucketEnumMaps.WebHookOutcomes.ToApiString(webHookOutcome);

    /// <summary>
    /// Converts an optional <see cref="WebHookOutcomes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="webHookOutcome">The outcome to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? WebHookOutcomeToString(WebHookOutcomes? webHookOutcome)
        => BitbucketEnumMaps.WebHookOutcomes.ToApiString(webHookOutcome);

    /// <summary>
    /// Parses a webhook outcome string into a <see cref="WebHookOutcomes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed outcome.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static WebHookOutcomes StringToWebHookOutcome(string s)
        => BitbucketEnumMaps.WebHookOutcomes.FromApiString(s);

    #endregion

    #region AnchorStates

    /// <summary>
    /// Converts an <see cref="AnchorStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="anchorState">The anchor state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string AnchorStateToString(AnchorStates anchorState)
        => BitbucketEnumMaps.AnchorStates.ToApiString(anchorState);

    #endregion

    #region DiffTypes

    /// <summary>
    /// Converts a <see cref="DiffTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="diffType">The diff type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string DiffTypeToString(DiffTypes diffType)
        => BitbucketEnumMaps.DiffTypes.ToApiString(diffType);

    /// <summary>
    /// Converts an optional <see cref="DiffTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="diffType">The diff type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? DiffTypeToString(DiffTypes? diffType)
        => BitbucketEnumMaps.DiffTypes.ToApiString(diffType);

    #endregion

    #region TagTypes

    /// <summary>
    /// Converts a <see cref="TagTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="tagType">The tag type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string TagTypeToString(TagTypes tagType)
        => BitbucketEnumMaps.TagTypes.ToApiString(tagType);

    #endregion

    #region RefRestrictionTypes

    /// <summary>
    /// Converts a <see cref="RefRestrictionTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refRestrictionType">The restriction to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string RefRestrictionTypeToString(RefRestrictionTypes refRestrictionType)
        => BitbucketEnumMaps.RefRestrictionTypes.ToApiString(refRestrictionType);

    /// <summary>
    /// Converts an optional <see cref="RefRestrictionTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refRestrictionType">The restriction to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RefRestrictionTypeToString(RefRestrictionTypes? refRestrictionType)
        => BitbucketEnumMaps.RefRestrictionTypes.ToApiString(refRestrictionType);

    /// <summary>
    /// Parses a ref restriction string into a <see cref="RefRestrictionTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed restriction.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static RefRestrictionTypes StringToRefRestrictionType(string s)
        => BitbucketEnumMaps.RefRestrictionTypes.FromApiString(s);

    #endregion

    #region RefMatcherTypes

    /// <summary>
    /// Converts a <see cref="RefMatcherTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refMatcherType">The matcher type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    private static string RefMatcherTypeToString(RefMatcherTypes refMatcherType)
        => BitbucketEnumMaps.RefMatcherTypes.ToApiString(refMatcherType);

    /// <summary>
    /// Converts an optional <see cref="RefMatcherTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refMatcherType">The matcher type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RefMatcherTypeToString(RefMatcherTypes? refMatcherType)
        => BitbucketEnumMaps.RefMatcherTypes.ToApiString(refMatcherType);

    #endregion

    #region SynchronizeActions

    /// <summary>
    /// Converts a <see cref="SynchronizeActions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="synchronizeAction">The synchronization action to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string SynchronizeActionToString(SynchronizeActions synchronizeAction)
        => BitbucketEnumMaps.SynchronizeActions.ToApiString(synchronizeAction);

    /// <summary>
    /// Parses a synchronization action string into a <see cref="SynchronizeActions"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed action.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static SynchronizeActions StringToSynchronizeAction(string s)
        => BitbucketEnumMaps.SynchronizeActions.FromApiString(s);

    #endregion

    #region BlockerCommentState

    /// <summary>
    /// Converts a <see cref="BlockerCommentState"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The blocker comment state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string BlockerCommentStateToString(BlockerCommentState state)
        => BitbucketEnumMaps.BlockerCommentState.ToApiString(state);

    /// <summary>
    /// Converts an optional <see cref="BlockerCommentState"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The blocker comment state to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? BlockerCommentStateToString(BlockerCommentState? state)
        => BitbucketEnumMaps.BlockerCommentState.ToApiString(state);

    /// <summary>
    /// Parses a blocker comment state string into a <see cref="BlockerCommentState"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed state.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static BlockerCommentState StringToBlockerCommentState(string s)
        => BitbucketEnumMaps.BlockerCommentState.FromApiString(s);

    #endregion

    #region CommentSeverity

    /// <summary>
    /// Converts a <see cref="CommentSeverity"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="severity">The comment severity to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string CommentSeverityToString(CommentSeverity severity)
        => BitbucketEnumMaps.CommentSeverity.ToApiString(severity);

    /// <summary>
    /// Converts an optional <see cref="CommentSeverity"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="severity">The comment severity to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? CommentSeverityToString(CommentSeverity? severity)
        => BitbucketEnumMaps.CommentSeverity.ToApiString(severity);

    /// <summary>
    /// Parses a comment severity string into a <see cref="CommentSeverity"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed severity.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static CommentSeverity StringToCommentSeverity(string s)
        => BitbucketEnumMaps.CommentSeverity.FromApiString(s);

    #endregion
}