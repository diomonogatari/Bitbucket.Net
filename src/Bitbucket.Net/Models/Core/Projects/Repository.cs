namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Full Bitbucket repository. Extends <see cref="RepositoryRef"/> with server-assigned identity and metadata.
/// </summary>
public class Repository : RepositoryRef
{
    /// <summary>
    /// Gets or sets the server-assigned repository identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the SCM type identifier (e.g. "git").
    /// </summary>
    public string? ScmId { get; set; }

    /// <summary>
    /// Gets or sets the repository state (e.g. "AVAILABLE").
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets a human-readable status message for the repository.
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the repository can be forked.
    /// </summary>
    public bool Forkable { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the repository is publicly accessible.
    /// </summary>
    public bool Public { get; set; }

    /// <summary>
    /// Gets or sets the clone URLs and other hypermedia links for this repository.
    /// </summary>
    public CloneLinks? Links { get; set; }

    public override string ToString() => Name ?? string.Empty;
}