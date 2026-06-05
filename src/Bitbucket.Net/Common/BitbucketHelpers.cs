using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;

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

    #endregion

    #region RepositoryArchivedState

    /// <summary>
    /// Converts a <see cref="RepositoryArchivedState"/> value to the Bitbucket API string
    /// (<c>ACTIVE</c>, <c>ARCHIVED</c>, or <c>ALL</c>).
    /// </summary>
    /// <param name="archived">The archived-state filter to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string RepositoryArchivedStateToString(RepositoryArchivedState archived)
        => BitbucketEnumMaps.RepositoryArchivedState.ToApiString(archived);

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

    #endregion
}