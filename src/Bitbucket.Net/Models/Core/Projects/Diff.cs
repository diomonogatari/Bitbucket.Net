namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A file-level diff. Extends <see cref="DiffInfo"/> with source/destination paths and hunks.
/// </summary>
public class Diff : DiffInfo
{
    /// <summary>
    /// Gets or sets the source (before) file path.
    /// </summary>
    public Path? Source { get; set; }

    /// <summary>
    /// Gets or sets the destination (after) file path.
    /// </summary>
    public Path? Destination { get; set; }

    /// <summary>
    /// Gets or sets the list of diff hunks containing the actual line changes.
    /// </summary>
    public List<DiffHunk>? Hunks { get; set; }
}