using Bitbucket.Net.Common.Models.Search;
using Flurl;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides operations for the Bitbucket Server code search API (Elasticsearch-backed).
/// Requires the Bitbucket Code Search add-on to be installed on the server.
/// </summary>
public partial class BitbucketClient
{
    protected IFlurlRequest GetSearchUrl() => GetBaseUrl("/search", "latest");

    /// <summary>
    /// Performs a server-side code search using the Bitbucket Code Search API.
    /// This is backed by Elasticsearch and significantly faster than client-side file scanning.
    /// </summary>
    /// <param name="query">
    /// The search query string. Supports Bitbucket search syntax:
    /// <list type="bullet">
    ///   <item><c>repo:slug</c> — filter to a specific repository</item>
    ///   <item><c>project:KEY</c> — filter to a specific project</item>
    ///   <item><c>lang:csharp</c> — filter by language</item>
    ///   <item><c>ext:cs</c> — filter by file extension</item>
    ///   <item><c>path:src/</c> — filter by file path</item>
    /// </list>
    /// </param>
    /// <param name="primaryLimit">Maximum number of file results to return. Default: 25.</param>
    /// <param name="secondaryLimit">Maximum number of hit contexts per file. Default: 10.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The code search response containing matching files and hit contexts.</returns>
    /// <exception cref="Common.Exceptions.BitbucketApiException">
    /// Thrown when the server returns an error (e.g., 404 if Code Search is not installed).
    /// </exception>
    public async Task<CodeSearchResponse> SearchCodeAsync(
        string query,
        int primaryLimit = 25,
        int secondaryLimit = 10,
        CancellationToken cancellationToken = default)
    {
        var request = new CodeSearchRequest
        {
            Query = query,
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits
            {
                Primary = primaryLimit,
                Secondary = secondaryLimit
            }
        };

        var response = await GetSearchUrl()
            .AppendPathSegment("/search")
            .SendAsync(HttpMethod.Post, CreateJsonContent(request), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CodeSearchResponse>(response, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Checks whether the Bitbucket Code Search API is available on the server.
    /// Returns true if the search endpoint responds successfully, false otherwise.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if server-side search is available; false otherwise.</returns>
    public async Task<bool> IsSearchAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Perform a minimal search to probe the endpoint
            var request = new CodeSearchRequest
            {
                Query = "test",
                Entities = SearchEntities.CodeOnly,
                Limits = new SearchLimits { Primary = 1, Secondary = 1 }
            };

            var response = await GetSearchUrl()
                .AppendPathSegment("/search")
                .SendAsync(HttpMethod.Post, CreateJsonContent(request), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return response.StatusCode < 400;
        }
        catch
        {
            return false;
        }
    }
}