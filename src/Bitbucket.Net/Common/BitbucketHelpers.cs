using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;
using System.Collections.Frozen;

namespace Bitbucket.Net.Common;

/// <summary>
/// Helper methods for converting between Bitbucket enum values and their wire-format string representations.
/// Uses <see cref="FrozenDictionary{TKey, TValue}"/> for optimal read-only lookup performance.
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

    private static readonly FrozenDictionary<BranchOrderBy, string> s_stringByBranchOrderBy = new Dictionary<BranchOrderBy, string>
    {
        [BranchOrderBy.Alphabetical] = "ALPHABETICAL",
        [BranchOrderBy.Modification] = "MODIFICATION",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="BranchOrderBy"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="orderBy">The ordering to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string BranchOrderByToString(BranchOrderBy orderBy)
    {
        if (!s_stringByBranchOrderBy.TryGetValue(orderBy, out string? result))
        {
            throw new ArgumentException($"Unknown branch order by: {orderBy}");
        }

        return result;
    }

    #endregion

    #region PullRequestDirections

    private static readonly FrozenDictionary<PullRequestDirections, string> s_stringByPullRequestDirection = new Dictionary<PullRequestDirections, string>
    {
        [PullRequestDirections.Incoming] = "INCOMING",
        [PullRequestDirections.Outgoing] = "OUTGOING",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="PullRequestDirections"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="direction">The direction to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestDirectionToString(PullRequestDirections direction)
    {
        if (!s_stringByPullRequestDirection.TryGetValue(direction, out string? result))
        {
            throw new ArgumentException($"Unknown pull request direction: {direction}");
        }

        return result;
    }

    #endregion

    #region PullRequestStates

    private static readonly FrozenDictionary<PullRequestStates, string> s_stringByPullRequestState = new Dictionary<PullRequestStates, string>
    {
        [PullRequestStates.Open] = "OPEN",
        [PullRequestStates.Declined] = "DECLINED",
        [PullRequestStates.Merged] = "MERGED",
        [PullRequestStates.All] = "ALL",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, PullRequestStates> s_pullRequestStateByString =
        s_stringByPullRequestState.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="PullRequestStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestStateToString(PullRequestStates state)
    {
        if (!s_stringByPullRequestState.TryGetValue(state, out string? result))
        {
            throw new ArgumentException($"Unknown pull request state: {state}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="PullRequestStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The state to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no state is provided.</returns>
    public static string? PullRequestStateToString(PullRequestStates? state) => state.HasValue
        ? PullRequestStateToString(state.Value)
        : null;

    /// <summary>
    /// Parses a Bitbucket pull request state string into a <see cref="PullRequestStates"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed state.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static PullRequestStates StringToPullRequestState(string s)
    {
        if (!s_pullRequestStateByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown pull request state: {s}");
        }

        return result;
    }

    #endregion

    #region PullRequestOrders

    private static readonly FrozenDictionary<PullRequestOrders, string> s_stringByPullRequestOrder = new Dictionary<PullRequestOrders, string>
    {
        [PullRequestOrders.Newest] = "NEWEST",
        [PullRequestOrders.Oldest] = "OLDEST",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="PullRequestOrders"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="order">The order to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PullRequestOrderToString(PullRequestOrders order)
    {
        if (!s_stringByPullRequestOrder.TryGetValue(order, out string? result))
        {
            throw new ArgumentException($"Unknown pull request order: {order}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="PullRequestOrders"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="order">The order to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no order is provided.</returns>
    public static string? PullRequestOrderToString(PullRequestOrders? order) => order.HasValue
        ? PullRequestOrderToString(order.Value)
        : null;

    #endregion

    #region PullRequestFromTypes

    private static readonly FrozenDictionary<PullRequestFromTypes, string> s_stringByPullRequestFromType = new Dictionary<PullRequestFromTypes, string>
    {
        [PullRequestFromTypes.Comment] = "COMMENT",
        [PullRequestFromTypes.Activity] = "ACTIVITY",
    }.ToFrozenDictionary();

    private static string PullRequestFromTypeToString(PullRequestFromTypes fromType)
    {
        if (!s_stringByPullRequestFromType.TryGetValue(fromType, out string? result))
        {
            throw new ArgumentException($"Unknown pull request from type: {fromType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="PullRequestFromTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fromType">The source type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when no source is provided.</returns>
    public static string? PullRequestFromTypeToString(PullRequestFromTypes? fromType) => fromType.HasValue
        ? PullRequestFromTypeToString(fromType.Value)
        : null;

    #endregion

    #region Permissions

    private static readonly FrozenDictionary<Permissions, string> s_stringByPermissions = new Dictionary<Permissions, string>
    {
        [Permissions.Admin] = "ADMIN",
        [Permissions.LicensedUser] = "LICENSED_USER",
        [Permissions.ProjectAdmin] = "PROJECT_ADMIN",
        [Permissions.ProjectCreate] = "PROJECT_CREATE",
        [Permissions.ProjectRead] = "PROJECT_READ",
        [Permissions.ProjectView] = "PROJECT_VIEW",
        [Permissions.ProjectWrite] = "PROJECT_WRITE",
        [Permissions.RepoAdmin] = "REPO_ADMIN",
        [Permissions.RepoRead] = "REPO_READ",
        [Permissions.RepoWrite] = "REPO_WRITE",
        [Permissions.SysAdmin] = "SYS_ADMIN",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, Permissions> s_permissionByString =
        s_stringByPermissions.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="Permissions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="permission">The permission to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string PermissionToString(Permissions permission)
    {
        if (!s_stringByPermissions.TryGetValue(permission, out string? result))
        {
            throw new ArgumentException($"Unknown permission: {permission}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="Permissions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="permission">The permission to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? PermissionToString(Permissions? permission) => permission.HasValue
        ? PermissionToString(permission.Value)
        : null;

    /// <summary>
    /// Parses a Bitbucket permission string into a <see cref="Permissions"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed permission.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static Permissions StringToPermission(string s)
    {
        if (!s_permissionByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown permission: {s}");
        }

        return result;
    }

    #endregion

    #region MergeCommits

    private static readonly FrozenDictionary<MergeCommits, string> s_stringByMergeCommits = new Dictionary<MergeCommits, string>
    {
        [MergeCommits.Exclude] = "exclude",
        [MergeCommits.Include] = "include",
        [MergeCommits.Only] = "only",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="MergeCommits"/> preference to the Bitbucket API string.
    /// </summary>
    /// <param name="mergeCommits">The merge commit option.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string MergeCommitsToString(MergeCommits mergeCommits)
    {
        if (!s_stringByMergeCommits.TryGetValue(mergeCommits, out string? result))
        {
            throw new ArgumentException($"Unknown merge commit: {mergeCommits}");
        }

        return result;
    }

    #endregion

    #region Roles

    private static readonly FrozenDictionary<Roles, string> s_stringByRoles = new Dictionary<Roles, string>
    {
        [Roles.Author] = "AUTHOR",
        [Roles.Reviewer] = "REVIEWER",
        [Roles.Participant] = "PARTICIPANT",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, Roles> s_roleByString =
        s_stringByRoles.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a pull request <see cref="Roles"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="role">The role to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string RoleToString(Roles role)
    {
        if (!s_stringByRoles.TryGetValue(role, out string? result))
        {
            throw new ArgumentException($"Unknown role: {role}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional pull request <see cref="Roles"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="role">The role to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RoleToString(Roles? role) => role.HasValue
        ? RoleToString(role.Value)
        : null;

    /// <summary>
    /// Parses a pull request role string into a <see cref="Roles"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed role.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static Roles StringToRole(string s)
    {
        if (!s_roleByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown role: {s}");
        }

        return result;
    }

    #endregion

    #region LineTypes

    private static readonly FrozenDictionary<LineTypes, string> s_stringByLineTypes = new Dictionary<LineTypes, string>
    {
        [LineTypes.Added] = "ADDED",
        [LineTypes.Removed] = "REMOVED",
        [LineTypes.Context] = "CONTEXT",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, LineTypes> s_lineTypeByString =
        s_stringByLineTypes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="LineTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="lineType">The line type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string LineTypeToString(LineTypes lineType)
    {
        if (!s_stringByLineTypes.TryGetValue(lineType, out string? result))
        {
            throw new ArgumentException($"Unknown line type: {lineType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="LineTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="lineType">The line type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? LineTypeToString(LineTypes? lineType)
    {
        return lineType.HasValue
            ? LineTypeToString(lineType.Value)
            : null;
    }

    /// <summary>
    /// Parses a line type string into a <see cref="LineTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed line type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static LineTypes StringToLineType(string s)
    {
        if (!s_lineTypeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown line type: {s}");
        }

        return result;
    }

    #endregion

    #region FileTypes

    private static readonly FrozenDictionary<FileTypes, string> s_stringByFileTypes = new Dictionary<FileTypes, string>
    {
        [FileTypes.From] = "FROM",
        [FileTypes.To] = "TO",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, FileTypes> s_fileTypeByString =
        s_stringByFileTypes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="FileTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fileType">The file type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string FileTypeToString(FileTypes fileType)
    {
        if (!s_stringByFileTypes.TryGetValue(fileType, out string? result))
        {
            throw new ArgumentException($"Unknown file type: {fileType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="FileTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="fileType">The file type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? FileTypeToString(FileTypes? fileType)
    {
        return fileType.HasValue
            ? FileTypeToString(fileType.Value)
            : null;
    }

    /// <summary>
    /// Parses a file type string into a <see cref="FileTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed file type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static FileTypes StringToFileType(string s)
    {
        if (!s_fileTypeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown file type: {s}");
        }

        return result;
    }

    #endregion

    #region ChangeScopes

    private static readonly FrozenDictionary<ChangeScopes, string> s_stringByChangeScopes = new Dictionary<ChangeScopes, string>
    {
        [ChangeScopes.All] = "ALL",
        [ChangeScopes.Unreviewed] = "UNREVIEWED",
        [ChangeScopes.Range] = "RANGE",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="ChangeScopes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="changeScope">The change scope to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ChangeScopeToString(ChangeScopes changeScope)
    {
        if (!s_stringByChangeScopes.TryGetValue(changeScope, out string? result))
        {
            throw new ArgumentException($"Unknown change scope: {changeScope}");
        }

        return result;
    }

    #endregion

    #region LogLevels

    private static readonly FrozenDictionary<LogLevels, string> s_stringByLogLevels = new Dictionary<LogLevels, string>
    {
        [LogLevels.Trace] = "TRACE",
        [LogLevels.Debug] = "DEBUG",
        [LogLevels.Info] = "INFO",
        [LogLevels.Warn] = "WARN",
        [LogLevels.Error] = "ERROR",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, LogLevels> s_logLevelByString =
        s_stringByLogLevels.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="LogLevels"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="logLevel">The log level to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string LogLevelToString(LogLevels logLevel)
    {
        if (!s_stringByLogLevels.TryGetValue(logLevel, out string? result))
        {
            throw new ArgumentException($"Unknown log level: {logLevel}");
        }

        return result;
    }

    /// <summary>
    /// Parses a log level string into a <see cref="LogLevels"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed log level.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static LogLevels StringToLogLevel(string s)
    {
        if (!s_logLevelByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown log level: {s}");
        }

        return result;
    }

    #endregion

    #region ParticipantStatus

    private static readonly FrozenDictionary<ParticipantStatus, string> s_stringByParticipantStatus = new Dictionary<ParticipantStatus, string>
    {
        [ParticipantStatus.Approved] = "APPROVED",
        [ParticipantStatus.NeedsWork] = "NEEDS_WORK",
        [ParticipantStatus.Unapproved] = "UNAPPROVED",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, ParticipantStatus> s_participantStatusByString =
        s_stringByParticipantStatus.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="ParticipantStatus"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="participantStatus">The status to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ParticipantStatusToString(ParticipantStatus participantStatus)
    {
        if (!s_stringByParticipantStatus.TryGetValue(participantStatus, out string? result))
        {
            throw new ArgumentException($"Unknown participant status: {participantStatus}");
        }

        return result;
    }

    /// <summary>
    /// Parses a participant status string into a <see cref="ParticipantStatus"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed status.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static ParticipantStatus StringToParticipantStatus(string s)
    {
        if (!s_participantStatusByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown participant status: {s}");
        }

        return result;
    }

    #endregion

    #region HookTypes

    private static readonly FrozenDictionary<HookTypes, string> s_stringByHookTypes = new Dictionary<HookTypes, string>
    {
        [HookTypes.PreReceive] = "PRE_RECEIVE",
        [HookTypes.PostReceive] = "POST_RECEIVE",
        [HookTypes.PrePullRequestMerge] = "PRE_PULL_REQUEST_MERGE",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, HookTypes> s_hookTypeByString =
        s_stringByHookTypes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a hook <see cref="HookTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="hookType">The hook type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string HookTypeToString(HookTypes hookType)
    {
        if (!s_stringByHookTypes.TryGetValue(hookType, out string? result))
        {
            throw new ArgumentException($"Unknown hook type: {hookType}");
        }

        return result;
    }

    /// <summary>
    /// Parses a hook type string into a <see cref="HookTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed hook type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static HookTypes StringToHookType(string s)
    {
        if (!s_hookTypeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown hook type: {s}");
        }

        return result;
    }

    #endregion

    #region ScopeTypes

    private static readonly FrozenDictionary<ScopeTypes, string> s_stringByScopeTypes = new Dictionary<ScopeTypes, string>
    {
        [ScopeTypes.Global] = "GLOBAL",
        [ScopeTypes.Project] = "PROJECT",
        [ScopeTypes.Repository] = "REPOSITORY",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, ScopeTypes> s_scopeTypeByString =
        s_stringByScopeTypes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="ScopeTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="scopeType">The scope type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ScopeTypeToString(ScopeTypes scopeType)
    {
        if (!s_stringByScopeTypes.TryGetValue(scopeType, out string? result))
        {
            throw new ArgumentException($"Unknown scope type: {scopeType}");
        }

        return result;
    }

    /// <summary>
    /// Parses a scope type string into a <see cref="ScopeTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed scope type.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static ScopeTypes StringToScopeType(string s)
    {
        if (!s_scopeTypeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown scope type: {s}");
        }

        return result;
    }

    #endregion

    #region ArchiveFormats

    private static readonly FrozenDictionary<ArchiveFormats, string> s_stringByArchiveFormats = new Dictionary<ArchiveFormats, string>
    {
        [ArchiveFormats.Zip] = "zip",
        [ArchiveFormats.Tar] = "tar",
        [ArchiveFormats.TarGz] = "tar.gz",
        [ArchiveFormats.Tgz] = "tgz",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts an <see cref="ArchiveFormats"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="archiveFormat">The archive format to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string ArchiveFormatToString(ArchiveFormats archiveFormat)
    {
        if (!s_stringByArchiveFormats.TryGetValue(archiveFormat, out string? result))
        {
            throw new ArgumentException($"Unknown archive format: {archiveFormat}");
        }

        return result;
    }

    #endregion

    #region WebHookOutcomes

    private static readonly FrozenDictionary<WebHookOutcomes, string> s_stringByWebHookOutcomes = new Dictionary<WebHookOutcomes, string>
    {
        [WebHookOutcomes.Success] = "SUCCESS",
        [WebHookOutcomes.Failure] = "FAILURE",
        [WebHookOutcomes.Error] = "ERROR",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, WebHookOutcomes> s_webHookOutcomeByString =
        s_stringByWebHookOutcomes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="WebHookOutcomes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="webHookOutcome">The outcome to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string WebHookOutcomeToString(WebHookOutcomes webHookOutcome)
    {
        if (!s_stringByWebHookOutcomes.TryGetValue(webHookOutcome, out string? result))
        {
            throw new ArgumentException($"Unknown web hook outcome: {webHookOutcome}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="WebHookOutcomes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="webHookOutcome">The outcome to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? WebHookOutcomeToString(WebHookOutcomes? webHookOutcome) => webHookOutcome.HasValue
        ? WebHookOutcomeToString(webHookOutcome.Value)
        : null;

    /// <summary>
    /// Parses a webhook outcome string into a <see cref="WebHookOutcomes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed outcome.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static WebHookOutcomes StringToWebHookOutcome(string s)
    {
        if (!s_webHookOutcomeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown web hook outcome: {s}");
        }

        return result;
    }

    #endregion

    #region AnchorStates

    private static readonly FrozenDictionary<AnchorStates, string> s_stringByAnchorStates = new Dictionary<AnchorStates, string>
    {
        [AnchorStates.Active] = "ACTIVE",
        [AnchorStates.Orphaned] = "ORPHANED",
        [AnchorStates.All] = "ALL",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts an <see cref="AnchorStates"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="anchorState">The anchor state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string AnchorStateToString(AnchorStates anchorState)
    {
        if (!s_stringByAnchorStates.TryGetValue(anchorState, out string? result))
        {
            throw new ArgumentException($"Unknown anchor state: {anchorState}");
        }

        return result;
    }

    #endregion

    #region DiffTypes

    private static readonly FrozenDictionary<DiffTypes, string> s_stringByDiffTypes = new Dictionary<DiffTypes, string>
    {
        [DiffTypes.Effective] = "EFFECTIVE",
        [DiffTypes.Range] = "RANGE",
        [DiffTypes.Commit] = "COMMIT",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="DiffTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="diffType">The diff type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string DiffTypeToString(DiffTypes diffType)
    {
        if (!s_stringByDiffTypes.TryGetValue(diffType, out string? result))
        {
            throw new ArgumentException($"Unknown diff type: {diffType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="DiffTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="diffType">The diff type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? DiffTypeToString(DiffTypes? diffType)
    {
        return diffType.HasValue
            ? DiffTypeToString(diffType.Value)
            : null;
    }

    #endregion

    #region TagTypes

    private static readonly FrozenDictionary<TagTypes, string> s_stringByTagTypes = new Dictionary<TagTypes, string>
    {
        [TagTypes.LightWeight] = "LIGHTWEIGHT",
        [TagTypes.Annotated] = "ANNOTATED",
    }.ToFrozenDictionary();

    /// <summary>
    /// Converts a <see cref="TagTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="tagType">The tag type to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string TagTypeToString(TagTypes tagType)
    {
        if (!s_stringByTagTypes.TryGetValue(tagType, out string? result))
        {
            throw new ArgumentException($"Unknown tag type: {tagType}");
        }

        return result;
    }

    #endregion

    #region RefRestrictionTypes

    private static readonly FrozenDictionary<RefRestrictionTypes, string> s_stringByRefRestrictionTypes = new Dictionary<RefRestrictionTypes, string>
    {
        [RefRestrictionTypes.AllChanges] = "read-only",
        [RefRestrictionTypes.RewritingHistory] = "fast-forward-only",
        [RefRestrictionTypes.Deletion] = "no-deletes",
        [RefRestrictionTypes.ChangesWithoutPullRequest] = "pull-request-only",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, RefRestrictionTypes> s_refRestrictionTypeByString =
        s_stringByRefRestrictionTypes.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="RefRestrictionTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refRestrictionType">The restriction to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string RefRestrictionTypeToString(RefRestrictionTypes refRestrictionType)
    {
        if (!s_stringByRefRestrictionTypes.TryGetValue(refRestrictionType, out string? result))
        {
            throw new ArgumentException($"Unknown ref restriction type: {refRestrictionType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="RefRestrictionTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refRestrictionType">The restriction to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RefRestrictionTypeToString(RefRestrictionTypes? refRestrictionType)
    {
        return refRestrictionType.HasValue
            ? RefRestrictionTypeToString(refRestrictionType.Value)
            : null;
    }

    /// <summary>
    /// Parses a ref restriction string into a <see cref="RefRestrictionTypes"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed restriction.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static RefRestrictionTypes StringToRefRestrictionType(string s)
    {
        if (!s_refRestrictionTypeByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown ref restriction type: {s}");
        }

        return result;
    }

    #endregion

    #region RefMatcherTypes

    private static readonly FrozenDictionary<RefMatcherTypes, string> s_stringByRefMatcherTypes = new Dictionary<RefMatcherTypes, string>
    {
        [RefMatcherTypes.Branch] = "BRANCH",
        [RefMatcherTypes.Pattern] = "PATTERN",
        [RefMatcherTypes.ModelCategory] = "MODEL_CATEGORY",
        [RefMatcherTypes.ModelBranch] = "MODEL_BRANCH",
    }.ToFrozenDictionary();

    private static string RefMatcherTypeToString(RefMatcherTypes refMatcherType)
    {
        if (!s_stringByRefMatcherTypes.TryGetValue(refMatcherType, out string? result))
        {
            throw new ArgumentException($"Unknown ref matcher type: {refMatcherType}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="RefMatcherTypes"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="refMatcherType">The matcher type to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? RefMatcherTypeToString(RefMatcherTypes? refMatcherType)
    {
        return refMatcherType.HasValue
            ? RefMatcherTypeToString(refMatcherType.Value)
            : null;
    }

    #endregion

    #region SynchronizeActions

    private static readonly FrozenDictionary<SynchronizeActions, string> s_stringBySynchronizeActions = new Dictionary<SynchronizeActions, string>
    {
        [SynchronizeActions.Merge] = "MERGE",
        [SynchronizeActions.Discard] = "DISCARD",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, SynchronizeActions> s_synchronizeActionByString =
        s_stringBySynchronizeActions.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="SynchronizeActions"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="synchronizeAction">The synchronization action to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string SynchronizeActionToString(SynchronizeActions synchronizeAction)
    {
        if (!s_stringBySynchronizeActions.TryGetValue(synchronizeAction, out string? result))
        {
            throw new ArgumentException($"Unknown synchronize action: {synchronizeAction}");
        }

        return result;
    }

    /// <summary>
    /// Parses a synchronization action string into a <see cref="SynchronizeActions"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed action.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static SynchronizeActions StringToSynchronizeAction(string s)
    {
        if (!s_synchronizeActionByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown synchronize action: {s}");
        }

        return result;
    }

    #endregion

    #region BlockerCommentState

    private static readonly FrozenDictionary<BlockerCommentState, string> s_stringByBlockerCommentState = new Dictionary<BlockerCommentState, string>
    {
        [BlockerCommentState.Open] = "OPEN",
        [BlockerCommentState.Resolved] = "RESOLVED",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, BlockerCommentState> s_blockerCommentStateByString =
        s_stringByBlockerCommentState.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="BlockerCommentState"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The blocker comment state to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string BlockerCommentStateToString(BlockerCommentState state)
    {
        if (!s_stringByBlockerCommentState.TryGetValue(state, out string? result))
        {
            throw new ArgumentException($"Unknown blocker comment state: {state}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="BlockerCommentState"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="state">The blocker comment state to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? BlockerCommentStateToString(BlockerCommentState? state) => state.HasValue
        ? BlockerCommentStateToString(state.Value)
        : null;

    /// <summary>
    /// Parses a blocker comment state string into a <see cref="BlockerCommentState"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed state.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static BlockerCommentState StringToBlockerCommentState(string s)
    {
        if (!s_blockerCommentStateByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown blocker comment state: {s}");
        }

        return result;
    }

    #endregion

    #region CommentSeverity

    private static readonly FrozenDictionary<CommentSeverity, string> s_stringByCommentSeverity = new Dictionary<CommentSeverity, string>
    {
        [CommentSeverity.Normal] = "NORMAL",
        [CommentSeverity.Blocker] = "BLOCKER",
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, CommentSeverity> s_commentSeverityByString =
        s_stringByCommentSeverity.ToFrozenDictionary(kv => kv.Value, kv => kv.Key, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Converts a <see cref="CommentSeverity"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="severity">The comment severity to convert.</param>
    /// <returns>The API string representation.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static string CommentSeverityToString(CommentSeverity severity)
    {
        if (!s_stringByCommentSeverity.TryGetValue(severity, out string? result))
        {
            throw new ArgumentException($"Unknown comment severity: {severity}");
        }

        return result;
    }

    /// <summary>
    /// Converts an optional <see cref="CommentSeverity"/> value to the Bitbucket API string.
    /// </summary>
    /// <param name="severity">The comment severity to convert.</param>
    /// <returns>The API string representation or <see langword="null"/> when not supplied.</returns>
    public static string? CommentSeverityToString(CommentSeverity? severity) => severity.HasValue
        ? CommentSeverityToString(severity.Value)
        : null;

    /// <summary>
    /// Parses a comment severity string into a <see cref="CommentSeverity"/> value.
    /// </summary>
    /// <param name="s">The string returned by the API.</param>
    /// <returns>The parsed severity.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is not recognized.</exception>
    public static CommentSeverity StringToCommentSeverity(string s)
    {
        if (!s_commentSeverityByString.TryGetValue(s, out var result))
        {
            throw new ArgumentException($"Unknown comment severity: {s}");
        }

        return result;
    }

    #endregion
}