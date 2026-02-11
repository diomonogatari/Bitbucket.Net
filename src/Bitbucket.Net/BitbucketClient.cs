using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Serialization;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Bitbucket.Net;

/// <summary>
/// Client for interacting with Bitbucket Server REST APIs.
/// <para>
/// The client implements <see cref="IDisposable"/>. When created via the
/// <see cref="BitbucketClient(HttpClient, string, Func{string}?)"/> constructor,
/// the client owns the internal <see cref="IFlurlClient"/> wrapper and disposes it.
/// When created via the <see cref="BitbucketClient(IFlurlClient, Func{string}?)"/> constructor,
/// the caller retains ownership of the <see cref="IFlurlClient"/> and is responsible for its disposal.
/// </para>
/// </summary>
public partial class BitbucketClient : IDisposable
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // Source-generated context only — no reflection fallback.
        // Missing [JsonSerializable] registrations in BitbucketJsonContext will throw
        // NotSupportedException at the call site, surfacing the problem immediately.
        TypeInfoResolver = BitbucketJsonContext.Default,
        Converters =
        {
            new UnixDateTimeOffsetConverter(),
            new NullableUnixDateTimeOffsetConverter(),
            new PermissionsConverter(),
            new RolesConverter(),
            new FileTypesConverter(),
            new LineTypesConverter(),
            new ParticipantStatusConverter(),
            new PullRequestStatesConverter(),
            new HookTypesConverter(),
            new ScopeTypesConverter(),
            new WebHookOutcomesConverter(),
            new RefRestrictionTypesConverter(),
            new SynchronizeActionsConverter(),
            new BlockerCommentStateConverter(),
            new CommentSeverityConverter()
        },
    };

    // Write-only options for serializing outbound request bodies.
    // Includes a reflection fallback for anonymous types used in API methods.
    // Future improvement: replace anonymous types with typed request DTOs and remove this fallback.
    private static readonly JsonSerializerOptions s_writeJsonOptions = new(s_jsonOptions)
    {
        TypeInfoResolver = JsonTypeInfoResolver.Combine(
            BitbucketJsonContext.Default,
            new DefaultJsonTypeInfoResolver()
        ),
    };

    private static readonly ISerializer s_serializer = new DefaultJsonSerializer(s_jsonOptions);

    private readonly Url _url;
    private readonly Func<string>? _getToken;
    private readonly string? _userName;
    private readonly string? _password;
    private readonly IFlurlClient? _injectedClient;
    private readonly bool _ownsClient;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitbucketClient"/> class with the specified base URL.
    /// </summary>
    /// <param name="url">The base Bitbucket Server URL.</param>
    private BitbucketClient(string url)
    {
        _url = url;
    }

    /// <summary>
    /// Creates a BitbucketClient with basic authentication.
    /// </summary>
    /// <param name="url">The base URL of the Bitbucket Server instance.</param>
    /// <param name="userName">The username for basic authentication.</param>
    /// <param name="password">The password for basic authentication.</param>
    public BitbucketClient(string url, string userName, string password)
        : this(url)
    {
        _userName = userName;
        _password = password;
    }

    /// <summary>
    /// Creates a BitbucketClient with token-based authentication.
    /// </summary>
    /// <param name="url">The base URL of the Bitbucket Server instance.</param>
    /// <param name="getToken">A function that returns the bearer token.</param>
    public BitbucketClient(string url, Func<string> getToken)
        : this(url)
    {
        _getToken = getToken;
    }

    /// <summary>
    /// Creates a BitbucketClient using an externally managed HttpClient.
    /// This constructor is designed for dependency injection scenarios where consumers
    /// want to configure the HttpClient with IHttpClientFactory, Polly resilience policies,
    /// custom timeouts, or other middleware.
    /// </summary>
    /// <param name="httpClient">The externally managed HttpClient instance. The client should be configured with any desired resilience policies, timeouts, etc.</param>
    /// <param name="baseUrl">The base URL of the Bitbucket Server instance.</param>
    /// <param name="getToken">Optional: A function that returns the bearer token for authentication.</param>
    /// <remarks>
    /// <para>
    /// When using this constructor, authentication should typically be handled by configuring
    /// the HttpClient with appropriate headers via IHttpClientFactory or DelegatingHandlers.
    /// If getToken is provided, it will add the Authorization header to each request.
    /// </para>
    /// <para>
    /// Example DI registration:
    /// <code>
    /// services.AddHttpClient&lt;BitbucketClient&gt;()
    ///     .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)))
    ///     .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromMinutes(2));
    ///     
    /// services.AddSingleton&lt;BitbucketClient&gt;(sp => {
    ///     var httpClient = sp.GetRequiredService&lt;IHttpClientFactory&gt;().CreateClient(nameof(BitbucketClient));
    ///     return new BitbucketClient(httpClient, "https://bitbucket.example.com", () => GetToken());
    /// });
    /// </code>
    /// </para>
    /// </remarks>
    public BitbucketClient(HttpClient httpClient, string baseUrl, Func<string>? getToken = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

        _url = baseUrl;
        _getToken = getToken;
        _injectedClient = new FlurlClient(httpClient, baseUrl)
            .WithSettings(settings => settings.JsonSerializer = s_serializer);
        _ownsClient = true;
    }

    /// <summary>
    /// Creates a BitbucketClient using an externally managed IFlurlClient.
    /// This constructor provides maximum control over the Flurl client configuration.
    /// </summary>
    /// <param name="flurlClient">The pre-configured IFlurlClient instance.</param>
    /// <param name="getToken">Optional: A function that returns the bearer token for authentication.</param>
    /// <remarks>
    /// Use this constructor when you need fine-grained control over Flurl's configuration,
    /// such as custom event handlers, advanced settings, or when using IFlurlClientCache.
    /// </remarks>
    public BitbucketClient(IFlurlClient flurlClient, Func<string>? getToken = null)
    {
        _injectedClient = flurlClient ?? throw new ArgumentNullException(nameof(flurlClient));
        _url = flurlClient.BaseUrl ?? throw new ArgumentException("FlurlClient must have a BaseUrl configured.", nameof(flurlClient));
        _getToken = getToken;
    }

    /// <summary>
    /// Releases the resources used by the <see cref="BitbucketClient"/>.
    /// When the client was created via the <see cref="HttpClient"/> constructor,
    /// the internal <see cref="IFlurlClient"/> wrapper is disposed.
    /// When created via the <see cref="IFlurlClient"/> constructor, disposal is a no-op
    /// since the caller retains ownership.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_ownsClient)
        {
            _injectedClient?.Dispose();
        }
    }

    /// <summary>
    /// Builds a Flurl request rooted at the Bitbucket REST API.
    /// </summary>
    /// <param name="root">The API root segment (default is <c>/api</c>).</param>
    /// <param name="version">The API version segment (default is <c>1.0</c>).</param>
    /// <returns>An <see cref="IFlurlRequest"/> configured with authentication and serialization.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when the client has been disposed.</exception>
    private IFlurlRequest GetBaseUrl(string root = "/api", string version = "1.0")
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        IFlurlRequest request;

        // If using injected client, use it directly
        if (_injectedClient != null)
        {
            request = _injectedClient
                .Request()
                .AppendPathSegment($"/rest{root}/{version}");

            // Apply token authentication if provided
            if (_getToken != null)
            {
                request = request.WithOAuthBearerToken(_getToken());
            }
        }
        else
        {
            // Original behavior for non-DI scenarios
            var fullUrl = new Url(_url)
                .AppendPathSegment($"/rest{root}/{version}");
            request = new FlurlRequest(fullUrl)
                .WithAuthentication(_getToken, _userName, _password);
        }

        return request
            .AllowAnyHttpStatus()
            .WithSettings(settings => settings.JsonSerializer = s_serializer);
    }

    private static async Task<string> ReadResponseStringAsync(IFlurlResponse response, CancellationToken cancellationToken)
    {
        if (response.ResponseMessage?.Content is null)
        {
            return string.Empty;
        }

        return await response.ResponseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    private static StringContent CreateJsonContent<TValue>(TValue value)
    {
        var json = JsonSerializer.Serialize(value, s_writeJsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static StringContent CreateEmptyJsonContent()
    {
        return new StringContent(string.Empty, Encoding.UTF8, "application/json");
    }

    private static async Task<byte[]> ReadResponseBytesAsync(IFlurlResponse response, CancellationToken cancellationToken)
    {
        if (response.ResponseMessage?.Content is null)
        {
            return Array.Empty<byte>();
        }

        return await response.ResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<Stream> ReadResponseStreamAsync(IFlurlResponse response, CancellationToken cancellationToken)
    {
        if (response.ResponseMessage?.Content is null)
        {
            return Stream.Null;
        }

        return await response.ResponseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the response content and deserializes it.
    /// When no custom content handler is provided, deserializes directly from the response stream
    /// to avoid intermediate string allocations (especially beneficial for large paged responses).
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="response">The HTTP response.</param>
    /// <param name="contentHandler">Optional custom handler to parse the response content as a string.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The deserialized response content.</returns>
    private static async Task<TResult> ReadResponseContentAsync<TResult>(IFlurlResponse response, Func<string, TResult>? contentHandler = null, CancellationToken cancellationToken = default)
    {
        // Custom handler needs the raw string (used for non-JSON responses)
        if (contentHandler is not null)
        {
            string content = await ReadResponseStringAsync(response, cancellationToken).ConfigureAwait(false);
            return contentHandler(content);
        }

        // Deserialize directly from the stream — avoids intermediate string allocation
        var stream = await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);
        await using (stream.ConfigureAwait(false))
        {
            if (stream == Stream.Null)
            {
                return default!;
            }

            return (await JsonSerializer.DeserializeAsync<TResult>(stream, s_jsonOptions, cancellationToken).ConfigureAwait(false))!;
        }
    }

    /// <summary>
    /// Reads the response content and returns success based on empty body.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the response body is empty; otherwise, <c>false</c>.</returns>
    private static async Task<bool> ReadResponseContentAsync(IFlurlResponse response, CancellationToken cancellationToken = default)
    {
        string content = await ReadResponseStringAsync(response, cancellationToken).ConfigureAwait(false);
        return string.IsNullOrWhiteSpace(content);
    }

    /// <summary>
    /// Throws an exception if the response indicates an error.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when error handling is finished.</returns>
    private static async Task HandleErrorsAsync(IFlurlResponse response, CancellationToken cancellationToken = default)
    {
        if (response.StatusCode >= 400)
        {
            var errors = Array.Empty<Error>();
            string? requestUrl = response.ResponseMessage?.RequestMessage?.RequestUri?.ToString();
            string? rawResponseBody = null;

            try
            {
                // Read the response body first so we can include it in the error if parsing fails
                rawResponseBody = await ReadResponseStringAsync(response, cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(rawResponseBody))
                {
                    var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(rawResponseBody, s_jsonOptions);
                    if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                    {
                        errors = [.. errorResponse.Errors];
                    }
                }
            }
            catch
            {
                // If we can't parse the error response as JSON, create a synthetic error with the raw body
                if (!string.IsNullOrWhiteSpace(rawResponseBody))
                {
                    // Truncate very long responses
                    var truncatedBody = rawResponseBody.Length > 500
                        ? rawResponseBody[..500] + "..."
                        : rawResponseBody;
                    errors = [new Error { Message = truncatedBody }];
                }
            }

            throw BitbucketApiException.Create(response.StatusCode, errors, requestUrl);
        }
    }

    /// <summary>
    /// Handles an HTTP response, throwing on errors and deserializing the content.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="response">The HTTP response.</param>
    /// <param name="contentHandler">Optional custom content handler.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The deserialized response content.</returns>
    private static async Task<TResult> HandleResponseAsync<TResult>(IFlurlResponse response, Func<string, TResult>? contentHandler = null, CancellationToken cancellationToken = default)
    {
        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseContentAsync(response, contentHandler, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles an HTTP response, throwing on errors and returning a boolean success indicator.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the response body is empty; otherwise, <c>false</c>.</returns>
    private static async Task<bool> HandleResponseAsync(IFlurlResponse response, CancellationToken cancellationToken = default)
    {
        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseContentAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves paged results from a paginated endpoint.
    /// </summary>
    /// <typeparam name="T">The item type in the paged results.</typeparam>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="queryParamValues">The query parameter values for requests.</param>
    /// <param name="selector">A delegate that retrieves a page of results.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>All retrieved items.</returns>
    private static async Task<IEnumerable<T>> GetPagedResultsAsync<T>(int? maxPages, IDictionary<string, object?> queryParamValues, Func<IDictionary<string, object?>, CancellationToken, Task<PagedResults<T>>> selector, CancellationToken cancellationToken = default)
    {
        var results = new List<T>();
        bool isLastPage = false;
        int numPages = 0;

        while (!isLastPage && (maxPages == null || numPages < maxPages))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var selectorResults = await selector(queryParamValues, cancellationToken).ConfigureAwait(false);
            results.AddRange(selectorResults.Values);

            isLastPage = selectorResults.IsLastPage;
            if (!isLastPage && selectorResults.NextPageStart.HasValue)
            {
                queryParamValues["start"] = selectorResults.NextPageStart.Value;
            }

            numPages++;
        }

        return results;
    }

    /// <summary>
    /// Streams paged results as an IAsyncEnumerable, yielding items as they are retrieved.
    /// This is more memory-efficient for large result sets and provides faster time-to-first-result.
    /// </summary>
    /// <typeparam name="T">The type of items in the paged results.</typeparam>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="queryParamValues">Query parameters for the API request.</param>
    /// <param name="selector">Function to retrieve a page of results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable that yields items as they are retrieved.</returns>
    private static async IAsyncEnumerable<T> GetPagedResultsStreamAsync<T>(
        int? maxPages,
        IDictionary<string, object?> queryParamValues,
        Func<IDictionary<string, object?>, CancellationToken, Task<PagedResults<T>>> selector,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        bool isLastPage = false;
        int numPages = 0;

        while (!isLastPage && (maxPages == null || numPages < maxPages))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var selectorResults = await selector(queryParamValues, cancellationToken).ConfigureAwait(false);

            foreach (var item in selectorResults.Values)
            {
                yield return item;
            }

            isLastPage = selectorResults.IsLastPage;
            if (!isLastPage && selectorResults.NextPageStart.HasValue)
            {
                queryParamValues["start"] = selectorResults.NextPageStart.Value;
            }

            numPages++;
        }
    }
}