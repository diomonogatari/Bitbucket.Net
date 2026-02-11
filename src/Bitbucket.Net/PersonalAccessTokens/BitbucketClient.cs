using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.PersonalAccessTokens;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides personal access token related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base personal access token URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the PAT root.</returns>
    private IFlurlRequest GetPatUrl() => GetBaseUrl("/access-tokens");

    /// <summary>
    /// Gets the personal access token URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the PAT root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the PAT path.</returns>
    private IFlurlRequest GetPatUrl(string path) => GetPatUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves access tokens for a user.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of access tokens.</returns>
    public async Task<IReadOnlyList<AccessToken>> GetUserAccessTokensAsync(string userSlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetPatUrl($"/users/{userSlug}")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<AccessToken>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a personal access token for a user.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="accessToken">The token creation payload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created access token including secret.</returns>
    public async Task<FullAccessToken> CreateAccessTokenAsync(string userSlug, AccessTokenCreate accessToken, CancellationToken cancellationToken = default)
    {
        var response = await GetPatUrl($"/users/{userSlug}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(accessToken), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<FullAccessToken>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a personal access token by identifier.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="tokenId">The token identifier.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The access token details.</returns>
    public async Task<AccessToken> GetUserAccessTokenAsync(string userSlug, string tokenId, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetPatUrl($"/users/{userSlug}/{tokenId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<AccessToken>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a personal access token.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="tokenId">The token identifier.</param>
    /// <param name="accessToken">The updated token payload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated access token details.</returns>
    public async Task<AccessToken> ChangeUserAccessTokenAsync(string userSlug, string tokenId, AccessTokenCreate accessToken, CancellationToken cancellationToken = default)
    {
        var response = await GetPatUrl($"/users/{userSlug}/{tokenId}")
            .SendAsync(HttpMethod.Post, CreateJsonContent(accessToken), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<FullAccessToken>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a personal access token.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="tokenId">The token identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the token was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteUserAccessTokenAsync(string userSlug, string tokenId, CancellationToken cancellationToken = default)
    {
        var response = await GetPatUrl($"/users/{userSlug}/{tokenId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}