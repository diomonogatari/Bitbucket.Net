using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides user-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base users URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the users endpoint.</returns>
    private IFlurlRequest GetUsersUrl() => GetBaseUrl()
        .AppendPathSegment("/users");

    /// <summary>
    /// Gets the users URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the users endpoint.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the users path.</returns>
    private IFlurlRequest GetUsersUrl(string path) => GetUsersUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves users with optional filters.
    /// </summary>
    /// <param name="filter">Optional search filter.</param>
    /// <param name="group">Optional group filter.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size for returned users.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="permissionN">Additional permission filters.</param>
    /// <returns>A collection of users.</returns>
    public Task<IReadOnlyList<User>> GetUsersAsync(string? filter = null, string? group = null, string? permission = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default,
        params string[] permissionN)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
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

        return GetPagedAsync<User>(
            GetUsersUrl(), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Updates the current user's profile fields.
    /// </summary>
    /// <param name="email">Optional email address.</param>
    /// <param name="displayName">Optional display name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated user.</returns>
    public async Task<User> UpdateUserAsync(string? email = null, string? displayName = null, CancellationToken cancellationToken = default)
    {
        var obj = new
        {
            displayName,
            email,
        };

        var response = await GetUsersUrl()
            .SendAsync(HttpMethod.Put, CreateJsonContent(obj), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<User>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the current user's credentials.
    /// </summary>
    /// <param name="passwordChange">The password change payload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateUserCredentialsAsync(PasswordChange passwordChange, CancellationToken cancellationToken = default)
    {
        var response = await GetUsersUrl("/credentials")
            .SendAsync(HttpMethod.Put, CreateJsonContent(passwordChange), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a user by slug.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="avatarSize">Optional avatar size for the returned user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested user.</returns>
    public async Task<User> GetUserAsync(string userSlug, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userSlug);

        var response = await GetUsersUrl($"/{userSlug}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<User>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a user's avatar.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the avatar was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteUserAvatarAsync(string userSlug, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userSlug);

        var response = await GetUsersUrl($"/{userSlug}/avatar.png")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves user settings.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A dictionary of user settings.</returns>
    public async Task<IDictionary<string, object?>> GetUserSettingsAsync(string userSlug, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userSlug);

        var response = await GetUsersUrl($"/{userSlug}/settings")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, object?>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates user settings.
    /// </summary>
    /// <param name="userSlug">The user slug.</param>
    /// <param name="userSettings">The settings to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the settings were updated; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateUserSettingsAsync(string userSlug, IDictionary<string, object?> userSettings, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userSlug);

        var response = await GetUsersUrl($"/{userSlug}/settings")
            .SendAsync(HttpMethod.Post, CreateJsonContent(userSettings), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }
}