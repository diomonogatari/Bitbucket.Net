using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Admin;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["at"] = at,
            ["markup"] = BitbucketHelpers.BoolToString(markup),
            ["hardWrap"] = BitbucketHelpers.BoolToString(hardWrap),
            ["htmlEscape"] = BitbucketHelpers.BoolToString(htmlEscape),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/raw/{path}")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/settings/pull-requests")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequestSettings>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(pullRequestSettings), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["type"] = hookType,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/settings/hooks")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Hook>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Hook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Put, CreateJsonContent(hookSettings), cancellationToken: cancellationToken)
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/settings/hooks/{hookKey}/settings")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, object?>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Put, CreateJsonContent(allSettings), cancellationToken: cancellationToken)
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
        var response = await GetProjectUrl(projectKey)
            .AppendPathSegment($"/settings/pull-requests/{scmId}")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<PullRequestSettings>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(mergeStrategies), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filterText"] = filterText,
            ["orderBy"] = BitbucketHelpers.BranchOrderByToString(orderBy),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/tags")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Tag>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams tags for a repository, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="filterText">Filter text for tag names.</param>
    /// <param name="orderBy">Ordering option.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of tags.</returns>
    public IAsyncEnumerable<Tag> GetProjectRepositoryTagsStreamAsync(string projectKey, string repositorySlug,
        string filterText,
        BranchOrderBy orderBy,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["filterText"] = filterText,
            ["orderBy"] = BitbucketHelpers.BranchOrderByToString(orderBy),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/tags")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Tag>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(data), cancellationToken: cancellationToken)
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/tags/{tagName}")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Tag>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["event"] = @event,
            ["statistics"] = BitbucketHelpers.BoolToString(statistics),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/webhooks")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<WebHook>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
            .SendAsync(HttpMethod.Post, CreateJsonContent(webHook), cancellationToken: cancellationToken)
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
            .SendAsync(HttpMethod.Post, CreateEmptyJsonContent(), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["statistics"] = BitbucketHelpers.BoolToString(statistics),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<WebHook>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Put, CreateJsonContent(webHook), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["event"] = @event,
            ["outcome"] = BitbucketHelpers.WebHookOutcomeToString(outcome),
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/latest")
            .SetQueryParams(queryParamValues)
            //.GetJsonAsync<WebHookInvocation>()
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync(response, s => s, cancellationToken).ConfigureAwait(false);
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/statistics")
            .SetQueryParam("event", @event)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<WebHookStatistics>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/webhooks/{webHookId}/statistics/summary")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Dictionary<string, WebHookStatisticsCounts>>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}