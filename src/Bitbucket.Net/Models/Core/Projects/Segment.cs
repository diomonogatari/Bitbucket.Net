namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A segment within a diff hunk, grouping consecutive lines of the same type.
/// </summary>
public class Segment
{
    /// <summary>
    /// Gets or sets the segment type (e.g. "ADDED", "REMOVED", or "CONTEXT").
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the lines in this segment.
    /// </summary>
    public List<LineRef>? Lines { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this segment was truncated by the server.
    /// </summary>
    public bool Truncated { get; set; }
}