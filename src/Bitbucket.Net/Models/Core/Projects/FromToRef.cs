namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Represents a reference (branch/tag) in a pull request's source or target.
/// </summary>
public class FromToRef
{
    /// <summary>
    /// The full ref ID (e.g., "refs/heads/feature-branch").
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// The display-friendly ref ID (e.g., "feature-branch").
    /// </summary>
    public string? DisplayId { get; set; }

    /// <summary>
    /// The SHA of the latest commit on this ref.
    /// This is useful for creating line-specific comments on pull requests.
    /// </summary>
    public string? LatestCommit { get; set; }

    /// <summary>
    /// The type of ref (e.g., "BRANCH", "TAG").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// The repository containing this ref.
    /// </summary>
    public RepositoryRef? Repository { get; set; }
}