using Bitbucket.Net.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when rate limiting is applied (HTTP 429 Too Many Requests).
/// This indicates too many requests have been made in a given time period.
/// </summary>
public class BitbucketRateLimitException : BitbucketApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketRateLimitException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketRateLimitException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message, HttpStatusCode.TooManyRequests, errors, requestUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketRateLimitException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketRateLimitException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, HttpStatusCode.TooManyRequests, errors, innerException, requestUrl)
    {
    }
}