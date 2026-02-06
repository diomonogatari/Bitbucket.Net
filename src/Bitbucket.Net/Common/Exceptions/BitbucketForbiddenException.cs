using System;
using System.Collections.Generic;
using System.Net;
using Bitbucket.Net.Common.Models;

namespace Bitbucket.Net.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when access is forbidden (HTTP 403 Forbidden).
    /// This indicates the user is authenticated but lacks permission for the requested operation.
    /// </summary>
    public class BitbucketForbiddenException : BitbucketApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketForbiddenException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketForbiddenException(string message, IReadOnlyList<Error> errors, string? requestUrl = null)
            : base(message, HttpStatusCode.Forbidden, errors, requestUrl)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitbucketForbiddenException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errors">The collection of errors from the Bitbucket response.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="requestUrl">The request URL that caused the error.</param>
        public BitbucketForbiddenException(string message, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
            : base(message, HttpStatusCode.Forbidden, errors, innerException, requestUrl)
        {
        }
    }
}
