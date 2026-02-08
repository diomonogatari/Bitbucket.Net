namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Lightweight reference to a Bitbucket project, identified by its key.
/// </summary>
public class ProjectRef
{
    /// <summary>
    /// Gets or sets the unique project key (e.g. "PRJ").
    /// </summary>
    public string? Key { get; set; }
}