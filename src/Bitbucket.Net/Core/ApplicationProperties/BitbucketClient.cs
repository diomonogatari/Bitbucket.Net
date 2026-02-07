using Flurl.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
    private IFlurlRequest GetApplicationPropertiesUrl() => GetBaseUrl()
        .AppendPathSegment("/application-properties");

    /// <summary>
    /// Retrieves application properties.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A dictionary of application property values.</returns>
    public async Task<IDictionary<string, object?>> GetApplicationPropertiesAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetApplicationPropertiesUrl()
            .GetJsonAsync<Dictionary<string, object?>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response;
    }
}