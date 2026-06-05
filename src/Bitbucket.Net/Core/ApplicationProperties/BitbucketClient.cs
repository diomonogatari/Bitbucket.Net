using Bitbucket.Net.Common;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides application properties Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base application properties URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the application-properties endpoint.</returns>
    protected IFlurlRequest GetApplicationPropertiesUrl() => GetBaseUrl()
        .AppendPathSegment("/application-properties");

    /// <summary>
    /// Retrieves application properties.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A dictionary of application property values.</returns>
    public async Task<IDictionary<string, object?>> GetApplicationPropertiesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetApplicationPropertiesUrl()
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, object?>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}