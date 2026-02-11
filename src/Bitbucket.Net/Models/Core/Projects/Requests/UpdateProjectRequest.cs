namespace Bitbucket.Net.Models.Core.Projects.Requests;

/// <summary>
/// Request body for updating an existing Bitbucket project.
/// </summary>
public sealed class UpdateProjectRequest
{
    /// <summary>
    /// The project key. Optional — when omitted the key from the URL path is used.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// The new human-readable project name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// The new project description.
    /// </summary>
    public string? Description { get; init; }
}