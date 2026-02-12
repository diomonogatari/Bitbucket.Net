using Bitbucket.Net.Common;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Models.Core.Projects.Requests;
using Flurl.Http;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Bitbucket.Net;

public partial class BitbucketClient
{
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
    public Task<IReadOnlyList<Branch>> GetBranchesAsync(string projectKey, string repositorySlug,
        int? maxPages = null,
        int? limit = null,
        int? start = null,
        string? baseBranchOrTag = null,
        bool? details = null,
        string? filterText = null,
        BranchOrderBy? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["base"] = baseBranchOrTag,
            ["details"] = details.HasValue ? BitbucketHelpers.BoolToString(details.Value) : null,
            ["filterText"] = filterText,
            ["orderBy"] = orderBy.HasValue ? BitbucketHelpers.BranchOrderByToString(orderBy.Value) : null,
        };

        return GetPagedAsync<Branch>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/branches"), queryParamValues, maxPages, cancellationToken);
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["limit"] = limit,
            ["start"] = start,
            ["base"] = baseBranchOrTag,
            ["details"] = details.HasValue ? BitbucketHelpers.BoolToString(details.Value) : null,
            ["filterText"] = filterText,
            ["orderBy"] = orderBy.HasValue ? BitbucketHelpers.BranchOrderByToString(orderBy.Value) : null,
        };

        return GetPagedStreamAsync<Branch>(
            GetProjectsReposUrl(projectKey, repositorySlug, "/branches"), queryParamValues, maxPages, cancellationToken);
    }

    /// <summary>
    /// Creates a branch in a repository.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="repositorySlug">The repository slug.</param>
    /// <param name="request">The create branch request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created branch.</returns>
    public async Task<Branch> CreateBranchAsync(string projectKey, string repositorySlug, CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/branches")
            .SendAsync(HttpMethod.Post, CreateJsonContent(request), cancellationToken: cancellationToken)
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
        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/branches/default")
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<Branch>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
            .SendAsync(HttpMethod.Put, CreateJsonContent(branchRef), cancellationToken: cancellationToken)
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
        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
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

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, "/browse")
            .SetQueryParams(queryParamValues, Flurl.NullValueHandling.NameOnly)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BrowseItem>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
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

        var response = await GetProjectsReposUrl(projectKey, repositorySlug, $"/browse/{path}")
            .SetQueryParams(queryParamValues, Flurl.NullValueHandling.NameOnly)
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        return await HandleResponseAsync<BrowsePathItem>(response, cancellationToken: cancellationToken).ConfigureAwait(false);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var request = GetProjectsReposUrl(projectKey, repositorySlug, $"/raw/{path}");

        if (!string.IsNullOrEmpty(at))
        {
            request = request.SetQueryParam("at", at);
        }

        var response = await request
            .GetAsync(cancellationToken)
            .ConfigureAwait(false);

        await HandleErrorsAsync(response, cancellationToken).ConfigureAwait(false);
        return await ReadResponseStreamAsync(response, cancellationToken).ConfigureAwait(false);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

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
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

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
}