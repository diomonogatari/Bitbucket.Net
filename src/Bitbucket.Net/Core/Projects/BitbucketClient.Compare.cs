using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects;
using Flurl.Http;
using System.Runtime.CompilerServices;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
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
    public Task<IReadOnlyList<Change>> GetRepositoryCompareChangesAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
        };

        return GetPagedAsync<Change>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/compare/changes"), queryParamValues, maxPages, cancellationToken);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
            ["srcPath"] = srcPath,
            ["contextLines"] = contextLines,
            ["whitespace"] = whitespace,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Differences>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
            ["srcPath"] = srcPath,
            ["contextLines"] = contextLines,
            ["whitespace"] = whitespace,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/compare/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        var responseStream = await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);

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
    public Task<IReadOnlyList<Commit>> GetRepositoryCompareCommitsAsync(string projectKey, string repositorySlug, string from, string to,
        string? fromRepo = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["from"] = from,
            ["to"] = to,
            ["fromRepo"] = fromRepo,
        };

        return GetPagedAsync<Commit>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/compare/commits"), queryParamValues, maxPages, cancellationToken);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["until"] = until,
            ["whitespace"] = whitespace,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Differences>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["contextLines"] = contextLines,
            ["since"] = since,
            ["srcPath"] = srcPath,
            ["until"] = until,
            ["whitespace"] = whitespace,
        };

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/diff")
            .SetQueryParams(queryParamValues)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        var responseStream = await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);

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
    public Task<IReadOnlyList<string>> GetRepositoryFilesAsync(string projectKey, string repositorySlug, string? at = null,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["at"] = at,
        };

        return GetPagedAsync<string>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/files"), queryParamValues, maxPages, cancellationToken);
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/last-modified")
            .SetQueryParam("at", at)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<LastModified>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}