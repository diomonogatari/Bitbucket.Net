using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.RefRestrictions;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetRefRestrictionsUrl() => GetBaseUrl("/branch-permissions", "2.0");

        private IFlurlRequest GetRefRestrictionsUrl(string path) => GetRefRestrictionsUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<RefRestriction>> GetProjectRefRestrictionsAsync(string projectKey,
            RefRestrictionTypes? type = null,
            RefMatcherTypes? matcherType = null,
            string? matcherId = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            int? avatarSize = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["type"] = BitbucketHelpers.RefRestrictionTypeToString(type),
                ["matcherType"] = BitbucketHelpers.RefMatcherTypeToString(matcherType),
                ["matcherId"] = matcherId,
                ["limit"] = limit,
                ["start"] = start,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<RefRestriction>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
                .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
                .PostJsonAsync(refRestrictions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<IEnumerable<RefRestriction>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RefRestriction>> CreateProjectRefRestrictionsAsync(string projectKey, params RefRestrictionCreate[] refRestrictions)
        {
            return await CreateProjectRefRestrictionsAsync(projectKey, default, refRestrictions).ConfigureAwait(false);
        }

        public async Task<RefRestriction> CreateProjectRefRestrictionAsync(string projectKey, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions")
                .PostJsonAsync(refRestriction, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<RefRestriction> GetProjectRefRestrictionAsync(string projectKey, int refRestrictionId, int? avatarSize = null, CancellationToken cancellationToken = default)
        {
            return await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions/{refRestrictionId}")
	            .SetQueryParam("avatarSize", avatarSize)
                .GetJsonAsync<RefRestriction>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> DeleteProjectRefRestrictionAsync(string projectKey, int refRestrictionId, CancellationToken cancellationToken = default)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/restrictions/{refRestrictionId}")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RefRestriction>> GetRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug,
            RefRestrictionTypes? type = null,
            RefMatcherTypes? matcherType = null,
            string? matcherId = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            int? avatarSize = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["type"] = BitbucketHelpers.RefRestrictionTypeToString(type),
                ["matcherType"] = BitbucketHelpers.RefMatcherTypeToString(matcherType),
                ["matcherId"] = matcherId,
                ["limit"] = limit,
                ["start"] = start,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<RefRestriction>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken, params RefRestrictionCreate[] refRestrictions)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                .WithHeader("Accept", "application/vnd.atl.bitbucket.bulk+json")
                .PostJsonAsync(refRestrictions, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<IEnumerable<RefRestriction>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RefRestriction>> CreateRepositoryRefRestrictionsAsync(string projectKey, string repositorySlug, params RefRestrictionCreate[] refRestrictions)
        {
            return await CreateRepositoryRefRestrictionsAsync(projectKey, repositorySlug, default, refRestrictions).ConfigureAwait(false);
        }

        public async Task<RefRestriction> CreateRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, RefRestrictionCreate refRestriction, CancellationToken cancellationToken = default)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions")
                .PostJsonAsync(refRestriction, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<RefRestriction>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<RefRestriction> GetRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId,
	        int? avatarSize = null, CancellationToken cancellationToken = default)
        {
            return await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions/{refRestrictionId}")
	            .SetQueryParam("avatarSize", avatarSize)
                .GetJsonAsync<RefRestriction>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> DeleteRepositoryRefRestrictionAsync(string projectKey, string repositorySlug, int refRestrictionId, CancellationToken cancellationToken = default)
        {
            var response = await GetRefRestrictionsUrl($"/projects/{projectKey}/repos/{repositorySlug}/restrictions/{refRestrictionId}")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
