using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Tasks;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

/// <summary>
/// Provides project and repository management Bitbucket API operations.
/// </summary>
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl()
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Project>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["name"] = name,
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl()
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Project>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken);
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
            .PostJsonAsync(projectDefinition, cancellationToken: cancellationToken)
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
            .PutJsonAsync(projectDefinition, cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl($"/{projectKey}/permissions/users")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<UserPermission>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsUrl($"/{projectKey}/permissions/users/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<LicensedUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl($"/{projectKey}/permissions/groups")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<GroupPermission>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsUrl($"/{projectKey}/permissions/groups/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<LicensedUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["allow"] = BitbucketHelpers.BoolToString(allow),
        };

        var response = await GetProjectsUrl($"/{projectKey}/permissions/{BitbucketHelpers.PermissionToString(permission)}/all")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
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

    /// <summary>
    /// Retrieves repositories for a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of repositories.</returns>
    public async Task<IEnumerable<Repository>> GetProjectRepositoriesAsync(string projectKey,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl($"/{projectKey}/repos")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Repository>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all repositories for a project as an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    public IAsyncEnumerable<Repository> GetProjectRepositoriesStreamAsync(string projectKey,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsUrl($"/{projectKey}/repos")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Repository>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Creates a repository within a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositoryName">The repository name.</param>
    /// <param name="scmId">Optional SCM identifier (default is git).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created repository.</returns>
    public async Task<Repository> CreateProjectRepositoryAsync(string projectKey, string repositoryName, string scmId = "git", CancellationToken cancellationToken = default)
    {
        var data = new
        {
            name = repositoryName,
            scmId,
        };

        var response = await GetProjectsUrl($"/{projectKey}/repos")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Repository>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a repository within a project.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested repository.</returns>
    public async Task<Repository> GetProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .GetJsonAsync<Repository>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a fork of a repository, optionally targeting another project or slug.
    /// </summary>
    /// <param name="projectKey">The source project key.</param>
    /// <param name="repositorySlug">The source repository slug.</param>
    /// <param name="targetProjectKey">Optional target project key for the fork.</param>
    /// <param name="targetSlug">Optional target repository slug.</param>
    /// <param name="targetName">Optional display name for the fork.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created repository fork.</returns>
    public async Task<RepositoryFork> CreateProjectRepositoryForkAsync(string projectKey, string repositorySlug, string? targetProjectKey = null, string? targetSlug = null, string? targetName = null, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            slug = targetSlug ?? repositorySlug,
            name = targetName,
            project = targetProjectKey == null ? null : new ProjectRef { Key = targetProjectKey },
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryFork>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Schedules a repository for deletion.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the repository was scheduled for deletion; otherwise, <c>false</c>.</returns>
    public async Task<bool> ScheduleProjectRepositoryForDeletionAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates repository metadata such as name, forkability, project, or visibility.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="targetName">Optional new repository name.</param>
    /// <param name="isForkable">Optional forkable flag.</param>
    /// <param name="targetProjectKey">Optional target project key.</param>
    /// <param name="isPublic">Optional public visibility flag.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated repository.</returns>
    public async Task<Repository> UpdateProjectRepositoryAsync(string projectKey, string repositorySlug,
        string? targetName = null,
        bool? isForkable = null,
        string? targetProjectKey = null,
        bool? isPublic = null,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            name = targetName,
            forkable = isForkable,
            project = targetProjectKey == null ? null : new ProjectRef { Key = targetProjectKey },
            @public = isPublic,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .PutJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Repository>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves forks of a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of repository forks.</returns>
    public async Task<IEnumerable<RepositoryFork>> GetProjectRepositoryForksAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/forks")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<RepositoryFork>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Recreates a repository in place.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The recreated repository.</returns>
    public async Task<Repository> RecreateProjectRepositoryAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/recreate")
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Repository>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves repositories related to the specified repository (e.g., forks).
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of related repository forks.</returns>
    public async Task<IEnumerable<RepositoryFork>> GetRelatedProjectRepositoriesAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/related")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<RepositoryFork>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves an archive (zip/tar) of a repository at a specific ref.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="at">The ref (commit/branch/tag) to archive.</param>
    /// <param name="fileName">The archive file name.</param>
    /// <param name="archiveFormat">The archive format.</param>
    /// <param name="path">Optional path filter.</param>
    /// <param name="prefix">Optional archive prefix.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Archive bytes.</returns>
    public async Task<byte[]> GetProjectRepositoryArchiveAsync(string projectKey, string repositorySlug,
        string at,
        string fileName,
        ArchiveFormats archiveFormat,
        string path,
        string prefix,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["fileName"] = fileName,
            ["format"] = BitbucketHelpers.ArchiveFormatToString(archiveFormat),
            ["path"] = path,
            ["prefix"] = prefix,
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, "/archive")
            .SetQueryParams(queryParamValues)
            .GetBytesAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves group permissions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of group permissions.</returns>
    public async Task<IEnumerable<GroupPermission>> GetProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug,
        string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<GroupPermission>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a group's permissions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["name"] = name,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups")
            .SetQueryParams(queryParamValues)
            .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a group's permissions from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="name">The group name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the permissions were removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectRepositoryGroupPermissionsAsync(string projectKey, string repositorySlug, string name, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups")
            .SetQueryParam("name", name)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves groups or users without permissions on a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of removable group or user entries.</returns>
    public async Task<IEnumerable<DeletableGroupOrUser>> GetProjectRepositoryGroupPermissionsNoneAsync(string projectKey, string repositorySlug,
        string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<DeletableGroupOrUser>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves user permissions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of user permissions.</returns>
    public async Task<IEnumerable<UserPermission>> GetProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug,
        string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<UserPermission>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a user's permissions for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="permission">The permission to grant.</param>
    /// <param name="name">The user name.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the update succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UpdateProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, Permissions permission, string name, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["name"] = name,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users")
            .SetQueryParams(queryParamValues)
            .PutJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a user's permissions from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="name">The user name.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns><c>true</c> if the permissions were removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectRepositoryUserPermissionsAsync(string projectKey, string repositorySlug, string name,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users")
            .SetQueryParam("name", name)
            .SetQueryParam("avatarSize", avatarSize)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves users who have no permissions on a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of users without repository permissions.</returns>
    public async Task<IEnumerable<User>> GetProjectRepositoryUserPermissionsNoneAsync(string projectKey, string repositorySlug,
        string? filter = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users/none")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<User>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves branches for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="baseBranchOrTag">Optional base branch or tag filter.</param>
    /// <param name="details">Whether to include additional details.</param>
    /// <param name="filterText">Optional branch name filter.</param>
    /// <param name="orderBy">Optional branch ordering.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of branches.</returns>
    public async Task<IEnumerable<Branch>> GetBranchesAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? baseBranchOrTag = null,
        bool? details = null,
        string? filterText = null,
        BranchOrderBy? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["base"] = baseBranchOrTag,
            ["details"] = details.HasValue ? BitbucketHelpers.BoolToString(details.Value) : null,
            ["filterText"] = filterText,
            ["orderBy"] = orderBy.HasValue ? BitbucketHelpers.BranchOrderByToString(orderBy.Value) : null,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsReposUrl(projectKey, repositorySlug, "/branches")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Branch>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all branches for a repository as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<Branch> GetBranchesStreamAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? baseBranchOrTag = null,
        bool? details = null,
        string? filterText = null,
        BranchOrderBy? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["base"] = baseBranchOrTag,
            ["details"] = details.HasValue ? BitbucketHelpers.BoolToString(details.Value) : null,
            ["filterText"] = filterText,
            ["orderBy"] = orderBy.HasValue ? BitbucketHelpers.BranchOrderByToString(orderBy.Value) : null,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsReposUrl(projectKey, repositorySlug, "/branches")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<Branch>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Creates a branch in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="branchInfo">The branch information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created branch.</returns>
    public async Task<Branch> CreateBranchAsync(string projectKey, string repositorySlug, BranchInfo branchInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/branches")
            .PostJsonAsync(branchInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Branch>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the default branch for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The default branch.</returns>
    public async Task<Branch> GetDefaultBranchAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, "/branches/default")
            .GetJsonAsync<Branch>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sets the default branch for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="branchRef">The target branch reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the default branch was updated; otherwise, <c>false</c>.</returns>
    public async Task<bool> SetDefaultBranchAsync(string projectKey, string repositorySlug, BranchRef branchRef, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/branches")
            .PutJsonAsync(branchRef, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Browses repository content at a specific ref.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="at">The ref (branch, tag, or commit).</param>
    /// <param name="type">Whether to include type information.</param>
    /// <param name="blame">Whether to include blame metadata.</param>
    /// <param name="noContent">If true and blame is requested, omit file content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The browsed item metadata.</returns>
    public async Task<BrowseItem> BrowseProjectRepositoryAsync(string projectKey, string repositorySlug, string at, bool type = false,
        bool blame = false,
        bool noContent = false,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["type"] = BitbucketHelpers.BoolToString(type),
        };
        if (blame)
        {
            queryParamValues.Add("blame", value: null);
        }
        if (blame && noContent)
        {
            queryParamValues.Add("noContent", value: null);
        }

        return await GetProjectsReposUrl(projectKey, repositorySlug, "/browse")
            .SetQueryParams(queryParamValues, Flurl.NullValueHandling.NameOnly)
            .GetJsonAsync<BrowseItem>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Browses a specific path within a repository at a given ref.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="path">The path to browse.</param>
    /// <param name="at">The ref (branch, tag, or commit).</param>
    /// <param name="type">Whether to include type information.</param>
    /// <param name="blame">Whether to include blame metadata.</param>
    /// <param name="noContent">If true and blame is requested, omit file content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The browsed path item metadata.</returns>
    public async Task<BrowsePathItem> BrowseProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path, string at, bool type = false,
        bool blame = false,
        bool noContent = false,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["type"] = BitbucketHelpers.BoolToString(type),
        };
        if (blame)
        {
            queryParamValues.Add("blame", value: null);
        }
        if (blame && noContent)
        {
            queryParamValues.Add("noContent", value: null);
        }

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/browse/{path}")
            .SetQueryParams(queryParamValues, Flurl.NullValueHandling.NameOnly)
            .GetJsonAsync<BrowsePathItem>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the raw content of a file as a stream. This is optimal for large files as it doesn't buffer the entire content in memory.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="path">The file path within the repository.</param>
    /// <param name="at">Optional ref (branch, tag, or commit) to get the file content at. Defaults to default branch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the raw file content. Caller is responsible for disposing.</returns>
    public async Task<Stream> GetRawFileContentStreamAsync(string projectKey, string repositorySlug, string path,
        string? at = null,
        CancellationToken cancellationToken = default)
    {
        var request = GetProjectsReposUrl(projectKey, repositorySlug, $"/raw/{path}");

        if (!string.IsNullOrEmpty(at))
        {
            request = request.SetQueryParam("at", at);
        }

        return await request
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams the raw content of a file line by line. This is optimal for large text files.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="path">The file path within the repository.</param>
    /// <param name="at">Optional ref (branch, tag, or commit) to get the file content at. Defaults to default branch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of lines from the file.</returns>
    public async IAsyncEnumerable<string> GetRawFileContentLinesStreamAsync(string projectKey, string repositorySlug, string path,
        string? at = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var stream = await GetRawFileContentStreamAsync(projectKey, repositorySlug, path, at, cancellationToken).ConfigureAwait(false);

        await using (stream.ConfigureAwait(false))
        {
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (line is not null)
                {
                    yield return line;
                }
            }
        }
    }

    /// <summary>
    /// Updates a file at the specified path in the repository.
    /// Uses ArrayPool&lt;byte&gt; for zero-copy buffer management to minimize heap allocations.
    /// </summary>
    public async Task<Commit> UpdateProjectRepositoryPathAsync(string projectKey, string repositorySlug, string path,
        string fileName,
        string branch,
        string? message = null,
        string? sourceCommitId = null,
        string? sourceBranch = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileName))
        {
            throw new ArgumentException($"File doesn't exist: {fileName}", nameof(fileName));
        }

        var fileInfo = new FileInfo(fileName);
        int fileSize = checked((int)fileInfo.Length);

        // Use ArrayPool to rent a buffer instead of allocating new array
        byte[] buffer = ArrayPool<byte>.Shared.Rent(fileSize);
        try
        {
            int bytesRead;
            var stm = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            await using (stm.ConfigureAwait(false))
            {
                bytesRead = await stm.ReadAsync(buffer.AsMemory(0, fileSize), cancellationToken).ConfigureAwait(false);
            }

            // Create MemoryStream over the exact bytes read (not the rented buffer size)
            using var memoryStream = new MemoryStream(buffer, 0, bytesRead, writable: false);

            var data = new DynamicMultipartFormDataContent
            {
                { new StreamContent(memoryStream), "content" },
                { new StringContent(branch), "branch" },
                { message, message == null ? null : new StringContent(message), "message" },
                { sourceCommitId, sourceCommitId == null ? null : new StringContent(sourceCommitId), "sourceCommitId" },
                { sourceBranch, sourceBranch == null ? null : new StringContent(sourceBranch), "sourceBranch" },
            };

            var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/browse/{path}")
                .PutAsync(data.ToMultipartFormDataContent(), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return await HandleResponseAsync<Commit>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Always return the buffer to the pool
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Retrieves changes for a repository between two refs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The target ref.</param>
    /// <param name="since">Optional starting ref.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes.</returns>
    public async Task<IEnumerable<Change>> GetChangesAsync(string projectKey, string repositorySlug, string until, string? since = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["since"] = since,
            ["until"] = until,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/changes")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Change>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves commits for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The ref to retrieve commits until.</param>
    /// <param name="followRenames">Whether to follow renames.</param>
    /// <param name="ignoreMissing">Whether to ignore missing commits.</param>
    /// <param name="merges">Merge commit inclusion policy.</param>
    /// <param name="path">Optional path filter.</param>
    /// <param name="since">Optional starting ref.</param>
    /// <param name="withCounts">Whether to include commit counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of commits.</returns>
    public async Task<IEnumerable<Commit>> GetCommitsAsync(string projectKey, string repositorySlug,
        string until,
        bool followRenames = false,
        bool ignoreMissing = false,
        MergeCommits merges = MergeCommits.Exclude,
        string? path = null,
        string? since = null,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["followRenames"] = BitbucketHelpers.BoolToString(followRenames),
            ["ignoreMissing"] = BitbucketHelpers.BoolToString(ignoreMissing),
            ["merges"] = BitbucketHelpers.MergeCommitsToString(merges),
            ["path"] = path,
            ["since"] = since,
            ["until"] = until,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/commits")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Commit>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all commits for a repository as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<Commit> GetCommitsStreamAsync(string projectKey, string repositorySlug,
        string until,
        bool followRenames = false,
        bool ignoreMissing = false,
        MergeCommits merges = MergeCommits.Exclude,
        string? path = null,
        string? since = null,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["followRenames"] = BitbucketHelpers.BoolToString(followRenames),
            ["ignoreMissing"] = BitbucketHelpers.BoolToString(ignoreMissing),
            ["merges"] = BitbucketHelpers.MergeCommitsToString(merges),
            ["path"] = path,
            ["since"] = since,
            ["until"] = until,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/commits")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Commit>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Retrieves a commit by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="path">Optional path filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested commit.</returns>
    public async Task<Commit> GetCommitAsync(string projectKey, string repositorySlug, string commitId, string? path = null, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["path"] = path,
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Commit>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the list of file changes for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="since">Optional starting commit ID.</param>
    /// <param name="withComments">Whether to include comment counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes.</returns>
    public async Task<IEnumerable<Change>> GetCommitChangesAsync(string projectKey, string repositorySlug, string commitId,
        string? since = null,
        bool withComments = true,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["since"] = since,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/changes")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Change>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves comments for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="path">The file path within the commit.</param>
    /// <param name="since">Optional starting comment ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of comments.</returns>
    public async Task<IEnumerable<Comment>> GetCommitCommentsAsync(string projectKey, string repositorySlug, string commitId,
        string path,
        string? since = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["path"] = path,
            ["since"] = since,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Comment>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a comment on a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentInfo">The comment payload.</param>
    /// <param name="since">Optional starting comment ID for context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment reference.</returns>
    public async Task<CommentRef> CreateCommitCommentAsync(string projectKey, string repositorySlug, string commitId,
        CommentInfo commentInfo, string? since = null, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["since"] = since,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(commentInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific commit comment by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested comment reference.</returns>
    public async Task<CommentRef> GetCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetJsonAsync<CommentRef>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the text of a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="commentText">The updated comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated comment reference.</returns>
    public async Task<CommentRef> UpdateCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        CommentText commentText, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .PutJsonAsync(commentText, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a commit comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">Optional comment version for concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteCommitCommentAsync(string projectKey, string repositorySlug, string commitId, long commentId,
        int version = -1,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/comments/{commentId}")
            .SetQueryParams(queryParamValues)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a diff for a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="autoSrcPath">Whether to auto-detect source path.</param>
    /// <param name="contextLines">Context lines to include.</param>
    /// <param name="since">Optional since commit.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="whitespace">Whitespace handling strategy.</param>
    /// <param name="withComments">Whether to include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The diff result.</returns>
    public async Task<Differences> GetCommitDiffAsync(string projectKey, string repositorySlug, string commitId,
        bool autoSrcPath = false,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["autoSrcPath"] = BitbucketHelpers.BoolToString(autoSrcPath),
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["whitespace"] = whitespace,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/diff")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Differences>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams the diff for a specific commit, yielding individual diff entries as they are parsed.
    /// This is more memory-efficient for large diffs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="autoSrcPath">Auto source path.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="since">Since commit.</param>
    /// <param name="srcPath">Source path filter.</param>
    /// <param name="whitespace">Whitespace handling.</param>
    /// <param name="withComments">Include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of diffs.</returns>
    public async IAsyncEnumerable<Diff> GetCommitDiffStreamAsync(string projectKey, string repositorySlug, string commitId,
        bool autoSrcPath = false,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["autoSrcPath"] = BitbucketHelpers.BoolToString(autoSrcPath),
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["whitespace"] = whitespace,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        var responseStream = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/diff")
            .SetQueryParams(queryParamValues)
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            await foreach (var diff in DeserializeDiffsFromStreamAsync(responseStream, cancellationToken).ConfigureAwait(false))
            {
                yield return diff;
            }
        }
    }

    /// <summary>
    /// Starts watching a commit for notifications.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if watch was created; otherwise, <c>false</c>.</returns>
    public async Task<bool> CreateCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/watch")
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Stops watching a commit.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="commitId">The commit ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteCommitWatchAsync(string projectKey, string repositorySlug, string commitId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/commits/{commitId}/watch")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Compares two refs and returns the list of changes.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="from">The source ref.</param>
    /// <param name="to">The target ref.</param>
    /// <param name="fromRepo">Optional source repository key for cross-repo compare.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes between the refs.</returns>
    public async Task<IEnumerable<Change>> GetRepositoryCompareChangesAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/changes")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Change>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Compares two refs and returns a diff.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="from">The source ref.</param>
    /// <param name="to">The target ref.</param>
    /// <param name="fromRepo">Optional source repository key for cross-repo compare.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="whitespace">Whitespace handling strategy.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The diff between the refs.</returns>
    public async Task<Differences> GetRepositoryCompareDiffAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        string? srcPath = null,
        int contextLines = -1,
        string whitespace = "ignore-all",
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
            ["srcPath"] = srcPath,
            ["contextLines"] = contextLines,
            ["whitespace"] = whitespace,
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/diff")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Differences>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams the compare diff between two refs, yielding individual diff entries as they are parsed.
    /// This is more memory-efficient for large diffs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="from">The source ref (branch, tag, or commit).</param>
    /// <param name="to">The target ref (branch, tag, or commit).</param>
    /// <param name="fromRepo">Optional source repository if comparing across forks.</param>
    /// <param name="srcPath">Source path filter.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="whitespace">Whitespace handling.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of diffs.</returns>
    public async IAsyncEnumerable<Diff> GetRepositoryCompareDiffStreamAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        string? srcPath = null,
        int contextLines = -1,
        string whitespace = "ignore-all",
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
            ["srcPath"] = srcPath,
            ["contextLines"] = contextLines,
            ["whitespace"] = whitespace,
        };

        var responseStream = await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/diff")
            .SetQueryParams(queryParamValues)
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            await foreach (var diff in DeserializeDiffsFromStreamAsync(responseStream, cancellationToken).ConfigureAwait(false))
            {
                yield return diff;
            }
        }
    }

    /// <summary>
    /// Compares two refs and returns the commits between them.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="from">The source ref.</param>
    /// <param name="to">The target ref.</param>
    /// <param name="fromRepo">Optional source repository key for cross-repo compare.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of commits between the refs.</returns>
    public async Task<IEnumerable<Commit>> GetRepositoryCompareCommitsAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/commits")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Commit>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a repository diff between two commits.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The target commit ID.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="since">Optional starting commit ID.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="whitespace">Whitespace handling strategy.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The diff result.</returns>
    public async Task<Differences> GetRepositoryDiffAsync(string projectKey, string repositorySlug, string until,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["until"] = until,
            ["whitespace"] = whitespace,
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, "/diff")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Differences>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams the repository diff, yielding individual diff entries as they are parsed.
    /// This is more memory-efficient for large diffs.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="until">The commit ID to diff until.</param>
    /// <param name="contextLines">Number of context lines.</param>
    /// <param name="since">The commit ID to diff since.</param>
    /// <param name="srcPath">Source path filter.</param>
    /// <param name="whitespace">Whitespace handling.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of diffs.</returns>
    public async IAsyncEnumerable<Diff> GetRepositoryDiffStreamAsync(string projectKey, string repositorySlug, string until,
        int contextLines = -1,
        string? since = null,
        string? srcPath = null,
        string whitespace = "ignore-all",
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["until"] = until,
            ["whitespace"] = whitespace,
        };

        var responseStream = await GetProjectsReposUrl(projectKey, repositorySlug, "/diff")
            .SetQueryParams(queryParamValues)
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        await using (responseStream.ConfigureAwait(false))
        {
            await foreach (var diff in DeserializeDiffsFromStreamAsync(responseStream, cancellationToken).ConfigureAwait(false))
            {
                yield return diff;
            }
        }
    }

    /// <summary>
    /// Retrieves file paths in a repository at the specified ref.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="at">Optional ref (branch, tag, commit).</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of file paths.</returns>
    public async Task<IEnumerable<string>> GetRepositoryFilesAsync(string projectKey, string repositorySlug, string? at = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["at"] = at,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/files")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<string>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves last-modified metadata for a repository at a ref.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="at">The ref (branch, tag, or commit).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Last modified information.</returns>
    public async Task<LastModified> GetProjectRepositoryLastModifiedAsync(string projectKey, string repositorySlug, string at, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, "/last-modified")
            .SetQueryParam("at", at)
            .GetJsonAsync<LastModified>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves participants related to pull requests in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="direction">Direction of pull requests to consider.</param>
    /// <param name="filter">Optional filter string.</param>
    /// <param name="role">Optional role filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of identities.</returns>
    public async Task<IEnumerable<Identity>> GetRepositoryParticipantsAsync(string projectKey, string repositorySlug,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? filter = null,
        Roles? role = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["filter"] = filter,
            ["role"] = BitbucketHelpers.RoleToString(role),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/participants")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Identity>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves pull requests for a repository with optional filtering.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="direction">Pull request direction filter.</param>
    /// <param name="branchId">Optional branch filter.</param>
    /// <param name="state">Pull request state.</param>
    /// <param name="order">Ordering option.</param>
    /// <param name="withAttributes">Whether to include attributes.</param>
    /// <param name="withProperties">Whether to include properties.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of pull requests.</returns>
    public async Task<IEnumerable<PullRequest>> GetPullRequestsAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? branchId = null,
        PullRequestStates state = PullRequestStates.Open,
        PullRequestOrders order = PullRequestOrders.Newest,
        bool withAttributes = true,
        bool withProperties = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["at"] = branchId,
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["withAttributes"] = BitbucketHelpers.BoolToString(withAttributes),
            ["withProperties"] = BitbucketHelpers.BoolToString(withProperties),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<PullRequest>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all pull requests for a repository as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<PullRequest> GetPullRequestsStreamAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        PullRequestDirections direction = PullRequestDirections.Incoming,
        string? branchId = null,
        PullRequestStates state = PullRequestStates.Open,
        PullRequestOrders order = PullRequestOrders.Newest,
        bool withAttributes = true,
        bool withProperties = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["direction"] = BitbucketHelpers.PullRequestDirectionToString(direction),
            ["at"] = branchId,
            ["state"] = BitbucketHelpers.PullRequestStateToString(state),
            ["order"] = BitbucketHelpers.PullRequestOrderToString(order),
            ["withAttributes"] = BitbucketHelpers.BoolToString(withAttributes),
            ["withProperties"] = BitbucketHelpers.BoolToString(withProperties),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
                .SetQueryParams(qpv)
                .GetJsonAsync<PagedResults<PullRequest>>(cancellationToken: ct)
                .ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Creates a pull request in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestInfo">The pull request payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created pull request.</returns>
    public async Task<PullRequest> CreatePullRequestAsync(string projectKey, string repositorySlug, PullRequestInfo pullRequestInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/pull-requests")
            .PostJsonAsync(pullRequestInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a pull request by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested pull request.</returns>
    public async Task<PullRequest> GetPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .GetJsonAsync<PullRequest>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="pullRequestUpdate">The update payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pull request.</returns>
    public async Task<PullRequest> UpdatePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, PullRequestUpdate pullRequestUpdate, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .PutJsonAsync(pullRequestUpdate, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="versionInfo">Version info for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the pull request was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, VersionInfo versionInfo, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}")
            .SendJsonAsync(HttpMethod.Delete, versionInfo, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves activities for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="fromId">Optional starting activity ID.</param>
    /// <param name="fromType">Optional activity type filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of pull request activities.</returns>
    public async Task<IEnumerable<PullRequestActivity>> GetPullRequestActivitiesAsync(string projectKey, string repositorySlug, long pullRequestId,
        long? fromId = null,
        PullRequestFromTypes? fromType = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["fromId"] = fromId,
            ["fromType"] = BitbucketHelpers.PullRequestFromTypeToString(fromType),
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/activities")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<PullRequestActivity>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Declines a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the pull request was declined; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeclinePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/decline")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves the merge state for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merge state.</returns>
    public async Task<PullRequestMergeState> GetPullRequestMergeStateAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<PullRequestMergeState>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the merge base (common ancestor) commit for a pull request.
    /// This is the best common ancestor between the latest commits of the source and target branches.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merge base commit, or null if not found (HTTP 204 - no common ancestor exists).</returns>
    /// <remarks>
    /// This endpoint is useful for creating line-specific comments on pull requests.
    /// The returned commit ID can be used as the <c>fromHash</c> parameter when creating anchored comments,
    /// while the <c>toHash</c> can be obtained from <see cref="FromToRef.LatestCommit"/> on the pull request's FromRef.
    /// </remarks>
    public async Task<Commit?> GetPullRequestMergeBaseAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge-base")
            .AllowHttpStatus(204)
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        // HTTP 204 indicates no common ancestor exists (e.g., unrelated histories)
        if (response.StatusCode == 204)
        {
            return null;
        }

        return await response.GetJsonAsync<Commit>().ConfigureAwait(false);
    }

    /// <summary>
    /// Merges a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The merged pull request.</returns>
    public async Task<PullRequest> MergePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/merge")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reopens a declined pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reopened pull request.</returns>
    public async Task<PullRequest> ReopenPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, int version = -1, CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["version"] = version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/reopen")
            .SetQueryParams(queryParamValues)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequest>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Approves a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reviewer entry reflecting the approval.</returns>
    public async Task<Reviewer> ApprovePullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/approve")
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Reviewer>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes an approval from a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reviewer entry after removal.</returns>
    public async Task<Reviewer> DeletePullRequestApprovalAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/approve")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Reviewer>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves changes for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="changeScope">Scope of changes to include.</param>
    /// <param name="sinceId">Optional since commit ID.</param>
    /// <param name="untilId">Optional until commit ID.</param>
    /// <param name="withComments">Whether to include comment counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of changes.</returns>
    public async Task<IEnumerable<Change>> GetPullRequestChangesAsync(string projectKey, string repositorySlug, long pullRequestId,
        ChangeScopes changeScope = ChangeScopes.All,
        string? sinceId = null,
        string? untilId = null,
        bool withComments = true,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["changeScope"] = BitbucketHelpers.ChangeScopeToString(changeScope),
            ["sinceId"] = sinceId,
            ["untilId"] = untilId,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/changes")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Change>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a comment on a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="text">The comment text.</param>
    /// <param name="parentId">Optional parent comment ID to create a reply.</param>
    /// <param name="diffType">Optional diff type.</param>
    /// <param name="fromHash">Optional from commit hash for anchoring.</param>
    /// <param name="path">Optional file path for anchoring.</param>
    /// <param name="srcPath">Optional source path for move/rename anchors.</param>
    /// <param name="toHash">Optional to commit hash for anchoring.</param>
    /// <param name="line">Optional line number for anchoring.</param>
    /// <param name="fileType">Optional file type for anchoring.</param>
    /// <param name="lineType">Optional line type for anchoring.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created comment reference.</returns>
    public async Task<CommentRef> CreatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId,
        string text,
        string? parentId = null,
        DiffTypes? diffType = null,
        string? fromHash = null,
        string? path = null,
        string? srcPath = null,
        string? toHash = null,
        int? line = null,
        FileTypes? fileType = null,
        LineTypes? lineType = null,
        CancellationToken cancellationToken = default)
    {
        // Build the comment payload dynamically to avoid sending empty anchor objects
        // which Bitbucket Server 9.0 rejects with HTTP 500.
        // See: BUG-003 - add_pull_request_comment returns 500 error
        var data = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["text"] = text,
        };

        if (!string.IsNullOrEmpty(parentId))
        {
            data["parent"] = new { id = parentId };
        }

        // Only include anchor if at least one anchor-related field is specified
        // Empty anchor objects cause HTTP 500 on Bitbucket Server 9.0
        var hasAnchorData = diffType.HasValue
            || !string.IsNullOrEmpty(fromHash)
            || !string.IsNullOrEmpty(path)
            || !string.IsNullOrEmpty(srcPath)
            || !string.IsNullOrEmpty(toHash)
            || line.HasValue
            || fileType.HasValue
            || lineType.HasValue;

        if (hasAnchorData)
        {
            data["anchor"] = new
            {
                diffType = BitbucketHelpers.DiffTypeToString(diffType),
                fromHash,
                path,
                srcPath,
                toHash,
                line,
                fileType = BitbucketHelpers.FileTypeToString(fileType),
                lineType = BitbucketHelpers.LineTypeToString(lineType),
            };
        }

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves comments for a pull request path.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="path">The file path to filter comments.</param>
    /// <param name="anchorState">Anchor state filter.</param>
    /// <param name="diffType">Diff type filter.</param>
    /// <param name="fromHash">Optional from commit hash.</param>
    /// <param name="toHash">Optional to commit hash.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of pull request comments.</returns>
    public async Task<IEnumerable<CommentRef>> GetPullRequestCommentsAsync(string projectKey, string repositorySlug, long pullRequestId,
        string path,
        AnchorStates anchorState = AnchorStates.Active,
        DiffTypes diffType = DiffTypes.Effective,
        string? fromHash = null,
        string? toHash = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
            ["path"] = path,
            ["anchorState"] = BitbucketHelpers.AnchorStateToString(anchorState),
            ["diffType"] = BitbucketHelpers.DiffTypeToString(diffType),
            ["fromHash"] = fromHash,
            ["toHash"] = toHash,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/comments")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<CommentRef>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a single pull request comment by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested comment reference.</returns>
    public async Task<CommentRef> GetPullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("avatarSize", avatarSize)
            .GetJsonAsync<CommentRef>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">The comment version for optimistic concurrency.</param>
    /// <param name="text">The updated comment text.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated comment reference.</returns>
    public async Task<CommentRef> UpdatePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int version, string text, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            version,
            text,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("version", version)
            .PutJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<CommentRef>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a pull request comment.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="commentId">The comment ID.</param>
    /// <param name="version">Optional version for optimistic concurrency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the comment was deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestCommentAsync(string projectKey, string repositorySlug, long pullRequestId, long commentId,
        int version = -1,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/comments/{commentId}")
            .SetQueryParam("version", version)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves commits associated with a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="withCounts">Whether to include change counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of commits.</returns>
    public async Task<IEnumerable<Commit>> GetPullRequestCommitsAsync(string projectKey, string repositorySlug, long pullRequestId,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/commits")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Commit>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams all commits for a pull request as an IAsyncEnumerable.
    /// </summary>
    public IAsyncEnumerable<Commit> GetPullRequestCommitsStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        bool withCounts = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/commits")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Commit>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Retrieves the diff for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="contextLines">Number of context lines to include.</param>
    /// <param name="diffType">Diff type.</param>
    /// <param name="sinceId">Optional since commit ID.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="untilId">Optional until commit ID.</param>
    /// <param name="whitespace">Whitespace handling option.</param>
    /// <param name="withComments">Whether to include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Differences for the pull request.</returns>
    public async Task<Differences> GetPullRequestDiffAsync(string projectKey, string repositorySlug, long pullRequestId,
        int contextLines = -1,
        DiffTypes diffType = DiffTypes.Effective,
        string? sinceId = null,
        string? srcPath = null,
        string? untilId = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = CreatePullRequestDiffQueryParams(contextLines, diffType, sinceId, srcPath, untilId, whitespace, withComments);

        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Differences>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams diff entries for a pull request as an <see cref="IAsyncEnumerable{T}"/>.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="contextLines">Number of context lines to include.</param>
    /// <param name="diffType">Diff type.</param>
    /// <param name="sinceId">Optional since commit ID.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="untilId">Optional until commit ID.</param>
    /// <param name="whitespace">Whitespace handling option.</param>
    /// <param name="withComments">Whether to include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream of diff entries.</returns>
    public async IAsyncEnumerable<Diff> GetPullRequestDiffStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        int contextLines = -1,
        DiffTypes diffType = DiffTypes.Effective,
        string? sinceId = null,
        string? srcPath = null,
        string? untilId = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var queryParamValues = CreatePullRequestDiffQueryParams(contextLines, diffType, sinceId, srcPath, untilId, whitespace, withComments);
        var responseStream = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff")
            .SetQueryParams(queryParamValues)
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        try
        {
            await foreach (var diff in DeserializePullRequestDiffsAsync(responseStream, cancellationToken).ConfigureAwait(false))
            {
                yield return diff;
            }
        }
        finally
        {
            await responseStream.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Retrieves the diff for a specific path within a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="path">The file path to filter by.</param>
    /// <param name="contextLines">Number of context lines to include.</param>
    /// <param name="diffType">Diff type.</param>
    /// <param name="sinceId">Optional since commit ID.</param>
    /// <param name="srcPath">Optional source path filter.</param>
    /// <param name="untilId">Optional until commit ID.</param>
    /// <param name="whitespace">Whitespace handling option.</param>
    /// <param name="withComments">Whether to include comments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Differences for the specified path.</returns>
    public async Task<Differences> GetPullRequestDiffPathAsync(string projectKey, string repositorySlug, long pullRequestId,
        string path,
        int contextLines = -1,
        DiffTypes diffType = DiffTypes.Effective,
        string? sinceId = null,
        string? srcPath = null,
        string? untilId = null,
        string whitespace = "ignore-all",
        bool withComments = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = CreatePullRequestDiffQueryParams(contextLines, diffType, sinceId, srcPath, untilId, whitespace, withComments);

        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff/{path}")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<Differences>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private static Dictionary<string, object?> CreatePullRequestDiffQueryParams(int contextLines, DiffTypes diffType, string? sinceId,
        string? srcPath, string? untilId, string whitespace, bool withComments)
    {
        return new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["contextLines"] = contextLines,
            ["diffType"] = BitbucketHelpers.DiffTypeToString(diffType),
            ["sinceId"] = sinceId,
            ["srcPath"] = srcPath,
            ["untilId"] = untilId,
            ["whitespace"] = whitespace,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };
    }

    private static async IAsyncEnumerable<Diff> DeserializePullRequestDiffsAsync(Stream responseStream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var diff in DeserializeDiffsFromStreamAsync(responseStream, cancellationToken).ConfigureAwait(false))
        {
            yield return diff;
        }
    }

    /// <summary>
    /// Deserializes diff entries from a JSON stream containing a "diffs" array.
    /// Used by all diff streaming methods (commit, repository, compare, pull request).
    /// Uses zero-copy deserialization directly from JsonElement to avoid intermediate string allocations.
    /// </summary>
    private static async IAsyncEnumerable<Diff> DeserializeDiffsFromStreamAsync(Stream responseStream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var doc = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (!doc.RootElement.TryGetProperty("diffs", out var diffsArray) || diffsArray.ValueKind != JsonValueKind.Array)
        {
            yield break;
        }

        foreach (var diffElement in diffsArray.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Zero-copy: Deserialize directly from JsonElement instead of GetRawText() string allocation
            var diff = diffElement.Deserialize<Diff>(s_jsonOptions);
            if (diff is not null)
            {
                yield return diff;
            }
        }
    }

    // Note: MoveToDiffArrayAsync is no longer needed with System.Text.Json approach

    /// <summary>
    /// Retrieves participants for a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of participants.</returns>
    public async Task<IEnumerable<Participant>> GetPullRequestParticipantsAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug)
                    .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Participant>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Assigns a role to a user in a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="named">The user to assign.</param>
    /// <param name="role">The role to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created participant entry.</returns>
    public async Task<Participant> AssignUserRoleToPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId,
        Named named,
        Roles role,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            user = named,
            role = BitbucketHelpers.RoleToString(role),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Participant>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a participant from a pull request by username.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userName">The username to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeletePullRequestParticipantAsync(string projectKey, string repositorySlug, long pullRequestId, string userName, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants")
            .SetQueryParam("username", userName)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a participant's approval status on a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userSlug">The user slug to update.</param>
    /// <param name="named">The user identity.</param>
    /// <param name="approved">Whether the participant approves the PR.</param>
    /// <param name="participantStatus">The participant status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated participant entry.</returns>
    public async Task<Participant> UpdatePullRequestParticipantStatus(string projectKey, string repositorySlug, long pullRequestId,
        string userSlug,
        Named named,
        bool approved,
        ParticipantStatus participantStatus,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            user = named,
            approved = BitbucketHelpers.BoolToString(approved),
            status = BitbucketHelpers.ParticipantStatusToString(participantStatus),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants/{userSlug}")
            .PutJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Participant>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Removes a participant from a pull request by user slug.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="userSlug">The user slug to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if removal succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnassignUserFromPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, string userSlug, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/participants/{userSlug}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks for a pull request using the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="avatarSize">Avatar size for user avatars.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of tasks.</returns>
    /// <remarks>
    /// <para>
    /// <b>Deprecation Notice:</b> This endpoint was deprecated in Bitbucket Server 9.0 and returns 404 Not Found on servers version 9.0+.
    /// </para>
    /// <para>
    /// For Bitbucket Server 9.0+, use <see cref="GetPullRequestBlockerCommentsAsync"/> instead.
    /// For cross-version compatibility, use <see cref="GetPullRequestTasksWithFallbackAsync"/>.
    /// </para>
    /// </remarks>
    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync for 9.0+ or GetPullRequestTasksWithFallbackAsync for cross-version compatibility.")]
    public async Task<IEnumerable<BitbucketTask>> GetPullRequestTasksAsync(string projectKey, string repositorySlug, long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/tasks")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<BitbucketTask>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the task count for a pull request using the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task count.</returns>
    /// <remarks>
    /// <para>
    /// <b>Deprecation Notice:</b> This endpoint was deprecated in Bitbucket Server 9.0 and may return 404 Not Found on servers version 9.0+.
    /// </para>
    /// <para>
    /// For Bitbucket Server 9.0+, use <see cref="GetPullRequestBlockerCommentsAsync"/> and count the results.
    /// </para>
    /// </remarks>
    [Obsolete("This endpoint is deprecated in Bitbucket Server 9.0+. Use GetPullRequestBlockerCommentsAsync and count the results for 9.0+ compatibility.")]
    public async Task<BitbucketTaskCount> GetPullRequestTaskCountAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/tasks/count")
            .GetJsonAsync<BitbucketTaskCount>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    #region Blocker Comments (Bitbucket Server 9.0+)

    /// <summary>
    /// Gets blocker comments (tasks) for a pull request.
    /// This endpoint is available in Bitbucket Server 9.0+ and replaces the legacy tasks endpoint.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="state">Optional filter: <see cref="BlockerCommentState.Open"/>, <see cref="BlockerCommentState.Resolved"/>, or null for all.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of blocker comments.</returns>
    /// <remarks>
    /// <para>
    /// In Bitbucket Server 9.0+, tasks have been replaced by blocker comments.
    /// A blocker comment is a comment with <c>severity: 'BLOCKER'</c> that must be resolved before the pull request can be merged.
    /// </para>
    /// <para>
    /// For servers prior to 9.0, use <see cref="GetPullRequestTasksAsync"/> instead.
    /// </para>
    /// </remarks>
    public async Task<IEnumerable<BlockerComment>> GetPullRequestBlockerCommentsAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        BlockerCommentState? state = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["state"] = BitbucketHelpers.BlockerCommentStateToString(state),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/blocker-comments")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<BlockerComment>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a single blocker comment by ID.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The blocker comment.</returns>
    public async Task<BlockerComment> GetPullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .GetJsonAsync<BlockerComment>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a blocker comment (task) on a pull request.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="text">The blocker comment text.</param>
    /// <param name="anchor">Optional anchor for file/line-specific blockers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created blocker comment.</returns>
    public async Task<BlockerComment> CreatePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        string text,
        CommentAnchor? anchor = null,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            text,
            severity = "BLOCKER",
            anchor,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a blocker comment's text.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="text">The updated blocker comment text.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated blocker comment.</returns>
    public async Task<BlockerComment> UpdatePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        string text,
        int version,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            text,
            version,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .PutJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a blocker comment.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the blocker comment was deleted successfully.</returns>
    public async Task<bool> DeletePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}")
            .SetQueryParam("version", version)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Resolves a blocker comment (marks the task as complete).
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The resolved blocker comment.</returns>
    public async Task<BlockerComment> ResolvePullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}/resolve")
            .SetQueryParam("version", version)
            .PutAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reopens a resolved blocker comment.
    /// This endpoint is available in Bitbucket Server 9.0+.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="blockerCommentId">The blocker comment ID.</param>
    /// <param name="version">The version of the blocker comment (for optimistic locking).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reopened blocker comment.</returns>
    public async Task<BlockerComment> ReopenPullRequestBlockerCommentAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        long blockerCommentId,
        int version,
        CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/blocker-comments/{blockerCommentId}/reopen")
            .SetQueryParam("version", version)
            .PutAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BlockerComment>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets pull request tasks with automatic fallback for cross-version compatibility.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method provides backward compatibility across Bitbucket Server versions:
    /// </para>
    /// <list type="bullet">
    /// <item><description><b>Bitbucket Server 9.0+:</b> Uses the new <c>/blocker-comments</c> endpoint.</description></item>
    /// <item><description><b>Bitbucket Server &lt; 9.0:</b> Falls back to the legacy <c>/tasks</c> endpoint.</description></item>
    /// </list>
    /// <para>
    /// The method first tries the new blocker-comments endpoint. If it returns 404 (Not Found),
    /// it automatically falls back to the legacy tasks endpoint.
    /// </para>
    /// <para>
    /// For new code targeting Bitbucket Server 9.0+, prefer using 
    /// <see cref="GetPullRequestBlockerCommentsAsync"/> directly for better type safety.
    /// </para>
    /// </remarks>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="maxPages">Maximum number of pages to retrieve.</param>
    /// <param name="limit">Maximum number of results per page.</param>
    /// <param name="start">Pagination start index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A collection of blocker comments (<see cref="BlockerComment"/>) on Bitbucket 9.0+,
    /// or legacy tasks (<see cref="BitbucketTask"/>) on older versions.
    /// </returns>
    public async Task<IEnumerable<object>> GetPullRequestTasksWithFallbackAsync(
        string projectKey,
        string repositorySlug,
        long pullRequestId,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try new blocker-comments endpoint first (Bitbucket 9.0+)
            var blockerComments = await GetPullRequestBlockerCommentsAsync(
                projectKey, repositorySlug, pullRequestId,
                maxPages: maxPages, limit: limit, start: start,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return blockerComments.Cast<object>();
        }
        catch (BitbucketNotFoundException)
        {
            // Fall back to legacy tasks endpoint (Bitbucket < 9.0)
#pragma warning disable CS0618 // Type or member is obsolete - intentional fallback
            var tasks = await GetPullRequestTasksAsync(
                projectKey, repositorySlug, pullRequestId,
                maxPages: maxPages, limit: limit, start: start,
                cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CS0618

            return tasks.Cast<object>();
        }
    }

    #endregion

    /// <summary>
    /// Subscribes the current user to watch a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was added; otherwise, <c>false</c>.</returns>
    public async Task<bool> WatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/watch")
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Unsubscribes the current user from watching a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the watch was removed; otherwise, <c>false</c>.</returns>
    public async Task<bool> UnwatchPullRequestAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/watch")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves raw content from a file path in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="path">The file path to fetch.</param>
    /// <param name="at">Optional ref (branch, tag, commit).</param>
    /// <param name="markup">Whether to render markup.</param>
    /// <param name="hardWrap">Whether to hard wrap the output.</param>
    /// <param name="htmlEscape">Whether to HTML-escape the output.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A stream containing the raw content.</returns>
    public async Task<Stream> RetrieveRawContentAsync(string projectKey, string repositorySlug, string path,
        string? at = null,
        bool markup = false,
        bool hardWrap = true,
        bool htmlEscape = true,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["markup"] = BitbucketHelpers.BoolToString(markup),
            ["hardWrap"] = BitbucketHelpers.BoolToString(hardWrap),
            ["htmlEscape"] = BitbucketHelpers.BoolToString(htmlEscape),
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/raw/{path}")
            .SetQueryParams(queryParamValues)
            .GetStreamAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves pull request settings for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pull request settings.</returns>
    public async Task<PullRequestSettings> GetProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, "/settings/pull-requests")
            .GetJsonAsync<PullRequestSettings>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates pull request settings for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestSettings">The settings payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pull request settings.</returns>
    public async Task<PullRequestSettings> UpdateProjectRepositoryPullRequestSettingsAsync(string projectKey, string repositorySlug,
        PullRequestSettings pullRequestSettings, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/settings/pull-requests")
            .PostJsonAsync(pullRequestSettings, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequestSettings>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves repository hooks.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookType">Optional hook type filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of hooks.</returns>
    public async Task<IEnumerable<Hook>> GetProjectRepositoryHooksSettingsAsync(string projectKey, string repositorySlug,
        HookTypes? hookType = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["type"] = hookType,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/settings/hooks")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Hook>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a specific hook's settings.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The hook configuration.</returns>
    public async Task<Hook> GetProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}")
            .GetJsonAsync<Hook>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a repository hook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectRepositoryHookSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Enables a repository hook, optionally providing settings.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="hookSettings">Optional hook settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The enabled hook.</returns>
    public async Task<Hook> EnableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, object? hookSettings = null, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}/enabled")
            .PutJsonAsync(hookSettings, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Hook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Disables a repository hook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The disabled hook.</returns>
    public async Task<Hook> DisableProjectRepositoryHookAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}/enabled")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Hook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves all settings for a repository hook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of hook settings.</returns>
    public async Task<Dictionary<string, object?>> GetProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}/settings")
            .GetJsonAsync<Dictionary<string, object?>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates all settings for a repository hook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="hookKey">The hook key.</param>
    /// <param name="allSettings">The settings payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated settings.</returns>
    public async Task<Dictionary<string, object?>> UpdateProjectRepositoryHookAllSettingsAsync(string projectKey, string repositorySlug, string hookKey,
        Dictionary<string, object?> allSettings, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}/settings")
            .PutJsonAsync(allSettings, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, object?>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves merge strategies for pull requests within a project SCM.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="scmId">The SCM identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The pull request settings.</returns>
    public async Task<PullRequestSettings> GetProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, CancellationToken cancellationToken = default)
    {
        return await GetProjectUrl(projectKey)
            .AppendPathSegment($"/settings/pull-requests/{scmId}")
            .GetJsonAsync<PullRequestSettings>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates merge strategies for pull requests within a project SCM.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="scmId">The SCM identifier.</param>
    /// <param name="mergeStrategies">The merge strategies payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated merge strategies.</returns>
    public async Task<MergeStrategies> UpdateProjectPullRequestsMergeStrategiesAsync(string projectKey, string scmId, MergeStrategies mergeStrategies, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectUrl(projectKey)
            .AppendPathSegment($"/settings/pull-requests/{scmId}")
            .PostJsonAsync(mergeStrategies, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<MergeStrategies>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves tags from a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filterText">Filter text for tag names.</param>
    /// <param name="orderBy">Ordering option.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tags.</returns>
    public async Task<IEnumerable<Tag>> GetProjectRepositoryTagsAsync(string projectKey, string repositorySlug,
        string filterText,
        BranchOrderBy orderBy,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filterText"] = filterText,
            ["orderBy"] = BitbucketHelpers.BranchOrderByToString(orderBy),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/tags")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Tag>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a tag in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="name">The tag name.</param>
    /// <param name="startPoint">The starting commit or ref.</param>
    /// <param name="message">The tag message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created tag.</returns>
    public async Task<Tag> CreateProjectRepositoryTagAsync(string projectKey, string repositorySlug,
        string name,
        string startPoint,
        string message,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            name,
            startPoint,
            message,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/tags")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Tag>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a tag by name.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="tagName">The tag name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested tag.</returns>
    public async Task<Tag> GetProjectRepositoryTagAsync(string projectKey, string repositorySlug, string tagName, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/tags/{tagName}")
            .GetJsonAsync<Tag>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves webhooks for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="event">Optional event filter.</param>
    /// <param name="statistics">Whether to include statistics.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of webhooks.</returns>
    public async Task<IEnumerable<WebHook>> GetProjectRepositoryWebHooksAsync(string projectKey, string repositorySlug,
        string? @event = null,
        bool statistics = false,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["event"] = @event,
            ["statistics"] = BitbucketHelpers.BoolToString(statistics),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetProjectsReposUrl(projectKey, repositorySlug, "/webhooks")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<WebHook>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a webhook for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHook">The webhook payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created webhook.</returns>
    public async Task<WebHook> CreateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, WebHook webHook, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/webhooks")
            .PostJsonAsync(webHook, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<WebHook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests a webhook delivery for a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="url">The URL to test.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The webhook test response.</returns>
    public async Task<WebHookTestRequestResponse> TestProjectRepositoryWebHookAsync(string projectKey, string repositorySlug, string url, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/webhooks/test")
            .SetQueryParam("url", url)
            .PostJsonAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<WebHookTestRequestResponse>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a webhook by ID.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="statistics">Whether to include statistics.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The webhook.</returns>
    public async Task<WebHook> GetProjectRepositoryWebHookAsync(string projectKey, string repositorySlug,
        string webHookId,
        bool statistics = false,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["statistics"] = BitbucketHelpers.BoolToString(statistics),
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}")
            .SetQueryParams(queryParamValues)
            .GetJsonAsync<WebHook>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Updates a webhook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="webHook">The webhook payload.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated webhook.</returns>
    public async Task<WebHook> UpdateProjectRepositoryWebHookAsync(string projectKey, string repositorySlug,
        string webHookId, WebHook webHook, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}")
            .PutJsonAsync(webHook, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<WebHook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes a webhook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise, <c>false</c>.</returns>
    public async Task<bool> DeleteProjectRepositoryWebHookAsync(string projectKey, string repositorySlug,
        string webHookId, CancellationToken cancellationToken = default)
    {
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    //public async Task<WebHookInvocation> GetProjectRepositoryWebHookLatestAsync(string projectKey, string repositorySlug,
    /// <summary>
    /// Retrieves the latest webhook invocation summary as a string.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="event">Optional event filter.</param>
    /// <param name="outcome">Optional outcome filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The latest invocation payload.</returns>
    public async Task<string> GetProjectRepositoryWebHookLatestAsync(string projectKey, string repositorySlug,
        string webHookId,
        string? @event = null,
        WebHookOutcomes? outcome = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>
(StringComparer.Ordinal)
        {
            ["event"] = @event,
            ["outcome"] = BitbucketHelpers.WebHookOutcomeToString(outcome),
        };

        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/latest")
            .SetQueryParams(queryParamValues)
            //.GetJsonAsync<WebHookInvocation>()
            .GetStringAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves webhook statistics.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="event">Optional event filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Webhook statistics.</returns>
    public async Task<WebHookStatistics> GetProjectRepositoryWebHookStatisticsAsync(string projectKey, string repositorySlug,
        string webHookId,
        string? @event = null,
        CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/statistics")
            .SetQueryParam("event", @event)
            .GetJsonAsync<WebHookStatistics>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a statistics summary for a webhook.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="webHookId">The webhook ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of webhook statistics counts.</returns>
    public async Task<Dictionary<string, WebHookStatisticsCounts>> GetProjectRepositoryWebHookStatisticsSummaryAsync(string projectKey, string repositorySlug,
        string webHookId, CancellationToken cancellationToken = default)
    {
        return await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/statistics/summary")
            .GetJsonAsync<Dictionary<string, WebHookStatisticsCounts>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}