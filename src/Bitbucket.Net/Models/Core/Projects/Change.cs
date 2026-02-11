namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Represents a file change in a Bitbucket commit or pull request.
/// </summary>
public class Change
{
    /// <summary>
    /// Gets or sets the content hash of the file after the change.
    /// </summary>
    public string? ContentId { get; init; }

    /// <summary>
    /// Gets or sets the content hash of the file before the change.
    /// </summary>
    public string? FromContentId { get; init; }

    /// <summary>
    /// Gets or sets the file path after the change.
    /// </summary>
    public Path? Path { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the file is executable after the change.
    /// </summary>
    public bool Executable { get; init; }

    /// <summary>
    /// Gets or sets the percentage of the file that is unchanged.
    /// </summary>
    public int PercentUnchanged { get; init; }

    /// <summary>
    /// Gets or sets the change type (e.g. "ADD", "MODIFY", "DELETE", "MOVE", "COPY").
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the node type (e.g. "FILE" or "DIRECTORY").
    /// </summary>
    public string? NodeType { get; init; }

    /// <summary>
    /// Gets or sets the original file path before a move or copy.
    /// </summary>
    public Path? SrcPath { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the file was executable before the change.
    /// </summary>
    public bool SrcExecutable { get; init; }

    /// <summary>
    /// Gets or sets the hypermedia links for this change.
    /// </summary>
    public Links? Links { get; init; }
}