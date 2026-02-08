namespace Bitbucket.Net.Common.Models;

/// <summary>
/// Represents an error returned by the Bitbucket Server API.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets or sets the context of the error (e.g., the field or resource that caused the error).
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the exception that occurred on the server, if available.
    /// </summary>
    public string? ExceptionName { get; set; }
}