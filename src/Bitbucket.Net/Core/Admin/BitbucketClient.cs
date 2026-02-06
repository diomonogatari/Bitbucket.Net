using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;
using PasswordChange = Bitbucket.Net.Models.Core.Admin.PasswordChange;

namespace Bitbucket.Net
{
    public partial class BitbucketClient
    {
        private IFlurlRequest GetAdminUrl() => GetBaseUrl()
            .AppendPathSegment("/admin");

        private IFlurlRequest GetAdminUrl(string path) => GetAdminUrl()
            .AppendPathSegment(path);

        public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminGroupsAsync(string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["filter"] = filter
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/groups")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<DeletableGroupOrUser> CreateAdminGroupAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/groups")
                .SetQueryParam("name", name)
                .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<DeletableGroupOrUser>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<DeletableGroupOrUser> DeleteAdminGroupAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/groups")
                .SetQueryParam("name", name)
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<DeletableGroupOrUser>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> AddAdminGroupUsersAsync(GroupUsers groupUsers, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/groups/add-users")
                .PostJsonAsync(groupUsers, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserInfo>> GetAdminGroupMoreMembersAsync(string context, string? filter = null,
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
                ["context"] = context,
                ["filter"] = filter,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/groups/more-members")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserInfo>> GetAdminGroupMoreNonMembersAsync(string context, string? filter = null,
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
                ["context"] = context,
                ["filter"] = filter,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/groups/more-non-members")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserInfo>> GetAdminUsersAsync(string? filter = null,
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
                ["filter"] = filter,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/users")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<UserInfo>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> CreateAdminUserAsync(string name, string password, string displayName, string emailAddress,
            bool addToDefaultGroup = true, string notify = "false", CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["name"] = name,
                ["password"] = password,
                ["displayName"] = displayName,
                ["emailAddress"] = emailAddress,
                ["addToDefaultGroup"] = BitbucketHelpers.BoolToString(addToDefaultGroup),
                ["notify"] = notify
            };

            var response = await GetAdminUrl("/users")
                .SetQueryParams(queryParamValues)
                .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserInfo> UpdateAdminUserAsync(string? name = null, string? displayName = null, string? emailAddress = null, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                name,
                displayName,
                email = emailAddress
            };

            var response = await GetAdminUrl("/users")
                .PutJsonAsync(data, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserInfo> DeleteAdminUserAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/users")
                .SetQueryParam("name", name)
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> AddAdminUserGroupsAsync(UserGroups userGroups, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/users/add-groups")
                .PostJsonAsync(userGroups, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdminUserCaptcha(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/users/captcha")
                .SetQueryParam("name", name)
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> UpdateAdminUserCredentialsAsync(PasswordChange passwordChange, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/users/credentials")
                .PutJsonAsync(passwordChange, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminUserMoreMembersAsync(string context, string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["context"] = context,
                ["filter"] = filter
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/users/more-members")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminUserMoreNonMembersAsync(string context, string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["context"] = context,
                ["filter"] = filter
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/users/more-non-members")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> RemoveAdminUserFromGroupAsync(string userName, string groupName, CancellationToken cancellationToken = default)
        {
            var data = new
            {
                context = userName,
                itemName = groupName
            };

            var response = await GetAdminUrl("/users/remove-group")
                .PostJsonAsync(data, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserInfo> RenameAdminUserAsync(UserRename userRename, int? avatarSize = null, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("users/rename")
	            .SetQueryParam("avatarSize", avatarSize)
                .PostJsonAsync(userRename, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<UserInfo>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<Cluster> GetAdminClusterAsync(CancellationToken cancellationToken = default)
        {
            return await GetAdminUrl("/cluster")
                .GetJsonAsync<Cluster>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<LicenseDetails> GetAdminLicenseAsync(CancellationToken cancellationToken = default)
        {
            return await GetAdminUrl("/license")
                .GetJsonAsync<LicenseDetails>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<LicenseDetails> UpdateAdminLicenseAsync(LicenseInfo licenseInfo, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/license")
                .PostJsonAsync(licenseInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<LicenseDetails>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<MailServerConfiguration> GetAdminMailServerAsync(CancellationToken cancellationToken = default)
        {
            return await GetAdminUrl("/mail-server")
                .GetJsonAsync<MailServerConfiguration>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<MailServerConfiguration> UpdateAdminMailServerAsync(MailServerConfiguration mailServerConfiguration, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/mail-server")
                .PutJsonAsync(mailServerConfiguration, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<MailServerConfiguration>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdminMailServerAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/mail-server")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> GetAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/mail-server/sender-address")
                .GetAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, s => s, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> UpdateAdminMailServerSenderAddressAsync(string senderAddress, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/mail-server/sender-address")
                .PutJsonAsync(senderAddress, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, s => s, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/mail-server/sender-address")
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupPermission>> GetAdminGroupPermissionsAsync(string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["filter"] = filter
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/permissions/groups")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<GroupPermission>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> UpdateAdminGroupPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["permission"] = permission,
                ["name"] = name
            };

            var response = await GetAdminUrl("/permissions/groups")
                .SetQueryParams(queryParamValues)
                .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdminGroupPermissionsAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/permissions/groups")
                .SetQueryParam("name", name)
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DeletableGroupOrUser>> GetAdminGroupPermissionsNoneAsync(string? filter = null,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["limit"] = limit,
                ["start"] = start,
                ["filter"] = filter
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/permissions/groups/none")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<UserPermission>> GetAdminUserPermissionsAsync(string? filter = null,
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
                ["filter"] = filter,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/permissions/users")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<UserPermission>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> UpdateAdminUserPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default)
        {
            var queryParamValues = new Dictionary<string, object?>
            {
                ["permission"] = permission,
                ["name"] = name
            };

            var response = await GetAdminUrl("/permissions/users")
                .SetQueryParams(queryParamValues)
                .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> DeleteAdminUserPermissionsAsync(string name, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl("/permissions/users")
                .SetQueryParam("name", name)
                .DeleteAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<User>> GetAdminUserPermissionsNoneAsync(string? filter = null,
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
                ["filter"] = filter,
                ["avatarSize"] = avatarSize
            };

            return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                    await GetAdminUrl("/permissions/users/none")
                        .SetQueryParams(qpv)
                        .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                        .ConfigureAwait(false), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<MergeStrategies> GetAdminPullRequestsMergeStrategiesAsync(string scmId, CancellationToken cancellationToken = default)
        {
            return await GetAdminUrl($"/pull-requests/{scmId}")
                .GetJsonAsync<MergeStrategies>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<MergeStrategies> UpdateAdminPullRequestsMergeStrategiesAsync(string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default)
        {
            var response = await GetAdminUrl($"/pull-requests/{scmId}")
                .PostJsonAsync(mergeStrategies, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<MergeStrategies>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
