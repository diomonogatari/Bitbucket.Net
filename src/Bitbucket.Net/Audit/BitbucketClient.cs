using Bitbucket.Net.Models.Audit;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base audit URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> representing the audit endpoint.</returns>
    protected IFlurlRequest GetAuditUrl() => GetBaseUrl("/audit");

    /// <summary>
    /// Gets the audit URL with the specified path appended.
    /// </summary>
    /// <param name="path">The path to append to the audit URL.</param>
    /// <returns>An <see cref="IFlurlRequest"/> representing the audit endpoint with the specified path.</returns>
    protected IFlurlRequest GetAuditUrl(string path) => GetAuditUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves audit events for a specific project.
    /// </summary>
    /// <param name="projectKey">The key of the project.</param>
    /// <param name="maxPages">The maximum number of pages to retrieve. If <c>null</c>, all pages are retrieved.</param>
    /// <param name="limit">The maximum number of results per page.</param>
    /// <param name="start">The starting index for pagination.</param>
    /// <param name="avatarSize">The size of user avatars to include in the response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="AuditEvent"/> objects.</returns>
    public Task<IReadOnlyList<AuditEvent>> GetProjectAuditEventsAsync(string projectKey,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return GetPagedAsync<AuditEvent>(
            GetAuditUrl($"/projects/{projectKey}/events"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Retrieves audit events for a specific repository within a project.
    /// </summary>
    /// <param name="projectKey">The key of the project.</param>
    /// <param name="repositorySlug">The slug (identifier) of the repository.</param>
    /// <param name="maxPages">The maximum number of pages to retrieve. If <see langword="null"/>, all pages are retrieved.</param>
    /// <param name="limit">The maximum number of results per page.</param>
    /// <param name="start">The starting index for pagination.</param>
    /// <param name="avatarSize">The size of user avatars to include in the response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="AuditEvent"/> objects.</returns>
    public Task<IReadOnlyList<AuditEvent>> GetProjectRepoAuditEventsAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        int? avatarSize = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["avatarSize"] = avatarSize,
        };

        return GetPagedAsync<AuditEvent>(
            GetAuditUrl($"/projects/{projectKey}/repos/{repositorySlug}/events"), queryParamValues, maxPages, cancellationToken);
    }
}