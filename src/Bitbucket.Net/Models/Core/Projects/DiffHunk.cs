namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A hunk within a file diff, describing a contiguous block of changes.
/// </summary>
public class DiffHunk
{
    /// <summary>
    /// Gets or sets the starting line number in the source (before) file.
    /// </summary>
    public int SourceLine { get; set; }

    /// <summary>
    /// Gets or sets the number of lines from the source file included in this hunk.
    /// </summary>
    public int SourceSpan { get; set; }

    /// <summary>
    /// Gets or sets the starting line number in the destination (after) file.
    /// </summary>
    public int DestinationLine { get; set; }

    /// <summary>
    /// Gets or sets the number of lines from the destination file included in this hunk.
    /// </summary>
    public int DestinationSpan { get; set; }

    /// <summary>
    /// Gets or sets the segments (groups of added, removed, or context lines) in this hunk.
    /// </summary>
    public List<Segment>? Segments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this hunk was truncated by the server.
    /// </summary>
    public bool Truncated { get; set; }
}