using Bitbucket.Net.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails (HTTP 422 Unprocessable Entity).
/// This indicates the request was well-formed but contained semantic errors.
/// </summary>
public class BitbucketValidationException : BitbucketApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketValidationException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketValidationException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message, HttpStatusCode.UnprocessableEntity, errors, requestUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketValidationException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketValidationException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, HttpStatusCode.UnprocessableEntity, errors, innerException, requestUrl)
    {
    }
}