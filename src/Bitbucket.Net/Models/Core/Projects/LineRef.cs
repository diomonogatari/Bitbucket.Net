namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A single line in a diff segment with source and destination line numbers.
/// </summary>
public class LineRef
{
    /// <summary>
    /// Gets or sets the line number in the destination (after) file.
    /// </summary>
    public int Destination { get; set; }

    /// <summary>
    /// Gets or sets the line number in the source (before) file.
    /// </summary>
    public int Source { get; set; }

    /// <summary>
    /// Gets or sets the text content of the line.
    /// </summary>
    public string? Line { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this line was truncated by the server.
    /// </summary>
    public bool Truncated { get; set; }
}