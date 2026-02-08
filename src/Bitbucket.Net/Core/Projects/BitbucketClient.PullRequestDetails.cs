using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["fromId"] = fromId,
            ["fromType"] = BitbucketHelpers.PullRequestFromTypeToString(fromType),
            ["avatarSize"] = avatarSize,
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/activities")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequestActivity>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams activities for a pull request, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="fromId">Optional starting activity ID.</param>
    /// <param name="fromType">Optional from type filter.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="avatarSize">Optional avatar size.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of pull request activities.</returns>
    public IAsyncEnumerable<PullRequestActivity> GetPullRequestActivitiesStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        long? fromId = null,
        PullRequestFromTypes? fromType = null,
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
            ["fromId"] = fromId,
            ["fromType"] = BitbucketHelpers.PullRequestFromTypeToString(fromType),
            ["avatarSize"] = avatarSize,
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/activities")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<PullRequestActivity>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["changeScope"] = BitbucketHelpers.ChangeScopeToString(changeScope),
            ["sinceId"] = sinceId,
            ["untilId"] = untilId,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/changes")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Change>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Streams changes for a pull request, yielding items as they are retrieved.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="pullRequestId">The pull request ID.</param>
    /// <param name="changeScope">The change scope filter.</param>
    /// <param name="sinceId">Optional since commit ID.</param>
    /// <param name="untilId">Optional until commit ID.</param>
    /// <param name="withComments">Whether to include comment counts.</param>
    /// <param name="maxPages">Optional maximum number of pages to retrieve.</param>
    /// <param name="limit">Optional page size.</param>
    /// <param name="start">Optional starting index for pagination.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>An async enumerable of changes.</returns>
    public IAsyncEnumerable<Change> GetPullRequestChangesStreamAsync(string projectKey, string repositorySlug, long pullRequestId,
        ChangeScopes changeScope = ChangeScopes.All,
        string? sinceId = null,
        string? untilId = null,
        bool withComments = true,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["changeScope"] = BitbucketHelpers.ChangeScopeToString(changeScope),
            ["sinceId"] = sinceId,
            ["untilId"] = untilId,
            ["withComments"] = BitbucketHelpers.BoolToString(withComments),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/changes")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Change>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return await GetPagedResultsAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/commits")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Commit>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["withCounts"] = BitbucketHelpers.BoolToString(withCounts),
        };

        return GetPagedResultsStreamAsync(maxPages, queryParamValues, async (qpv, ct) =>
            {
                var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/pull-requests/{pullRequestId}/commits")
                    .SetQueryParams(qpv)
                    .GetAsync(ct)
                    .ConfigureAwait(false);

                return await HandleResponseAsync<PagedResults<Commit>>(response, cancellationToken: ct).ConfigureAwait(false);
            }, cancellationToken);
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

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Differences>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        var responseStream = await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);

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

        var response = await GetProjectsReposUrl(projectKey, repositorySlug)
            .AppendPathSegment($"/pull-requests/{pullRequestId}/diff/{path}")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Differences>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static Dictionary<string, object?> CreatePullRequestDiffQueryParams(int contextLines, DiffTypes diffType, string? sinceId,
        string? srcPath, string? untilId, string whitespace, bool withComments)
    {
        return new Dictionary<string, object?>(StringComparer.Ordinal)
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

}