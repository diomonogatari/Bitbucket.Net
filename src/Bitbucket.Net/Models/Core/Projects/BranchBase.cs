namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Base branch reference. Extends <see cref="WithId"/> with a display identifier and ref type.
/// </summary>
public class BranchBase : WithId
{
    /// <summary>
    /// Gets or sets the short display name of the branch (e.g. "main").
    /// </summary>
    public string? DisplayId { get; set; }

    /// <summary>
    /// Gets or sets the ref type (e.g. "BRANCH" or "TAG").
    /// </summary>
    public string? Type { get; set; }
}