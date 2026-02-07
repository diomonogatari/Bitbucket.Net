using System.Collections.Generic;

namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Represents a file path in a Bitbucket repository.
/// </summary>
public class Path
{
    /// <summary>
    /// The path components (directory and file name parts).
    /// </summary>
    public List<string>? Components { get; set; }

    /// <summary>
    /// The parent directory path.
    /// </summary>
    public string? Parent { get; set; }

    /// <summary>
    /// The file or directory name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The file extension (if any).
    /// </summary>
    public string? Extension { get; set; }

    /// <summary>
    /// The full path as a string, as returned by the Bitbucket API.
    /// Note: This property name is lowercase to match the JSON response.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public string? toString { get; set; }

    /// <summary>
    /// Returns the full path string representation.
    /// </summary>
    /// <returns>
    /// The path string from the API if available; otherwise,
    /// constructs the path from Components or falls back to Name.
    /// </returns>
    public override string ToString()
    {
        // Prefer the API-provided toString property
        if (!string.IsNullOrEmpty(toString))
            return toString;

        // Build from components if available
        if (Components is { Count: > 0 })
            return string.Join('/', Components);

        // Fallback to name, then type name (shouldn't happen in practice)
        return Name ?? "(unknown path)";
    }
}