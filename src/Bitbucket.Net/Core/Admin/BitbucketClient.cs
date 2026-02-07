using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PasswordChange = Bitbucket.Net.Models.Core.Admin.PasswordChange;

namespace Bitbucket.Net;

/// <summary>
/// Provides administrative operations for Bitbucket Server, including user, group, permissions, license, and mail server management.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base admin URL for Bitbucket Server operations.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the admin endpoint.</returns>
    private IFlurlRequest GetAdminUrl() => GetBaseUrl()
        .AppendPathSegment("/admin");

    /// <summary>
    /// Gets the admin URL for a specific path.
    /// </summary>
    /// <param name="path">The path to append.</param>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the admin path.</returns>
    private IFlurlRequest GetAdminUrl(string path) => GetAdminUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves groups with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of groups.</returns>
    public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminGroupsAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/groups")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a group.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created group.</returns>
    public async Task<DeletableGroupOrUser> CreateAdminGroupAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/groups")
            .SetQueryParam("name", name)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DeletableGroupOrUser>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a group.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deleted group info.</returns>
    public async Task<DeletableGroupOrUser> DeleteAdminGroupAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/groups")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<DeletableGroupOrUser>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds users to a group.
    /// </summary>
    /// <param name="groupUsers">The group and user payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if users were added; otherwise, <c>false</c>.</returns>
    public async Task<bool> AddAdminGroupUsersAsync(GroupUsers groupUsers, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/groups/add-users")
            .PostJsonAsync(groupUsers, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves members of a group beyond the initial page.
    /// </summary>
    /// <param name="context">The group context.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of group members.</returns>
    public async Task<IEnumerable<UserInfo>> GetAdminGroupMoreMembersAsync(string context, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["context"] = context,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/groups/more-members")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves non-members for a group beyond the initial page.
    /// </summary>
    /// <param name="context">The group context.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of non-members.</returns>
    public async Task<IEnumerable<UserInfo>> GetAdminGroupMoreNonMembersAsync(string context, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["context"] = context,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/groups/more-non-members")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves users with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users.</returns>
    public async Task<IEnumerable<UserInfo>> GetAdminUsersAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/users")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a user.
    /// </summary>
    /// <param name="name">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="displayName">The display name.</param>
    /// <param name="emailAddress">The email address.</param>
    /// <param name="addToDefaultGroup">Whether to add to the default group.</param>
    /// <param name="notify">Whether to notify the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> CreateAdminUserAsync(string name, string password, string displayName, string emailAddress,
        bool addToDefaultGroup = true, string notify = "false", CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["name"] = name,
            ["password"] = password,
            ["displayName"] = displayName,
            ["emailAddress"] = emailAddress,
            ["addToDefaultGroup"] = BitbucketHelpers.BoolToString(addToDefaultGroup),
            ["notify"] = notify,
        };

        var response = await GetAdminUrl("/users")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates user details.
    /// </summary>
    /// <param name="name">Optional username to update.</param>
    /// <param name="displayName">Optional display name.</param>
    /// <param name="emailAddress">Optional email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    public async Task<UserInfo> UpdateAdminUserAsync(string? name = null, string? displayName = null, string? emailAddress = null, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            name,
            displayName,
            email = emailAddress,
        };

        var response = await GetAdminUrl("/users")
            .PutJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="name">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deleted user info.</returns>
    public async Task<UserInfo> DeleteAdminUserAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/users")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds groups to a user.
    /// </summary>
    /// <param name="userGroups">The user groups payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if groups were added; otherwise, <c>false</c>.</returns>
    public async Task<bool> AddAdminUserGroupsAsync(UserGroups userGroups, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/users/add-groups")
            .PostJsonAsync(userGroups, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes captcha for a user.
    /// </summary>
    /// <param name="name">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAdminUserCaptcha(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/users/captcha")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates user credentials.
    /// </summary>
    /// <param name="passwordChange">The password change payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateAdminUserCredentialsAsync(PasswordChange passwordChange, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/users/credentials")
            .PutJsonAsync(passwordChange, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves additional groups for a user (memberships) beyond the first page.
    /// </summary>
    /// <param name="context">The username context.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of group memberships.</returns>
    public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminUserMoreMembersAsync(string context, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["context"] = context,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/users/more-members")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves additional groups that a user is not a member of.
    /// </summary>
    /// <param name="context">The username context.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of non-member groups.</returns>
    public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminUserMoreNonMembersAsync(string context, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["context"] = context,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/users/more-non-members")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a user from a group.
    /// </summary>
    /// <param name="userName">The username.</param>
    /// <param name="groupName">The group name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> RemoveAdminUserFromGroupAsync(string userName, string groupName, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            context = userName,
            itemName = groupName,
        };

        var response = await GetAdminUrl("/users/remove-group")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Renames a user.
    /// </summary>
    /// <param name="userRename">The rename payload.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user info.</returns>
    public async Task<UserInfo> RenameAdminUserAsync(UserRename userRename, int? avatarSize = null, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("users/rename")
            .SetQueryParam("avatarSize", avatarSize)
            .PostJsonAsync(userRename, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves cluster information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cluster details.</returns>
    public async Task<Cluster> GetAdminClusterAsync(CancellationToken cancellationToken = default)
    {
        return await GetAdminUrl("/cluster")
            .GetJsonAsync<Cluster>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves license details.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The license details.</returns>
    public async Task<LicenseDetails> GetAdminLicenseAsync(CancellationToken cancellationToken = default)
    {
        return await GetAdminUrl("/license")
            .GetJsonAsync<LicenseDetails>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates license information.
    /// </summary>
    /// <param name="licenseInfo">The license payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated license details.</returns>
    public async Task<LicenseDetails> UpdateAdminLicenseAsync(LicenseInfo licenseInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/license")
            .PostJsonAsync(licenseInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<LicenseDetails>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves mail server configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The mail server configuration.</returns>
    public async Task<MailServerConfiguration> GetAdminMailServerAsync(CancellationToken cancellationToken = default)
    {
        return await GetAdminUrl("/mail-server")
            .GetJsonAsync<MailServerConfiguration>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates mail server configuration.
    /// </summary>
    /// <param name="mailServerConfiguration">The configuration payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated configuration.</returns>
    public async Task<MailServerConfiguration> UpdateAdminMailServerAsync(MailServerConfiguration mailServerConfiguration, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/mail-server")
            .PutJsonAsync(mailServerConfiguration, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<MailServerConfiguration>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes mail server configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAdminMailServerAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/mail-server")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the mail server sender address.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sender address.</returns>
    public async Task<string> GetAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/mail-server/sender-address")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s => s, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the mail server sender address.
    /// </summary>
    /// <param name="senderAddress">The sender address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sender address.</returns>
    public async Task<string> UpdateAdminMailServerSenderAddressAsync(string senderAddress, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/mail-server/sender-address")
            .PutJsonAsync(senderAddress, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s => s, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the mail server sender address.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/mail-server/sender-address")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves admin group permissions with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of group permissions.</returns>
    public async Task<IEnumerable<GroupPermission>> GetAdminGroupPermissionsAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/permissions/groups")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<GroupPermission>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a group's permissions.
    /// </summary>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateAdminGroupPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["permission"] = permission,
            ["name"] = name,
        };

        var response = await GetAdminUrl("/permissions/groups")
            .SetQueryParams(queryParamValues)
            .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a group's permissions.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the permissions were removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAdminGroupPermissionsAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/permissions/groups")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves groups that currently have no admin permissions.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of groups without permissions.</returns>
    public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminGroupPermissionsNoneAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/permissions/groups/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves admin user permissions with optional filtering.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of user permissions.</returns>
    public async Task<IEnumerable<UserPermission>> GetAdminUserPermissionsAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/permissions/users")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<UserPermission>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a user's permissions.
    /// </summary>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="name">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateAdminUserPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["permission"] = permission,
            ["name"] = name,
        };

        var response = await GetAdminUrl("/permissions/users")
            .SetQueryParams(queryParamValues)
            .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a user's permissions.
    /// </summary>
    /// <param name="name">The username.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the permissions were removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteAdminUserPermissionsAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl("/permissions/users")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves users that currently have no admin permissions.
    /// </summary>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users without permissions.</returns>
    public async Task<IEnumerable<User>> GetAdminUserPermissionsNoneAsync(string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(System.StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetAdminUrl("/permissions/users/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves merge strategies for pull requests for a specific SCM.
    /// </summary>
    /// <param name="scmId">The SCM identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merge strategies configuration.</returns>
    public async Task<MergeStrategies> GetAdminPullRequestsMergeStrategiesAsync(string scmId, CancellationToken cancellationToken = default)
    {
        return await GetAdminUrl($"/pull-requests/{scmId}")
            .GetJsonAsync<MergeStrategies>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates merge strategies for pull requests for a specific SCM.
    /// </summary>
    /// <param name="scmId">The SCM identifier.</param>
    /// <param name="mergeStrategies">The merge strategies payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated merge strategies.</returns>
    public async Task<MergeStrategies> UpdateAdminPullRequestsMergeStrategiesAsync(string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default)
    {
        var response = await GetAdminUrl($"/pull-requests/{scmId}")
            .PostJsonAsync(mergeStrategies, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<MergeStrategies>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}