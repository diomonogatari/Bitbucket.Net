using Bitbucket.Net.Common.Models;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Exception thrown when rate limiting is applied (HTTP 429 Too Many Requests).
/// This indicates too many requests have been made in a given time period.
/// </summary>
public class BitbucketRateLimitException : BitbucketApiException
{
    /// <summary>How long to wait before retrying (from Retry-After header).</summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>Maximum rate limit (from X-RateLimit-Limit header).</summary>
    public int? RateLimit { get; }

    /// <summary>Remaining requests in the current window (from X-RateLimit-Remaining header).</summary>
    public int? RateLimitRemaining { get; }

    /// <summary>When the rate limit resets (from X-RateLimit-Reset header, Unix seconds).</summary>
    public DateTimeOffset? RateLimitReset { get; }

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
    /// Initializes a new instance of the <see cref="BitbucketRateLimitException"/> class with rate-limit headers.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="retryAfter">How long to wait before retrying.</param>
    /// <param name="rateLimit">Maximum rate limit.</param>
    /// <param name="rateLimitRemaining">Remaining requests in the current window.</param>
    /// <param name="rateLimitReset">When the rate limit resets.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketRateLimitException(
        string message,
        IReadOnlyList<Error> errors,
        TimeSpan? retryAfter,
        int? rateLimit,
        int? rateLimitRemaining,
        DateTimeOffset? rateLimitReset,
        string? requestUrl = null)
        : base(message, HttpStatusCode.TooManyRequests, errors, requestUrl)
    {
        RetryAfter = retryAfter;
        RateLimit = rateLimit;
        RateLimitRemaining = rateLimitRemaining;
        RateLimitReset = rateLimitReset;
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