namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for creating a new Bitbucket project.
/// </summary>
public sealed class CreateProjectRequest
{
    /// <summary>
    /// The unique project key (e.g. "PRJ"). Required.
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// The human-readable project name. Required.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// An optional project description.
    /// </summary>
    public string? Description { get; init; }
}