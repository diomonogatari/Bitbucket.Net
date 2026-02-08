namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Extends <see cref="ProjectRef"/> with a human-readable name and description.
/// </summary>
public class ProjectDefinition : ProjectRef
{
    /// <summary>
    /// Gets or sets the project display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the project description.
    /// </summary>
    public string? Description { get; set; }
}