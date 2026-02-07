using Bitbucket.Net.Common.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when there is a resource conflict (HTTP 409 Conflict).
/// This typically indicates a merge conflict, duplicate resource, or state conflict.
/// </summary>
public class BitbucketConflictException : BitbucketApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketConflictException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketConflictException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message, HttpStatusCode.Conflict, errors, requestUrl)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketConflictException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketConflictException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, HttpStatusCode.Conflict, errors, innerException, requestUrl)
    {
    }
}