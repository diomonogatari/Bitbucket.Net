using Bitbucket.Net.Common.Models;
using System.Net;

namespace Bitbucket.Net.Common.Exceptions;

/// <summary>
/// Base exception for all Bitbucket API errors. Contains detailed error information
/// from the Bitbucket Server response.
/// </summary>
public class BitbucketApiException : Exception
{
    /// <summary>
    /// Gets the HTTP status code returned by the Bitbucket Server.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the context information from the first error, if available.
    /// This typically contains the field or resource that caused the error.
    /// </summary>
    public string? Context { get; }

    /// <summary>
    /// Gets the collection of errors returned by the Bitbucket Server.
    /// </summary>
    public IReadOnlyList<Error> Errors { get; }

    /// <summary>
    /// Gets the request URL that caused the error, if available.
    /// </summary>
    public string? RequestUrl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketApiException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketApiException(string message, HttpStatusCode statusCode, IReadOnlyList<Error> errors, string? requestUrl = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? [];
        Context = errors?.Count > 0 ? errors[0].Context : null;
        RequestUrl = requestUrl;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketApiException"/> class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    public BitbucketApiException(string message, HttpStatusCode statusCode, IReadOnlyList<Error> errors, Exception innerException, string? requestUrl = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Errors = errors ?? [];
        Context = errors?.Count > 0 ? errors[0].Context : null;
        RequestUrl = requestUrl;
    }

    /// <summary>
    /// Creates the appropriate exception type based on the HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errors">The collection of errors from the Bitbucket response.</param>
    /// <param name="requestUrl">The request URL that caused the error.</param>
    /// <returns>A typed exception matching the HTTP status code.</returns>
    public static BitbucketApiException Create(int statusCode, IReadOnlyList<Error> errors, string? requestUrl = null)
    {
        var httpStatusCode = (HttpStatusCode)statusCode;
        string message = BuildErrorMessage(httpStatusCode, errors);

        return statusCode switch
        {
            400 => new BitbucketBadRequestException(message, errors, requestUrl),
            401 => new BitbucketAuthenticationException(message, errors, requestUrl),
            403 => new BitbucketForbiddenException(message, errors, requestUrl),
            404 => new BitbucketNotFoundException(message, errors, requestUrl),
            409 => new BitbucketConflictException(message, errors, requestUrl),
            422 => new BitbucketValidationException(message, errors, requestUrl),
            429 => new BitbucketRateLimitException(message, errors, requestUrl),
            >= 500 and < 600 => new BitbucketServerException(message, httpStatusCode, errors, requestUrl),
            _ => new BitbucketApiException(message, httpStatusCode, errors, requestUrl),
        };
    }

    private static string BuildErrorMessage(HttpStatusCode statusCode, IReadOnlyList<Error> errors)
    {
        if (errors == null || errors.Count == 0)
        {
            return $"Bitbucket API request failed with status {(int)statusCode} ({statusCode})";
        }

        var messages = new List<string>(errors.Count);
        foreach (var error in errors)
        {
            if (!string.IsNullOrEmpty(error.Context))
            {
                messages.Add($"[{error.Context}] {error.Message}");
            }
            else
            {
                messages.Add(error.Message);
            }
        }

        return $"Bitbucket API request failed with status {(int)statusCode} ({statusCode}): {string.Join("; ", messages)}";
    }
}