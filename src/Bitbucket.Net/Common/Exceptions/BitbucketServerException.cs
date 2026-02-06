using System;
using System.Collections.Generic;
using System.Net;
using Bitbucket.Net.Common.Models;

namespace Bitbucket.Net.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a server error occurs (HTTP 5xx).
    /// This indicates an internal server error on the Bitbucket Server side.
    /// </summary>
    public class BitbucketServerException : BitbucketApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketServerException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code (must be 5xx).</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketServerException(string message, HttpStatusCode statusCode, IReadOnlyList<Error> errors, string? requestUrl = null)
            : base(message, statusCode, errors, requestUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketServerException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code (must be 5xx).</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketServerException(string message, HttpStatusCode statusCode, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
            : base(message, statusCode, errors, innerException, requestUrl)
        {
        }
    }
}
