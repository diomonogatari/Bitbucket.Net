namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Full Bitbucket project. Extends <see cref="ProjectDefinition"/> with server-assigned identity and metadata.
/// </summary>
public class Project : ProjectDefinition
{
    /// <summary>
    /// Gets or sets the server-assigned project identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the project is publicly accessible.
    /// </summary>
    public bool Public { get; set; }

    /// <summary>
    /// Gets or sets the project type (e.g. "NORMAL" or "PERSONAL").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the hypermedia links for this project.
    /// </summary>
    public Links? Links { get; set; }

    public override string ToString() => Name ?? string.Empty;
}