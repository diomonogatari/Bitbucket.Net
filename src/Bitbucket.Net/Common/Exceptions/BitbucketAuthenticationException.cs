using System;
using System.Collections.Generic;
using System.Net;
using Bitbucket.Net.Common.Models;

namespace Bitbucket.Net.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails (HTTP 401 Unauthorized).
    /// This typically indicates invalid or missing credentials.
    /// </summary>
    public class BitbucketAuthenticationException : BitbucketApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketAuthenticationException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketAuthenticationException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
            : base(message, HttpStatusCode.Unauthorized, errors, requestUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketAuthenticationException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketAuthenticationException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
            : base(message, HttpStatusCode.Unauthorized, errors, innerException, requestUrl)
        {
        }
    }
}
