namespace Bitbucket.Net.Models.Builds.Requests;

/// <summary>
/// Request body for associating a build status with a commit.
/// </summary>
public sealed class AssociateBuildStatusRequest
{
    /// <summary>
    /// The build state (e.g. "SUCCESSFUL", "FAILED", "INPROGRESS"). Required.
    /// </summary>
    public required string State { get; init; }

    /// <summary>
    /// A unique key identifying the build (e.g. build plan ID). Required.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The URL linking back to the build result in the CI system. Required.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// An optional human-readable build name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// An optional build description.
    /// </summary>
    public string? Description { get; init; }
}