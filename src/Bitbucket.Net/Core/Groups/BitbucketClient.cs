using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common.Models;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetGroupsUrl() => GetBaseUrl()
            .AppendPathSegment("/groups");

        public async Task<IEnumerable<string>> GetGroupNamesAsync(string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["filter"] = filter,
                ["limit"] = limit,
                ["start"] = start
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetGroupsUrl()
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<string>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
