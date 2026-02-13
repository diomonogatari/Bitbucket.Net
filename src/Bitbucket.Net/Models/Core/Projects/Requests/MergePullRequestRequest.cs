namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for merging a pull request. All fields are optional;
/// when omitted, the server uses default merge behaviour.
/// </summary>
public sealed class MergePullRequestRequest
{
    /// <summary>
    /// The expected current version of the pull request for optimistic locking.
    /// When set to -1 (the default), the version check is skipped.
    /// </summary>
    public int Version { get; init; } = -1;

    /// <summary>
    /// An optional custom merge commit message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// An optional merge strategy override (e.g. "squash", "no-ff").
    /// </summary>
    public string? Strategy { get; init; }
}