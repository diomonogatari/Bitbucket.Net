using Bitbucket.Net.Common;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides hook-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base hooks URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the hooks endpoint.</returns>
    protected IFlurlRequest GetHooksUrl() => GetBaseUrl()
        .AppendPathSegment("/hooks");

    /// <summary>
    /// Retrieves the avatar for a project hook.
    /// </summary>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="version">Optional avatar version.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The avatar image bytes.</returns>
    public async Task<byte[]> GetProjectHooksAvatarAsync(string hookKey, string? version = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hookKey);

        var response = await GetHooksUrl()
            .AppendPathSegment($"/{hookKey}/avatar")
            .SetQueryParam("version", version)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseBytesAsync(response, cancellationToken).ConfigureAwait(false);
    }
}