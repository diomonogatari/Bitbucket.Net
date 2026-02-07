#nullable enable

using Bitbucket.Net.Common.Mcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class McpExtensionsTests
{
    #region TakeWithPaginationAsync Tests

    [Fact]
    public async Task TakeWithPaginationAsync_EmptySource_ReturnsEmptyResult()
    {
        var source = AsyncEnumerable.Empty<int>();

        var result = await source.TakeWithPaginationAsync(10);

        Assert.Empty(result.Items);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
        Assert.Equal(0, result.Count);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task TakeWithPaginationAsync_LessThanLimit_ReturnsAllItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeWithPaginationAsync(10);

        Assert.Equal(3, result.Items.Count);
        Assert.Equal(new[] { 1, 2, 3 }, result.Items);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3, 4, 5 };

    [Fact]
    public async Task TakeWithPaginationAsync_ExactlyLimit_ReturnsAllItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeWithPaginationAsync(5);

        Assert.Equal(5, result.Items.Count);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3, 4, 5, 6, 7 };

    [Fact]
    public async Task TakeWithPaginationAsync_MoreThanLimit_ReturnsLimitedItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeWithPaginationAsync(5);

        Assert.Equal(5, result.Items.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result.Items);
        Assert.True(result.HasMore);
        Assert.Equal(5, result.NextOffset);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task TakeWithPaginationAsync_AcceptsCancellationToken()
    {
        // Just verify the method accepts and passes through the cancellation token
        var cts = new CancellationTokenSource();
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeWithPaginationAsync(10, cts.Token);

        Assert.Equal(3, result.Items.Count);
    }

    #endregion

    #region TakeAsync Tests

    [Fact]
    public async Task TakeAsync_EmptySource_YieldsNothing()
    {
        var source = AsyncEnumerable.Empty<int>();

        var result = await source.TakeAsync(10).ToListAsync();

        Assert.Empty(result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task TakeAsync_LessThanLimit_YieldsAllItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeAsync(10).ToListAsync();

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3, 4, 5, 6, 7 };

    [Fact]
    public async Task TakeAsync_MoreThanLimit_YieldsLimitedItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeAsync(3).ToListAsync();

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task TakeAsync_ZeroLimit_YieldsNothing()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.TakeAsync(0).ToListAsync();

        Assert.Empty(result);
    }

    #endregion

    #region SkipAsync Tests

    [Fact]
    public async Task SkipAsync_EmptySource_YieldsNothing()
    {
        var source = AsyncEnumerable.Empty<int>();

        var result = await source.SkipAsync(5).ToListAsync();

        Assert.Empty(result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3, 4, 5 };

    [Fact]
    public async Task SkipAsync_SkipLessThanCount_YieldsRemaining()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.SkipAsync(2).ToListAsync();

        Assert.Equal(new[] { 3, 4, 5 }, result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task SkipAsync_SkipMoreThanCount_YieldsNothing()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.SkipAsync(10).ToListAsync();

        Assert.Empty(result);
    }

    private static readonly int[] int32Array = new[] { 1, 2, 3 };

    [Fact]
    public async Task SkipAsync_SkipZero_YieldsAllItems()
    {
        var source = CreateAsyncEnumerable(int32Array);

        var result = await source.SkipAsync(0).ToListAsync();

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    #endregion

    #region PageAsync Tests

    [Fact]
    public async Task PageAsync_FirstPage_ReturnsCorrectItems()
    {
        var source = CreateAsyncEnumerable(Enumerable.Range(1, 100));

        var result = await source.PageAsync(offset: 0, limit: 10);

        Assert.Equal(Enumerable.Range(1, 10), result.Items);
        Assert.True(result.HasMore);
        Assert.Equal(10, result.NextOffset);
    }

    [Fact]
    public async Task PageAsync_MiddlePage_ReturnsCorrectItems()
    {
        var source = CreateAsyncEnumerable(Enumerable.Range(1, 100));

        var result = await source.PageAsync(offset: 20, limit: 10);

        Assert.Equal(Enumerable.Range(21, 10), result.Items);
        Assert.True(result.HasMore);
        Assert.Equal(10, result.NextOffset);
    }

    [Fact]
    public async Task PageAsync_LastPage_ReturnsRemainingItems()
    {
        var source = CreateAsyncEnumerable(Enumerable.Range(1, 25));

        var result = await source.PageAsync(offset: 20, limit: 10);

        Assert.Equal(Enumerable.Range(21, 5), result.Items);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    [Fact]
    public async Task PageAsync_BeyondEnd_ReturnsEmpty()
    {
        var source = CreateAsyncEnumerable(Enumerable.Range(1, 10));

        var result = await source.PageAsync(offset: 100, limit: 10);

        Assert.Empty(result.Items);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    #endregion

    #region PaginatedResult Tests

    [Fact]
    public void PaginatedResult_Count_ReturnsItemCount()
    {
        var result = new PaginatedResult<int>([1, 2, 3], hasMore: true, nextOffset: 3);

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void PaginatedResult_Deconstruct_Works()
    {
        var result = new PaginatedResult<int>([1, 2], hasMore: true, nextOffset: 2);

        var (items, hasMore, nextOffset) = result;

        Assert.Equal(2, items.Count);
        Assert.True(hasMore);
        Assert.Equal(2, nextOffset);
    }

    [Fact]
    public void PaginatedResult_Items_IsReadOnly()
    {
        var result = new PaginatedResult<int>([1, 2, 3], hasMore: false, nextOffset: null);

        Assert.IsType<IReadOnlyList<int>>(result.Items, exactMatch: false);
    }

    #endregion

    #region Helper Methods

    private static async IAsyncEnumerable<T> CreateAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            yield return item;
            await Task.Yield(); // Simulate async behavior
        }
    }

    #endregion
}

internal static class AsyncEnumerableTestExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }
}