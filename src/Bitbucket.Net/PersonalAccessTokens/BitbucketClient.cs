using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.PersonalAccessTokens;
using Flurl.Http;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetPatUrl() => GetBaseUrl("/access-tokens");

        private IFlurlRequest GetPatUrl(string path) => GetPatUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<AccessToken>> GetUserAccessTokensAsync(string userSlug,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            int? avatarSize = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetPatUrl($"/users/{userSlug}")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<AccessToken>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<FullAccessToken> CreateAccessTokenAsync(string userSlug, AccessTokenCreate accessToken, CancellationToken cancellationToken = default)
        {
            var response = await GetPatUrl($"/users/{userSlug}")
                .PutJsonAsync(accessToken, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<FullAccessToken>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<AccessToken> GetUserAccessTokenAsync(string userSlug, string tokenId, int? avatarSize = null, CancellationToken cancellationToken = default)
        {
            return await GetPatUrl($"/users/{userSlug}/{tokenId}")
	            .SetQueryParam("avatarSize", avatarSize)
                .GetJsonAsync<AccessToken>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<AccessToken> ChangeUserAccessTokenAsync(string userSlug, string tokenId, AccessTokenCreate accessToken, CancellationToken cancellationToken = default)
        {
            var response = await GetPatUrl($"/users/{userSlug}/{tokenId}")
                .PostJsonAsync(accessToken, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<FullAccessToken>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteUserAccessTokenAsync(string userSlug, string tokenId, CancellationToken cancellationToken = default)
        {
            var response = await GetPatUrl($"/users/{userSlug}/{tokenId}")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }
}
