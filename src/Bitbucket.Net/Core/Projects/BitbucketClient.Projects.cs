using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Text.Json;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base projects URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the projects endpoint.</returns>
    private IFlurlRequest GetProjectsUrl() => GetBaseUrl()
        .AppendPathSegment("/projects");

    /// <summary>
    /// Gets the projects URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the projects path.</returns>
    private IFlurlRequest GetProjectsUrl(string path) => GetProjectsUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Gets the URL for a specific project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the project.</returns>
    private IFlurlRequest GetProjectUrl(string projectKey) => GetProjectsUrl()
        .AppendPathSegment($"/{projectKey}");

    /// <summary>
    /// Gets the URL for a repository within a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the repository.</returns>
    private IFlurlRequest GetProjectsReposUrl(string projectKey, string repositorySlug) => GetProjectsUrl($"/{projectKey}/repos/{repositorySlug}");

    /// <summary>
    /// Gets the URL for a specific path within a project repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="path">The additional path to append.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the repository path.</returns>
    private IFlurlRequest GetProjectsReposUrl(string projectKey, string repositorySlug, string path) => GetProjectsReposUrl(projectKey, repositorySlug)
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves projects accessible to the current user.
    /// </summary>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="name">Optional project name filter.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of projects.</returns>
    public async Task<IEnumerable<Project>> GetProjectsAsync(
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? name = null,
        Permissions? permission = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl()
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Project>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams projects accessible to the current user as they are retrieved, improving memory efficiency for large result sets.
    /// </summary>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="name">Optional project name filter.</param>
    /// <param name="permission">Optional permission filter.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of projects.</returns>
    public IAsyncEnumerable<Project> GetProjectsStreamAsync(
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? name = null,
        Permissions? permission = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl()
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Project>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
    }

    /// <summary>
    /// Creates a project.
    /// </summary>
    /// <param name="projectDefinition">The project definition.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created project.</returns>
    public async Task<Project> CreateProjectAsync(ProjectDefinition projectDefinition, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsUrl()
            .SendAsync(HttpMethod.Post, CreateJsonContent(projectDefinition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Project>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsUrl($"/{projectKey}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="projectDefinition">The updated project definition.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated project.</returns>
    public async Task<Project> UpdateProjectAsync(string projectKey, ProjectDefinition projectDefinition, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsUrl($"/{projectKey}")
            .SendAsync(HttpMethod.Put, CreateJsonContent(projectDefinition), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Project>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a project by key.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested project.</returns>
    public async Task<Project> GetProjectAsync(string projectKey, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsUrl($"/{projectKey}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Project>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves user permissions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of user permissions.</returns>
    public async Task<IEnumerable<UserPermission>> GetProjectUserPermissionsAsync(string projectKey, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/permissions/users")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<UserPermission>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a user's permissions from a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectUserPermissionsAsync(string projectKey, string userName, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["name"] = userName,
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/users")
            .SetQueryParams(queryParamValues)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a user's permissions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="userName">The user name.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateProjectUserPermissionsAsync(string projectKey, string userName, Permissions permission, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["name"] = userName,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/users")
            .SetQueryParams(queryParamValues)
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves users with no permissions on a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of licensed users without project permissions.</returns>
    public async Task<IEnumerable<LicensedUser>> GetProjectUserPermissionsNoneAsync(string projectKey, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/permissions/users/none")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<LicensedUser>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves group permissions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of group permissions.</returns>
    public async Task<IEnumerable<GroupPermission>> GetProjectGroupPermissionsAsync(string projectKey, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/permissions/groups")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<GroupPermission>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a group's permissions from a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="groupName">The group name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the group permissions were removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectGroupPermissionsAsync(string projectKey, string groupName, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["name"] = groupName,
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/groups")
            .SetQueryParams(queryParamValues)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a group's permissions for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="groupName">The group name.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateProjectGroupPermissionsAsync(string projectKey, string groupName, Permissions permission, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["name"] = groupName,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/groups")
            .SetQueryParams(queryParamValues)
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves groups that currently have no permissions on a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of licensed users representing groups without permissions.</returns>
    public async Task<IEnumerable<LicensedUser>> GetProjectGroupPermissionsNoneAsync(string projectKey, string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/permissions/groups/none")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<LicensedUser>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Checks whether a permission is granted to all users for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="permission">The permission to check.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the permission is granted to all; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsProjectDefaultPermissionAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsUrl($"/{projectKey}/permissions/{BitbucketHelpers.PermissionToString(permission)}/all")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s =>
        {
            using var doc = JsonDocument.Parse(s);
            return doc.RootElement.GetProperty("permitted").GetBoolean();
        }, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<bool> SetProjectDefaultPermissionAsync(string projectKey, Permissions permission, bool allow, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["allow"] = BitbucketHelpers.BoolToString(allow),
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/{BitbucketHelpers.PermissionToString(permission)}/all")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Post, new StringContent(string.Empty), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Grants a permission to all users for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the permission was granted; otherwise, <c>false</c>.</returns>
    public async Task<bool> GrantProjectPermissionToAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default)
    {
        return await SetProjectDefaultPermissionAsync(projectKey, permission, allow: true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Revokes a permission from all users for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="permission">The permission to revoke.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the permission was revoked; otherwise, <c>false</c>.</returns>
    public async Task<bool> RevokeProjectPermissionFromAllAsync(string projectKey, Permissions permission, CancellationToken cancellationToken = default)
    {
        return await SetProjectDefaultPermissionAsync(projectKey, permission, allow: false, cancellationToken).ConfigureAwait(false);
    }
}