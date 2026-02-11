using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides dashboard-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base dashboard URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the dashboard root.</returns>
    private IFlurlRequest GetDashboardUrl() => GetBaseUrl()
        .AppendPathSegment("/dashboard");

    /// <summary>
    /// Gets the dashboard URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the dashboard root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the dashboard path.</returns>
    private IFlurlRequest GetDashboardUrl(string path) => GetDashboardUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves pull requests for the current user's dashboard.
    /// </summary>
    /// <param name="state">Optional pull request state filter.</param>
    /// <param name="role">Optional participant role filter.</param>
    /// <param name="status">Optional participant status filters.</param>
    /// <param name="order">Optional sort order.</param>
    /// <param name="closedSinceSeconds">Optional filter for recently closed PRs (seconds).</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of pull requests.</returns>
    public async Task<IReadOnlyList<PullRequest>> GetDashboardPullRequestsAsync(PullRequestStates? state = null,
        Roles? role = null,
        List<ParticipantStatus>? status = null,
        PullRequestOrders? order = PullRequestOrders.Newest,
        int? closedSinceSeconds = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["role"] = BitbucketHelpers.RoleToString(role),
            ["status"] = status != null ? string.Join(',', status.Select(BitbucketHelpers.ParticipantStatusToString)) : null,
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["closedSince"] = closedSinceSeconds,
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetDashboardUrl("/pull-requests")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequest>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams pull requests for the current user's dashboard, yielding items as they are retrieved.
    /// </summary>
    /// <param name="state">Optional pull request state filter.</param>
    /// <param name="role">Optional participant role filter.</param>
    /// <param name="status">Optional participant status filters.</param>
    /// <param name="order">Optional sort order.</param>
    /// <param name="closedSinceSeconds">Optional filter for recently closed PRs (seconds).</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of pull requests.</returns>
    public IAsyncEnumerable<PullRequest> GetDashboardPullRequestsStreamAsync(PullRequestStates? state = null,
        Roles? role = null,
        List<ParticipantStatus>? status = null,
        PullRequestOrders? order = PullRequestOrders.Newest,
        int? closedSinceSeconds = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["role"] = BitbucketHelpers.RoleToString(role),
            ["status"] = status is not null ? string.Join(',', status.Select(BitbucketHelpers.ParticipantStatusToString)) : null,
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["closedSince"] = closedSinceSeconds,
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetDashboardUrl("/pull-requests")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequest>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Retrieves pull request suggestions for the current user.
    /// </summary>
    /// <param name="changesSinceSeconds">Time window in seconds to consider changes.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size (default 3).</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of pull request suggestions.</returns>
    public async Task<IReadOnlyList<PullRequestSuggestion>> GetDashboardPullRequestSuggestionsAsync(int changesSinceSeconds = 172800,
        int? maxPages = null,
        int? limit = 3,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["changesSince"] = changesSinceSeconds,
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetDashboardUrl("/pull-request-suggestions")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequestSuggestion>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }
}