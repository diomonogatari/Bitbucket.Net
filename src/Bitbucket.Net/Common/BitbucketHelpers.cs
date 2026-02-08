using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Logs;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Git;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.RefSync;

namespace Bitbucket.Net.Common;

/// <summary>
/// Helper methods for converting between Bitbucket enum values and their wire-format string representations.
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

    private static readonly Dictionary<BranchOrderBy, string> s_stringByBranchOrderBy = new()
    {
        [BranchOrderBy.Alphabetical] = "ALPHABETICAL",
        [BranchOrderBy.Modification] = "MODIFICATION",
    };

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

    private static readonly Dictionary<PullRequestDirections, string> s_stringByPullRequestDirection = new()
    {
        [PullRequestDirections.Incoming] = "INCOMING",
        [PullRequestDirections.Outgoing] = "OUTGOING",
    };

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

    private static readonly Dictionary<PullRequestStates, string> s_stringByPullRequestState = new()
    {
        [PullRequestStates.Open] = "OPEN",
        [PullRequestStates.Declined] = "DECLINED",
        [PullRequestStates.Merged] = "MERGED",
        [PullRequestStates.All] = "ALL",
    };

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
        var pair = s_stringByPullRequestState.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<PullRequestStates, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown pull request state: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region PullRequestOrders

    private static readonly Dictionary<PullRequestOrders, string> s_stringByPullRequestOrder = new()
    {
        [PullRequestOrders.Newest] = "NEWEST",
        [PullRequestOrders.Oldest] = "OLDEST",
    };

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

    private static readonly Dictionary<PullRequestFromTypes, string> s_stringByPullRequestFromType = new()
    {
        [PullRequestFromTypes.Comment] = "COMMENT",
        [PullRequestFromTypes.Activity] = "ACTIVITY",
    };

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

    private static readonly Dictionary<Permissions, string> s_stringByPermissions = new()
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
    };

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
        var pair = s_stringByPermissions.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<Permissions, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown permission: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region MergeCommits

    private static readonly Dictionary<MergeCommits, string> s_stringByMergeCommits = new()
    {
        [MergeCommits.Exclude] = "exclude",
        [MergeCommits.Include] = "include",
        [MergeCommits.Only] = "only",
    };

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

    private static readonly Dictionary<Roles, string> s_stringByRoles = new()
    {
        [Roles.Author] = "AUTHOR",
        [Roles.Reviewer] = "REVIEWER",
        [Roles.Participant] = "PARTICIPANT",
    };

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
        var pair = s_stringByRoles.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<Roles, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown role: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region LineTypes

    private static readonly Dictionary<LineTypes, string> s_stringByLineTypes = new()
    {
        [LineTypes.Added] = "ADDED",
        [LineTypes.Removed] = "REMOVED",
        [LineTypes.Context] = "CONTEXT",
    };

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
        var pair = s_stringByLineTypes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<LineTypes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown line type: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region FileTypes

    private static readonly Dictionary<FileTypes, string> s_stringByFileTypes = new()
    {
        [FileTypes.From] = "FROM",
        [FileTypes.To] = "TO",
    };

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
        var pair = s_stringByFileTypes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<FileTypes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown file type: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region ChangeScopes

    private static readonly Dictionary<ChangeScopes, string> s_stringByChangeScopes = new()
    {
        [ChangeScopes.All] = "ALL",
        [ChangeScopes.Unreviewed] = "UNREVIEWED",
        [ChangeScopes.Range] = "RANGE",
    };

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

    private static readonly Dictionary<LogLevels, string> s_stringByLogLevels = new()
    {
        [LogLevels.Trace] = "TRACE",
        [LogLevels.Debug] = "DEBUG",
        [LogLevels.Info] = "INFO",
        [LogLevels.Warn] = "WARN",
        [LogLevels.Error] = "ERROR",
    };

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
        var pair = s_stringByLogLevels.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<LogLevels, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown log level: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region ParticipantStatus

    private static readonly Dictionary<ParticipantStatus, string> s_stringByParticipantStatus = new()
    {
        [ParticipantStatus.Approved] = "APPROVED",
        [ParticipantStatus.NeedsWork] = "NEEDS_WORK",
        [ParticipantStatus.Unapproved] = "UNAPPROVED",
    };

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
        var pair = s_stringByParticipantStatus.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<ParticipantStatus, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown participant status: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region HookTypes

    private static readonly Dictionary<HookTypes, string> s_stringByHookTypes = new()
    {
        [HookTypes.PreReceive] = "PRE_RECEIVE",
        [HookTypes.PostReceive] = "POST_RECEIVE",
        [HookTypes.PrePullRequestMerge] = "PRE_PULL_REQUEST_MERGE",
    };

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
        var pair = s_stringByHookTypes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<HookTypes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown hook type: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region ScopeTypes

    private static readonly Dictionary<ScopeTypes, string> s_stringByScopeTypes = new()
    {
        [ScopeTypes.Project] = "PROJECT",
        [ScopeTypes.Repository] = "REPOSITORY",
    };

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
        var pair = s_stringByScopeTypes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<ScopeTypes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown scope type: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region ArchiveFormats

    private static readonly Dictionary<ArchiveFormats, string> s_stringByArchiveFormats = new()
    {
        [ArchiveFormats.Zip] = "zip",
        [ArchiveFormats.Tar] = "tar",
        [ArchiveFormats.TarGz] = "tar.gz",
        [ArchiveFormats.Tgz] = "tgz",
    };

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

    private static readonly Dictionary<WebHookOutcomes, string> s_stringByWebHookOutcomes = new()
    {
        [WebHookOutcomes.Success] = "SUCCESS",
        [WebHookOutcomes.Failure] = "FAILURE",
        [WebHookOutcomes.Error] = "ERROR",
    };

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
        var pair = s_stringByWebHookOutcomes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<WebHookOutcomes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown web hook outcome: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region AnchorStates

    private static readonly Dictionary<AnchorStates, string> s_stringByAnchorStates = new()
    {
        [AnchorStates.Active] = "ACTIVE",
        [AnchorStates.Orphaned] = "ORPHANED",
        [AnchorStates.All] = "ALL",
    };

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

    private static readonly Dictionary<DiffTypes, string> s_stringByDiffTypes = new()
    {
        [DiffTypes.Effective] = "EFFECTIVE",
        [DiffTypes.Range] = "RANGE",
        [DiffTypes.Commit] = "COMMIT",
    };

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

    private static readonly Dictionary<TagTypes, string> s_stringByTagTypes = new()
    {
        [TagTypes.LightWeight] = "LIGHTWEIGHT",
        [TagTypes.Annotated] = "ANNOTATED",
    };

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

    private static readonly Dictionary<RefRestrictionTypes, string> s_stringByRefRestrictionTypes = new()
    {
        [RefRestrictionTypes.AllChanges] = "read-only",
        [RefRestrictionTypes.RewritingHistory] = "fast-forward-only",
        [RefRestrictionTypes.Deletion] = "no-deletes",
        [RefRestrictionTypes.ChangesWithoutPullRequest] = "pull-request-only",
    };

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
        var pair = s_stringByRefRestrictionTypes.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<RefRestrictionTypes, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown ref restriction type: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region RefMatcherTypes

    private static readonly Dictionary<RefMatcherTypes, string> s_stringByRefMatcherTypes = new()
    {
        [RefMatcherTypes.Branch] = "BRANCH",
        [RefMatcherTypes.Pattern] = "PATTERN",
        [RefMatcherTypes.ModelCategory] = "MODEL_CATEGORY",
        [RefMatcherTypes.ModelBranch] = "MODEL_BRANCH",
    };

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

    private static readonly Dictionary<SynchronizeActions, string> s_stringBySynchronizeActions = new()
    {
        [SynchronizeActions.Merge] = "MERGE",
        [SynchronizeActions.Discard] = "DISCARD",
    };

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
        var pair = s_stringBySynchronizeActions.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<SynchronizeActions, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown synchronize action: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region BlockerCommentState

    private static readonly Dictionary<BlockerCommentState, string> s_stringByBlockerCommentState = new()
    {
        [BlockerCommentState.Open] = "OPEN",
        [BlockerCommentState.Resolved] = "RESOLVED",
    };

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
        var pair = s_stringByBlockerCommentState.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<BlockerCommentState, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown blocker comment state: {s}");
        }

        return pair.Key;
    }

    #endregion

    #region CommentSeverity

    private static readonly Dictionary<CommentSeverity, string> s_stringByCommentSeverity = new()
    {
        [CommentSeverity.Normal] = "NORMAL",
        [CommentSeverity.Blocker] = "BLOCKER",
    };

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
        var pair = s_stringByCommentSeverity.FirstOrDefault(kvp => kvp.Value.Equals(s, StringComparison.OrdinalIgnoreCase));
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (EqualityComparer<KeyValuePair<CommentSeverity, string>>.Default.Equals(pair))
        {
            throw new ArgumentException($"Unknown comment severity: {s}");
        }

        return pair.Key;
    }

    #endregion
}