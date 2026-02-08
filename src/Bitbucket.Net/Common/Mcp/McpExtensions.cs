using System.Runtime.CompilerServices;

namespace Bitbucket.Net.Common.Mcp;

/// <summary>
/// MCP-optimized extension methods for common truncation and pagination patterns.
/// Designed for Model Context Protocol (MCP) server integration where context window
/// limits require intelligent truncation of large result sets.
/// </summary>
public static class McpExtensions
{
    /// <summary>
    /// Takes the first N items from an async enumerable with pagination metadata preserved.
    /// This method is optimized for MCP servers that need to truncate large result sets
    /// while maintaining pagination information for follow-up requests.
    /// </summary>
    /// <typeparam name="T">The type of items in the sequence.</typeparam>
    /// <param name="source">The async enumerable source.</param>
    /// <param name="limit">Maximum number of items to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A PaginatedResult containing:
    /// - Items: The first N items from the source
    /// - HasMore: True if there are more items beyond the limit
    /// - NextOffset: The offset for the next page (equal to limit if HasMore is true)
    /// </returns>
    /// <remarks>
    /// Per MCP best practices, pagination responses should include has_more, next_offset, and total_count.
    /// This method fetches limit+1 items to determine if more exist without fetching the entire collection.
    /// 
    /// Usage with streaming APIs:
    /// <code>
    /// var result = await client.GetPullRequestsStreamAsync(projectKey, repoSlug)
    ///     .TakeWithPaginationAsync(limit: 25);
    /// 
    /// // Return to MCP client
    /// return new { 
    ///     items = result.Items, 
    ///     has_more = result.HasMore, 
    ///     next_offset = result.NextOffset 
    /// };
    /// </code>
    /// </remarks>
    public static async Task<PaginatedResult<T>> TakeWithPaginationAsync<T>(
        this IAsyncEnumerable<T> source,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var items = new List<T>(limit);
        int count = 0;

        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (count < limit)
            {
                items.Add(item);
            }

            count++;

            // Found one more than requested - we know there are more items
            if (count > limit)
            {
                return new PaginatedResult<T>(items, hasMore: true, nextOffset: limit);
            }
        }

        return new PaginatedResult<T>(items, hasMore: false, nextOffset: null);
    }

    /// <summary>
    /// Streams items with a hard limit, stopping enumeration after the limit is reached.
    /// More memory-efficient than TakeWithPaginationAsync when you don't need HasMore metadata.
    /// </summary>
    /// <typeparam name="T">The type of items in the sequence.</typeparam>
    /// <param name="source">The async enumerable source.</param>
    /// <param name="limit">Maximum number of items to yield.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable that yields at most limit items.</returns>
    /// <remarks>
    /// This is the most efficient option when you only need to limit results without
    /// knowing if more exist. The enumeration stops immediately after yielding
    /// the limit-th item.
    /// 
    /// Usage:
    /// <code>
    /// await foreach (var pr in client.GetPullRequestsStreamAsync(projectKey, repoSlug)
    ///     .TakeAsync(10))
    /// {
    ///     // Process at most 10 PRs
    /// }
    /// </code>
    /// </remarks>
    public static async IAsyncEnumerable<T> TakeAsync<T>(
        this IAsyncEnumerable<T> source,
        int limit,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int count = 0;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (count >= limit)
            {
                yield break;
            }

            yield return item;
            count++;
        }
    }

    /// <summary>
    /// Skips the first N items and then yields the remaining items.
    /// Useful for implementing offset-based pagination on top of streaming APIs.
    /// </summary>
    /// <typeparam name="T">The type of items in the sequence.</typeparam>
    /// <param name="source">The async enumerable source.</param>
    /// <param name="count">Number of items to skip.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable that skips the first count items.</returns>
    public static async IAsyncEnumerable<T> SkipAsync<T>(
        this IAsyncEnumerable<T> source,
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int skipped = 0;
        await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (skipped < count)
            {
                skipped++;
                continue;
            }

            yield return item;
        }
    }

    /// <summary>
    /// Implements offset/limit pagination on top of a streaming source.
    /// Combines Skip and Take for traditional pagination patterns.
    /// </summary>
    /// <typeparam name="T">The type of items in the sequence.</typeparam>
    /// <param name="source">The async enumerable source.</param>
    /// <param name="offset">Number of items to skip (0-based offset).</param>
    /// <param name="limit">Maximum number of items to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A PaginatedResult with items from offset to offset+limit-1.
    /// Note: NextOffset in the result is relative to the current window (equals limit when HasMore is true).
    /// To calculate the absolute offset for the next page, use: offset + result.NextOffset.
    /// </returns>
    /// <remarks>
    /// This is useful when an MCP client requests a specific page:
    /// <code>
    /// // Client requests page 3 with 25 items per page
    /// var result = await client.GetCommitsStreamAsync(projectKey, repoSlug)
    ///     .PageAsync(offset: 50, limit: 25);
    /// 
    /// // To get next page offset:
    /// int nextOffset = result.HasMore ? 50 + result.NextOffset.Value : -1;
    /// </code>
    /// </remarks>
    public static async Task<PaginatedResult<T>> PageAsync<T>(
        this IAsyncEnumerable<T> source,
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await source
            .SkipAsync(offset, cancellationToken)
            .TakeWithPaginationAsync(limit, cancellationToken)
            .ConfigureAwait(false);
    }
}

/// <summary>
/// Result of a paginated query with MCP-friendly metadata.
/// This class is designed to be thread-safe for read operations.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public sealed class PaginatedResult<T>(List<T> items, bool hasMore, int? nextOffset)
{
    private readonly List<T> _items = items;

    /// <summary>
    /// The items in the current page (read-only view).
    /// </summary>
    public IReadOnlyList<T> Items => _items;

    /// <summary>
    /// Indicates if more results are available beyond this page.
    /// Per MCP best practices: pagination responses should include has_more.
    /// </summary>
    public bool HasMore { get; } = hasMore;

    /// <summary>
    /// The offset for retrieving the next page of results.
    /// Null if there are no more results.
    /// Per MCP best practices: pagination responses should include next_offset.
    /// </summary>
    public int? NextOffset { get; } = nextOffset;

    /// <summary>
    /// The number of items in the current result set.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Deconstructs the result for tuple-style usage.
    /// </summary>
    public void Deconstruct(out IReadOnlyList<T> items, out bool hasMore, out int? nextOffset)
    {
        items = Items;
        hasMore = HasMore;
        nextOffset = NextOffset;
    }
}