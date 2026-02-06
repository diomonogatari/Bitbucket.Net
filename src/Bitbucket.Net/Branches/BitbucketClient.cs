using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Branches;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetBranchUrl() => GetBaseUrl("/branch-utils");

        private IFlurlRequest GetBranchUrl(string path) => GetBranchUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<BranchBase>> GetCommitBranchInfoAsync(string projectKey, string repositorySlug, string fullSha,
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
                    await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches/info/{fullSha}")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<BranchBase>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<BranchModel> GetRepoBranchModelAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
        {
            return await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branchmodel")
                .GetJsonAsync<BranchModel>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Branch> CreateRepoBranchAsync(string projectKey, string repositorySlug, string branchName, string startPoint, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                name = branchName,
                startPoint
            };

            var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches")
                .PostJsonAsync(data, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<Branch>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteRepoBranchAsync(string projectKey, string repositorySlug, string branchName, bool dryRun, string? endPoint = null, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                name = branchName,
                dryRun = BitbucketHelpers.BoolToString(dryRun),
                endPoint
            };

            var json = JsonSerializer.Serialize(data, s_jsonOptions);
            var response = await GetBranchUrl($"/projects/{projectKey}/repos/{repositorySlug}/branches")
                .WithHeader("Content-Type", "application/json")
                .SendAsync(HttpMethod.Delete, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
