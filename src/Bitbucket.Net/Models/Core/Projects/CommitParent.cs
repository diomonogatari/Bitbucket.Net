namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Lightweight commit reference containing the full and abbreviated SHA.
/// </summary>
public class CommitParent
{
    /// <summary>
    /// Gets or sets the full commit SHA hash.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the abbreviated commit SHA shown in the Bitbucket UI.
    /// </summary>
    public string? DisplayId { get; set; }
}