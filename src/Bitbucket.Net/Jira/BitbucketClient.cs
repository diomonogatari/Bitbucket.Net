using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Builds;
using Bitbucket.Net.Models.Jira;
using Flurl.Http;

namespace Bitbucket.Net;

/// <summary>
/// Provides Jira-related Bitbucket API operations.
/// </summary>
public partial class BitbucketClient
{
    /// <summary>
    /// Gets the base Jira URL.
    /// </summary>
    /// <returns>An <see cref="IFlurlRequest"/> targeting the Jira root.</returns>
    protected IFlurlRequest GetJiraUrl() => GetBaseUrl("/jira");

    /// <summary>
    /// Gets the Jira URL for the specified path.
    /// </summary>
    /// <param name="path">The path to append to the Jira root.</param>
    /// <returns>An <see cref="IFlurlRequest"/> pointing to the Jira path.</returns>
    protected IFlurlRequest GetJiraUrl(string path) => GetJiraUrl()
        .AppendPathSegment(path);

    /// <summary>
    /// Retrieves changesets linked to a Jira issue.
    /// </summary>
    /// <param name="issueKey">The Jira issue key.</param>
    /// <param name="maxChanges">Maximum number of changes per commit to include.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of changesets.</returns>
    public Task<IReadOnlyList<ChangeSet>> GetChangeSetsAsync(string issueKey, int maxChanges = 10,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issueKey);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["maxChanges"] = maxChanges,
        };

        return GetPagedAsync<ChangeSet>(
            GetJiraUrl($"/issues/{issueKey}/commits"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates a Jira issue linked to a pull request comment.
    /// </summary>
    /// <param name="pullRequestCommentId">The pull request comment identifier.</param>
    /// <param name="applicationId">The application identifier.</param>
    /// <param name="title">The issue title.</param>
    /// <param name="type">The issue type.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created Jira issue.</returns>
    public async Task<JiraIssue> CreateJiraIssueAsync(string pullRequestCommentId, string applicationId, string title, string type, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pullRequestCommentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        var data = new
        {
            id = "https://docs.atlassian.com/jira/REST/schema/string#",
            title,
            type,
        };

        var response = await GetJiraUrl($"/comments/{pullRequestCommentId}/issues")
            .SetQueryParam("applicationId", applicationId)
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<JiraIssue>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves Jira issues linked to a pull request.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request identifier.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A collection of Jira issue links.</returns>
    public async Task<IReadOnlyList<KeyedUrl>> GetJiraIssuesAsync(string projectKey, string repositorySlug, long pullRequestId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(repositorySlug);

        var response = await GetJiraUrl($"/projects/{projectKey}/repos/{repositorySlug}/pull-requests/{pullRequestId}/issues")
            .GetAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var items = await HandleResponseAsync<IEnumerable<KeyedUrl>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
        return items.ToList();
    }
}