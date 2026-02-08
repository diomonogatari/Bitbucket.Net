namespace Bitbucket.Net.Models.Core.Users;

/// <summary>
/// Base class for Bitbucket entities that have a name.
/// </summary>
public class Named
{
    /// <summary>
    /// Gets or sets the entity name (typically the username for users).
    /// </summary>
    public string? Name { get; set; }

    public override string ToString() => Name ?? string.Empty;
}