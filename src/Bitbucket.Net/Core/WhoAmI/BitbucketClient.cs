using Bitbucket.Net.Common;
using Flurl;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides user identity helper operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the username of the currently authenticated user.
    /// Uses the /plugins/servlet/applinks/whoami endpoint which returns
    /// just the username as plain text.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The username of the authenticated user, or null if not authenticated.</returns>
    /// <remarks>
    /// This endpoint is essential for MCP servers and other integrations that need to 
    /// identify the current user context. Unlike GetUsersAsync(), this returns the 
    /// authenticated user specifically, not a list of all users.
    /// 
    /// Usage example:
    /// <code>
    /// var client = new BitbucketClient(url, () => token);
    /// 
    /// // Get the authenticated user's username
    /// var username = await client.GetWhoAmIAsync();
    /// 
    /// // Then fetch full user details if needed
    /// if (username != null)
    /// {
    ///     var currentUser = await client.GetUserAsync(username);
    /// }
    /// </code>
    /// </remarks>
    public async Task<string?> GetWhoAmIAsync(CancellationToken cancellationToken = default)
    {
        string response;

        // Handle DI constructor scenario (injected IFlurlClient or HttpClient)
        if (_injectedClient != null)
        {
            var request = _injectedClient
                .Request()
                .AppendPathSegment("/plugins/servlet/applinks/whoami");

            // Apply token authentication if provided
            if (_getToken != null)
            {
                request = request.WithOAuthBearerToken(_getToken());
            }

            response = await request
                .GetStringAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            // Original behavior for non-DI scenarios
            // Construct full URL and convert to IFlurlRequest for authentication
            var fullUrl = new Url(_url).AppendPathSegment("/plugins/servlet/applinks/whoami");
            response = await new FlurlRequest(fullUrl)
                .WithAuthentication(_getToken, _userName, _password)
                .GetStringAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        return string.IsNullOrWhiteSpace(response) ? null : response.Trim();
    }
}