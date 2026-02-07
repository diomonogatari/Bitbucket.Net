using Bitbucket.Net.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when the request is malformed (HTTP 400 Bad Request).
/// This indicates invalid parameters, malformed JSON, or other request-level issues.
/// </summary>
public class BitbucketBadRequestException : BitbucketApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketBadRequestException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketBadRequestException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message, HttpStatusCode.BadRequest, errors, requestUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketBadRequestException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketBadRequestException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, HttpStatusCode.BadRequest, errors, innerException, requestUrl)
    {
    }
}