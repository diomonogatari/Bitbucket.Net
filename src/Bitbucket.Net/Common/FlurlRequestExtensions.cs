using Flurl.Http;

namespace Bitbucket.Net.Common;

/// <summary>
/// Extension methods for configuring and invoking Flurl requests in the Bitbucket client.
/// </summary>
public static class FlurlRequestExtensions
{
    /// <summary>
    /// Applies either bearer-token or basic authentication to the request.
    /// </summary>
    /// <param name="request">The request to decorate.</param>
    /// <param name="getToken">Delegate that supplies an OAuth bearer token. When provided, bearer auth is used.</param>
    /// <param name="userName">The user name for basic authentication.</param>
    /// <param name="password">The password for basic authentication.</param>
    /// <returns>The authenticated <see cref="IFlurlRequest"/>.</returns>
    public static IFlurlRequest WithAuthentication(this IFlurlRequest request, Func<string>? getToken, string? userName, string? password)
    {
        if (getToken != null)
        {
            string token = getToken();
            return request.WithOAuthBearerToken(token);
        }

        return request.WithBasicAuth(userName, password);
    }

    /// <summary>
    /// Sends a GET request, honoring the provided cancellation token.
    /// </summary>
    /// <param name="request">The request to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The Flurl response.</returns>
    public static Task<IFlurlResponse> GetAsync(this IFlurlRequest request, CancellationToken cancellationToken)
    {
        return request.GetAsync(HttpCompletionOption.ResponseContentRead, cancellationToken);
    }
}