namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Base class for Bitbucket entities identified by a string identifier.
/// </summary>
public class WithId
{
    /// <summary>
    /// Gets or sets the unique identifier (typically a Git ref path such as "refs/heads/main").
    /// </summary>
    public string? Id { get; set; }
}