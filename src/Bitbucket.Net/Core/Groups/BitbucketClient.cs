using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides group-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base groups URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the groups endpoint.</returns>
    private IFlurlRequest GetGroupsUrl() => GetBaseUrl()
        .AppendPathSegment("/groups");

    /// <summary>
    /// Retrieves group names with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of group names.</returns>
    public Task<IReadOnlyList<string>> GetGroupNamesAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedAsync<string>(
            GetGroupsUrl(), queryParamValues, maxPages, cancellationToken);
    }
}