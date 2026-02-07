namespace Bitbucket.Net.Common.Models;

/// <summary>
/// Represents the error response returned by the Bitbucket Server API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the collection of errors returned by the server.
    /// </summary>
    public IEnumerable<Error>? Errors { get; set; }
}