namespace Bitbucket.Net.Models.Core.Users;

/// <summary>
/// Full Bitbucket user. Extends <see cref="Identity"/> with server-assigned identity, display name, and account state.
/// </summary>
public class User : Identity
{
    /// <summary>
    /// Gets or sets the server-assigned user identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the user account is active.
    /// </summary>
    public bool Active { get; init; }

    /// <summary>
    /// Gets or sets the URL-friendly user identifier.
    /// </summary>
    public string? Slug { get; init; }

    /// <summary>
    /// Gets or sets the user type (e.g. "NORMAL" or "SERVICE").
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets or sets the URL of the user's avatar image.
    /// </summary>
    public string? AvatarUrl { get; init; }

    /// <summary>
    /// Returns the user's display name when available.
    /// </summary>
    public override string ToString() => DisplayName ?? string.Empty;
}