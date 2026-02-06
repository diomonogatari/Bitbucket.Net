using System.Collections.Generic;
using System.Text.Json;
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
        private IFlurlRequest GetInboxUrl() => GetBaseUrl()
            .AppendPathSegment("/inbox");

        private IFlurlRequest GetInboxUrl(string path) => GetInboxUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<PullRequest>> GetInboxPullRequestsAsync(
            int? maxPages = null,
            int? limit = 25,
            int? start = 0,
            Roles role = Roles.Reviewer,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["role"] = BitbucketHelpers.RoleToString(role)
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetInboxUrl("/pull-requests")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<PullRequest>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<int> GetInboxPullRequestsCountAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetInboxUrl("/pull-requests/count")
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, s =>
            {
                using var doc = JsonDocument.Parse(s);
                return doc.RootElement.GetProperty("count").GetInt32();
            }, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
