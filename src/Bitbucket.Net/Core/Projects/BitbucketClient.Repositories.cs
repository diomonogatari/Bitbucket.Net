using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Users;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/repos")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Repository>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsUrl($"/{projectKey}/repos")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Repository>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Repository>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
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
            .SendAsync(HttpMethod.Put, CreateJsonContent(data), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/forks")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<RepositoryFork>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
            .SendAsync(HttpMethod.Post, new StringContent(string.Empty), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/related")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<RepositoryFork>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["fileName"] = fileName,
            ["format"] = BitbucketHelpers.ArchiveFormatToString(archiveFormat),
            ["path"] = path,
            ["prefix"] = prefix,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/archive")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseBytesAsync(response, cancellationToken).ConfigureAwait(false);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<GroupPermission>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["name"] = name,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Put, new StringContent(string.Empty), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/groups/none")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<DeletableGroupOrUser>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["filter"] = filter,
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<UserPermission>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
            ["name"] = name,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users")
            .SetQueryParams(queryParamValues)
            .SendAsync(HttpMethod.Put, new StringContent(string.Empty), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filter"] = filter,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/permissions/users/none")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<User>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }
}