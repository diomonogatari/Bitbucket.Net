namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A single line of file content returned when browsing a repository path.
/// </summary>
public class Line
{
    /// <summary>
    /// The text of the line.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// <see langword="true"/> when Bitbucket truncated this line because it exceeded the server's
    /// maximum line length; otherwise <see langword="false"/> or <see langword="null"/> when the
    /// server does not report truncation.
    /// </summary>
    public bool? Truncated { get; init; }
}