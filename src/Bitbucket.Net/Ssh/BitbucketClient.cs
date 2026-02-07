using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.RefRestrictions;
using Bitbucket.Net.Models.Ssh;
using Flurl.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    private IFlurlRequest GetKeysUrl() => GetBaseUrl("/keys");

    private IFlurlRequest GetKeysUrl(string path) => GetKeysUrl()
        .AppendPathSegment(path);

    private IFlurlRequest GetSshUrl() => GetBaseUrl("/ssh");

    private IFlurlRequest GetSshUrl(string path) => GetSshUrl()
        .AppendPathSegment(path);

    public async Task<bool> DeleteProjectsReposKeysAsync(int keyId, CancellationToken cancellationToken, params string[] projectsOrRepos)
    {
        var json = JsonSerializer.Serialize(projectsOrRepos);
        var response = await GetKeysUrl($"/ssh/{keyId}")
            .WithHeader("Content-Type", "application/json")
            .SendAsync(HttpMethod.Delete, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteProjectsReposKeysAsync(int keyId, params string[] projectsOrRepos)
    {
        return await DeleteProjectsReposKeysAsync(keyId, default, projectsOrRepos).ConfigureAwait(false);
    }

    public async Task<IEnumerable<ProjectKey>> GetProjectKeysAsync(int keyId,
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
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetKeysUrl($"/ssh/{keyId}/projects")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<ProjectKey>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ProjectKey>> GetProjectKeysAsync(string projectKey,
        string? filter = null,
        Permissions? permission = null,
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
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetKeysUrl($"/projects/{projectKey}/ssh")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<ProjectKey>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<ProjectKey> CreateProjectKeyAsync(string projectKey, string keyText, Permissions permission, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            key = new { text = keyText },
            permission = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetKeysUrl($"/projects/{projectKey}/ssh")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProjectKey> GetProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteProjectKeyAsync(string projectKey, int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<ProjectKey> UpdateProjectKeyPermissionAsync(string projectKey, int keyId, Permissions permission, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/ssh/{keyId}/permissions/{BitbucketHelpers.PermissionToString(permission)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<ProjectKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<RepositoryKey>> GetRepoKeysAsync(int keyId,
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
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetKeysUrl($"/ssh/{keyId}/repos")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<RepositoryKey>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<RepositoryKey>> GetRepoKeysAsync(string projectKey, string repositorySlug,
        string? filter = null,
        bool? effective = null,
        Permissions? permission = null,
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
            ["effective"] = BitbucketHelpers.BoolToString(effective),
            ["permission"] = BitbucketHelpers.PermissionToString(permission),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<RepositoryKey>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RepositoryKey> CreateRepoKeyAsync(string projectKey, string repositorySlug, string keyText, Permissions permission, CancellationToken cancellationToken = default)
    {
        var data = new
        {
            key = new { text = keyText },
            permission = BitbucketHelpers.PermissionToString(permission),
        };

        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh")
            .PostJsonAsync(data, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<RepositoryKey> GetRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteRepoKeyAsync(string projectKey, string repositorySlug, int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<RepositoryKey> UpdateRepoKeyPermissionAsync(string projectKey, string repositorySlug, int keyId, Permissions permission, CancellationToken cancellationToken = default)
    {
        var response = await GetKeysUrl($"/projects/{projectKey}/repos/{repositorySlug}/ssh/{keyId}/permissions/{BitbucketHelpers.PermissionToString(permission)}")
            .PutAsync(new StringContent(""), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<RepositoryKey>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Key>> GetUserKeysAsync(string? userSlug = null,
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
            ["user"] = userSlug,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
                await GetSshUrl("/keys")
                    .SetQueryParams(qpv)
                    .GetJsonAsync<PagedResults<Key>>(cancellationToken: ct)
                    .ConfigureAwait(false), cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Key> CreateUserKeyAsync(string keyText, string? userSlug = null, CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl("/keys")
            .SetQueryParam("user", userSlug)
            .PostJsonAsync(new { text = keyText }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Key>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserKeysAsync(string? userSlug = null, CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl("/keys")
            .SetQueryParam("user", userSlug)
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteUserKeyAsync(int keyId, CancellationToken cancellationToken = default)
    {
        var response = await GetSshUrl($"/keys/{keyId}")
            .DeleteAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<SshSettings> GetSshSettingsAsync(CancellationToken cancellationToken = default)
    {
        return await GetSshUrl("/settings")
            .GetJsonAsync<SshSettings>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}