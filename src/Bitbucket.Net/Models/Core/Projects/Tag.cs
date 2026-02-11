namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A Git tag in a Bitbucket repository.
/// </summary>
public class Tag
{
    /// <summary>
    /// Gets or sets the full tag ref path (e.g. "refs/tags/v1.0").
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets or sets the short tag name shown in the Bitbucket UI.
    /// </summary>
    public string? DisplayId { get; init; }

    /// <summary>
    /// Gets or sets the ref type (typically "TAG").
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the SHA of the latest commit this tag points to.
    /// </summary>
    public string? LatestCommit { get; init; }

    /// <summary>
    /// Gets or sets the changeset identifier of the latest change.
    /// </summary>
    public string? LatestChangeset { get; init; }

    /// <summary>
    /// Gets or sets the tag object hash (for annotated tags).
    /// </summary>
    public string? Hash { get; init; }
}