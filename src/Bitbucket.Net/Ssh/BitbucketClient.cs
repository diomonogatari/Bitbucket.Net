using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.Ssh;
using Flurl.Http;
using System.Text;
using System.Text.Json;

namespace Bitbucket.Net;

/// <summary>
/// Provides SSH key management Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base SSH keys URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the keys root.</returns>
    private IFlurlRequest GetKeysUrl() => GetBaseUrl("/keys");

    /// <summary>
    /// Gets the SSH keys URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the keys root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the keys path.</returns>
    private IFlurlRequest GetKeysUrl(string path) => GetKeysUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Gets the base SSH URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the SSH root.</returns>
    private IFlurlRequest GetSshUrl() => GetBaseUrl("/ssh");

    /// <summary>
    /// Gets the SSH URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the SSH root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the SSH path.</returns>
    private IFlurlRequest GetSshUrl(string path) => GetSshUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Deletes an SSH key from multiple projects or repositories.
    /// </summary>
    /// <param name="keyId">The SSH key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <param name="projectsOrRepos">Project or repository identifiers.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectsReposKeysAsync(int keyId, CancellationToken cancellationToken, params string[] projectsOrRepos)
    {
        var json = JsonSerializer.Serialize(projectsOrRepos, s_writeJsonOptions);
        var response = await GetKeysUrl($"/ssh/{keyId}")
            .WithHeader("Content-Type", "application/json")
            .SendAsync(HttpMethod.Delete, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes an SSH key from multiple projects or repositories using the default cancellation token.
    /// </summary>
    /// <param name="keyId">The SSH key identifier.</param>
    /// <param name="projectsOrRepos">Project or repository identifiers.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectsReposKeysAsync(int keyId, params string[] projectsOrRepos)
    {
        return await DeleteProjectsReposKeysAsync(keyId, default, projectsOrRepos).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves project keys associated with an SSH key identifier.
    /// </summary>
    /// <param name="keyId">The SSH key identifier.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of project keys.</returns>
    public Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(int keyId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedAsync<ProjectKey>(
            GetKeysUrl($"/ssh/{keyId}/projects"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves project SSH keys within a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="filter">Optional filter for search.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of project keys.</returns>
    public Task<IReadOnlyList<ProjectKey>> GetProjectKeysAsync(string projectKey,
        string? filter = null,
        Permissions? permission = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return GetPagedAsync<ProjectKey>(
            GetKeysUrl($"/projects/{projectKey}/ssh"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates an SSH key for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="keyText">The public key text.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created project key.</returns>
    public async Task<ProjectKey> CreateProjectKeyAsync(string projectKey, string keyText, Permissions permission, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyText);

        var data = new
        {
            key = new { text = keyText },
            permission = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetKeysUrl($"/projects/{projectKey}/ssh")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific project SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested project key.</returns>
    public async Task<ProjectKey> GetProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a project SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the key was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the permission of a project SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="permission">The permission to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated project key.</returns>
    public async Task<ProjectKey> UpdateProjectKeyPermissionAsync(string projectKey, int keyId, Permissions permission, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}/permissions/{BitbucketHelpers.PermissionToString(permission)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves repository keys associated with an SSH key identifier.
    /// </summary>
    /// <param name="keyId">The SSH key identifier.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of repository keys.</returns>
    public Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(int keyId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedAsync<RepositoryKey>(
            GetKeysUrl($"/ssh/{keyId}/repos"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves repository SSH keys within a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filter">Optional filter for search.</param>
    /// <param name="effective">Whether to include effective permissions.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of repository keys.</returns>
    public Task<IReadOnlyList<RepositoryKey>> GetRepoKeysAsync(string projectKey, string repositorySlug,
        string? filter = null,
        bool? effective = null,
        Permissions? permission = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["effective"] = BitbucketHelpers.BoolToString(effective),
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return GetPagedAsync<RepositoryKey>(
            GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates an SSH key for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="keyText">The public key text.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created repository key.</returns>
    public async Task<RepositoryKey> CreateRepoKeyAsync(string projectKey, string repositorySlug, string keyText, Permissions permission, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);
        ArgumentException.ThrowIfNullOrWhiteSpace(keyText);

        var data = new
        {
            key = new { text = keyText },
            permission = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh")
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific repository SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested repository key.</returns>
    public async Task<RepositoryKey> GetRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a repository SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the key was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the permission of a repository SSH key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="permission">The permission to apply.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated repository key.</returns>
    public async Task<RepositoryKey> UpdateRepoKeyPermissionAsync(string projectKey, string repositorySlug, int keyId, Permissions permission, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}/permissions/{BitbucketHelpers.PermissionToString(permission)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves SSH keys for a user.
    /// </summary>
    /// <param name="userSlug">Optional user slug. If null, retrieves keys for the current user.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of SSH keys.</returns>
    public Task<IReadOnlyList<Key>> GetUserKeysAsync(string? userSlug = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["user"] = userSlug,
        };

        return GetPagedAsync<Key>(
            GetSshUrl("/keys"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates an SSH key for a user.
    /// </summary>
    /// <param name="keyText">The public key text.</param>
    /// <param name="userSlug">Optional user slug. If null, applies to the current user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created SSH key.</returns>
    public async Task<Key> CreateUserKeyAsync(string keyText, string? userSlug = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyText);

        var response = await GetSshUrl("/keys")
            .SetQueryParam("user", userSlug)
            .SendAsync(HttpMethod.Post, CreateJsonContent(new { text = keyText }), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Key>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes all SSH keys for a user.
    /// </summary>
    /// <param name="userSlug">Optional user slug. If null, deletes keys for the current user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if keys were deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteUserKeysAsync(string? userSlug = null, CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl("/keys")
            .SetQueryParam("user", userSlug)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a specific SSH key for the current user.
    /// </summary>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the key was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteUserKeyAsync(int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl($"/keys/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves SSH settings.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The SSH settings.</returns>
    public async Task<SshSettings> GetSshSettingsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl("/settings")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<SshSettings>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}