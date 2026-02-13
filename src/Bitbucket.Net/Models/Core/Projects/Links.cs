namespace Bitbucket.Net.Models.Core.Projects;

/// <summary>
/// Hypermedia links for a Bitbucket resource.
/// </summary>
public class Links
{
    /// <summary>
    /// Gets or sets the self-referencing links.
    /// </summary>
    public List<Link>? Self { get; init; }
}