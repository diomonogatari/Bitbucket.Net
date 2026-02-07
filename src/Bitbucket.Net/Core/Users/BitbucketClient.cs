using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetUsersUrl() => GetBaseUrl()
        .AppendPathSegment("/users");

    private IFlurlRequest GetUsersUrl(string path) => GetUsersUrl()
        .AppendPathSegment(path);

    public async Task<IEnumerable<User>> GetUsersAsync(string? filter = null, string? group = null, string? permission = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default,
        params string[] permissionN)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
            ["filter"] = filter,
            ["group"] = group,
            ["permission"] = permission,
        };

        int permissionNCounter = 0;
        foreach (string perm in permissionN)
        {
            permissionNCounter++;
            queryParamValues.Add($"permission.{permissionNCounter}", perm);
        }

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetUsersUrl()
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<User> UpdateUserAsync(string? email = null, string? displayName = null, CancellationToken cancellationToken = default)
    {
        var obj = new
        {
            displayName,
            email,
        };

        var response = await GetUsersUrl()
            .PutJsonAsync(obj, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<User>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> UpdateUserCredentialsAsync(PasswordChange passwordChange, CancellationToken cancellationToken = default)
    {
        var response = await GetUsersUrl("/credentials")
            .PutJsonAsync(passwordChange, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<User> GetUserAsync(string userSlug, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        return await GetUsersUrl($"/{userSlug}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetJsonAsync<User>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserAvatarAsync(string userSlug, CancellationToken cancellationToken = default)
    {
        var response = await GetUsersUrl($"/{userSlug}/avatar.png")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IDictionary<string, object?>> GetUserSettingsAsync(string userSlug, CancellationToken cancellationToken = default)
    {
        var response = await GetUsersUrl($"/{userSlug}/settings")
            .GetJsonAsync<Dictionary<string, object?>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response;
    }

    public async Task<bool> UpdateUserSettingsAsync(string userSlug, IDictionary<string, object?> userSettings, CancellationToken cancellationToken = default)
    {
        var response = await GetUsersUrl($"/{userSlug}/settings")
            .PostJsonAsync(userSettings, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}