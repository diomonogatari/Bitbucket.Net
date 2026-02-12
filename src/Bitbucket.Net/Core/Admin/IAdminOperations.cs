using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Users;

namespace Bitbucket.Net;

/// <summary>
/// Administration operations.
/// </summary>
public interface IAdminOperations
{
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminGroupsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<DeletableGroupOrUser> CreateAdminGroupAsync(string name, CancellationToken cancellationToken = default);
    Task<DeletableGroupOrUser> DeleteAdminGroupAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> AddAdminGroupUsersAsync(GroupUsers groupUsers, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminGroupMoreMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminGroupMoreNonMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserInfo>> GetAdminUsersAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> CreateAdminUserAsync(string name, string password, string displayName, string emailAddress, bool addToDefaultGroup = true, string notify = "false", CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateAdminUserAsync(string? name = null, string? displayName = null, string? emailAddress = null, CancellationToken cancellationToken = default);
    Task<UserInfo> DeleteAdminUserAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> AddAdminUserGroupsAsync(UserGroups userGroups, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminUserCaptcha(string name, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminUserCredentialsAsync(Models.Core.Admin.PasswordChange passwordChange, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminUserMoreMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminUserMoreNonMembersAsync(string context, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> RemoveAdminUserFromGroupAsync(string userName, string groupName, CancellationToken cancellationToken = default);
    Task<UserInfo> RenameAdminUserAsync(UserRename userRename, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<Cluster> GetAdminClusterAsync(CancellationToken cancellationToken = default);
    Task<LicenseDetails> GetAdminLicenseAsync(CancellationToken cancellationToken = default);
    Task<LicenseDetails> UpdateAdminLicenseAsync(LicenseInfo licenseInfo, CancellationToken cancellationToken = default);
    Task<MailServerConfiguration> GetAdminMailServerAsync(CancellationToken cancellationToken = default);
    Task<MailServerConfiguration> UpdateAdminMailServerAsync(MailServerConfiguration mailServerConfiguration, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminMailServerAsync(CancellationToken cancellationToken = default);
    Task<string> GetAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default);
    Task<string> UpdateAdminMailServerSenderAddressAsync(string senderAddress, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminMailServerSenderAddressAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetAdminGroupPermissionsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminGroupPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminGroupPermissionsAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeletableGroupOrUser>> GetAdminGroupPermissionsNoneAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetAdminUserPermissionsAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateAdminUserPermissionsAsync(Permissions permission, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteAdminUserPermissionsAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAdminUserPermissionsNoneAsync(string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<MergeStrategies> GetAdminPullRequestsMergeStrategiesAsync(string scmId, CancellationToken cancellationToken = default);
    Task<MergeStrategies> UpdateAdminPullRequestsMergeStrategiesAsync(string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default);
}