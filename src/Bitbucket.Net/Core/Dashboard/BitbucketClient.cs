using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetDashboardUrl() => GetBaseUrl()
            .AppendPathSegment("/dashboard");

        private IFlurlRequest GetDashboardUrl(string path) => GetDashboardUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<PullRequest>> GetDashboardPullRequestsAsync(PullRequestStates? state = null,
            Roles? role = null,
            List<ParticipantStatus>? status = null,
            PullRequestOrders? order = PullRequestOrders.Newest,
            int? closedSinceSeconds = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["state"] = BitbucketHelpers.PullRequestStateToString(state),
                ["role"] = BitbucketHelpers.RoleToString(role),
                ["status"] = status != null ? string.Join(",", status.Select(BitbucketHelpers.ParticipantStatusToString)) : null,
                ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
                ["closedSince"] = closedSinceSeconds,
                ["limit"] = limit,
                ["start"] = start
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetDashboardUrl("/pull-requests")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<PullRequest>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<PullRequestSuggestion>> GetDashboardPullRequestSuggestionsAsync(int changesSinceSeconds = 172800,
            int? maxPages = null,
            int? limit = 3,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["changesSince"] = changesSinceSeconds,
                ["limit"] = limit,
                ["start"] = start
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetDashboardUrl("/pull-request-suggestions")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<PullRequestSuggestion>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
