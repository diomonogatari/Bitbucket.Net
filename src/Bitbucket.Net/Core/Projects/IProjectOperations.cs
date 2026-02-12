using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;

namespace Bitbucket.Net;

/// <summary>
/// Project management operations.
/// </summary>
public interface IProjectOperations
{
    Task<IReadOnlyList<Project>> GetProjectsAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, Permissions? permission = null, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Project> GetProjectsStreamAsync(int? maxPages = null, int? limit = null, int? start = null, string? name = null, Permissions? permission = null, CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<Project> UpdateProjectAsync(string projectKey, UpdateProjectRequest request, CancellationToken cancellationToken = default);
    Task<Project> GetProjectAsync(string projectKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserPermission>> GetProjectUserPermissionsAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, int? avatarSize = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectUserPermissionsAsync(string projectKey, string userName, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectUserPermissionsAsync(string projectKey, string userName, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LicensedUser>> GetProjectUserPermissionsNoneAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPermission>> GetProjectGroupPermissionsAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectGroupPermissionsAsync(string projectKey, string groupName, CancellationToken cancellationToken = default);
    Task<bool> UpdateProjectGroupPermissionsAsync(string projectKey, string groupName, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LicensedUser>> GetProjectGroupPermissionsNoneAsync(string projectKey, string? filter = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<bool> IsProjectDefaultPermissionAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);
    Task<bool> GrantProjectPermissionToAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);
    Task<bool> RevokeProjectPermissionFromAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default);
}