namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Lightweight reference to a Bitbucket repository, identified by slug and parent project.
/// </summary>
public class RepositoryRef
{
    /// <summary>
    /// Gets or sets the URL-friendly repository identifier.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Gets or sets the repository display name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the parent project reference.
    /// </summary>
    public ProjectRef? Project { get; set; }
}