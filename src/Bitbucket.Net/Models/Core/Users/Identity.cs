namespace Bitbucket.Net.Models.Core.Users;

/// <summary>
/// Extends <see cref="Named"/> with an email address.
/// </summary>
public class Identity : Named
{
    /// <summary>
    /// Gets or sets the email address associated with the identity.
    /// </summary>
    public string? EmailAddress { get; set; }
}