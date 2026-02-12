using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.Ssh;

namespace Bitbucket.Net;

/// <summary>
/// SSH key operations.
/// </summary>
public interface ISshOperations
{
    Task<bool> DeleteProjectsReposKeysAsync(int keyId, CancellationToken cancellationToken, params string[] projectsOrRepos);
    Task<bool> DeleteProjectsReposKeysAsync(int keyId, params string[] projectsOrRepos);
    Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(int keyId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(string projectKey, string? filter = null, Permissions? permission = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<ProjectKey> CreateProjectKeyAsync(string projectKey, string keyText, Permissions permission, CancellationToken cancellationToken = default);
    Task<ProjectKey> GetProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default);
    Task<ProjectKey> UpdateProjectKeyPermissionAsync(string projectKey, int keyId, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(int keyId, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(string projectKey, string repositorySlug, string? filter = null, bool? effective = null, Permissions? permission = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<RepositoryKey> CreateRepoKeyAsync(string projectKey, string repositorySlug, string keyText, Permissions permission, CancellationToken cancellationToken = default);
    Task<RepositoryKey> GetRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default);
    Task<bool> DeleteRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default);
    Task<RepositoryKey> UpdateRepoKeyPermissionAsync(string projectKey, string repositorySlug, int keyId, Permissions permission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Key>> GetUserKeysAsync(string? userSlug = null, int? maxPages = null, int? limit = null, int? start = null, CancellationToken cancellationToken = default);
    Task<Key> CreateUserKeyAsync(string keyText, string? userSlug = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserKeysAsync(string? userSlug = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserKeyAsync(int keyId, CancellationToken cancellationToken = default);
    Task<SshSettings> GetSshSettingsAsync(CancellationToken cancellationToken = default);
}