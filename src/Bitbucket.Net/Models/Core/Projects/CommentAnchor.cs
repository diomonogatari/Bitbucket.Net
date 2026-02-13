namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Anchor location for an inline comment on a specific file and line in a pull request diff.
/// </summary>
public class CommentAnchor
{
    /// <summary>
    /// Gets or sets the line number the comment is anchored to.
    /// </summary>
    public int? Line { get; set; }

    /// <summary>
    /// Gets or sets the line type (e.g. ADDED, REMOVED, CONTEXT).
    /// </summary>
    public LineTypes LineType { get; set; }

    /// <summary>
    /// Gets or sets the file type (e.g. FROM for source, TO for destination).
    /// </summary>
    public FileTypes FileType { get; set; }

    /// <summary>
    /// Gets or sets the commit hash of the source side of the diff.
    /// </summary>
    public string? FromHash { get; set; }

    /// <summary>
    /// Gets or sets the commit hash of the destination side of the diff.
    /// </summary>
    public string? ToHash { get; set; }

    /// <summary>
    /// Gets or sets the file path the comment is anchored to.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the original file path before a rename or move.
    /// </summary>
    public string? SrcPath { get; set; }
}