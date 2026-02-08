namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// A single hyperlink in the Bitbucket REST API response.
/// </summary>
public class Link
{
    /// <summary>
    /// Gets or sets the URL of the link.
    /// </summary>
    public string? Href { get; set; }

    public override string ToString() => Href ?? string.Empty;
}