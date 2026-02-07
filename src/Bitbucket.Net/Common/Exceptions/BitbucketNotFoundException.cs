using Bitbucket.Net.Common.Models;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404 Not Found).
/// This typically indicates the project, repository, branch, or other resource does not exist.
/// </summary>
public class BitbucketNotFoundException : BitbucketApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketNotFoundException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message, HttpStatusCode.NotFound, errors, requestUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketNotFoundException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketNotFoundException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, HttpStatusCode.NotFound, errors, innerException, requestUrl)
    {
    }
}