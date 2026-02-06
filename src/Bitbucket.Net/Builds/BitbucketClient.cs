using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Builds;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetBuildsUrl() => GetBaseUrl("/build-status");

        private IFlurlRequest GetBuildsUrl(string path) => GetBuildsUrl()
            .AppendPathSegment(path);

        public async Task<BuildStats> GetBuildStatsForCommitAsync(string commitId, bool includeUnique = false, CancellationToken cancellationToken = default)
        {
            return await GetBuildsUrl($"/commits/stats/{commitId}")
                .SetQueryParam("includeUnique", BitbucketHelpers.BoolToString(includeUnique))
                .GetJsonAsync<BuildStats>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(CancellationToken cancellationToken, params string[] commitIds)
        {
            var response = await GetBuildsUrl("/commits/stats")
                .PostJsonAsync(commitIds, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<Dictionary<string, BuildStats>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, BuildStats>> GetBuildStatsForCommitsAsync(params string[] commitIds)
        {
            return await GetBuildStatsForCommitsAsync(default, commitIds).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BuildStatus>> GetBuildStatusForCommitAsync(string commitId,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetBuildsUrl($"/commits/{commitId}")
                        .GetJsonAsync<PagedResults<BuildStatus>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> AssociateBuildStatusWithCommitAsync(string commitId, BuildStatus buildStatus, CancellationToken cancellationToken = default)
        {
            var response = await GetBuildsUrl($"/commits/{commitId}")
                .PostJsonAsync(buildStatus, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
