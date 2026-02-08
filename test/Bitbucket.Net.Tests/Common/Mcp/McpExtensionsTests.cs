using Bitbucket.Net.Common.Mcp;
using System.Runtime.CompilerServices;
using Xunit;

namespace Bitbucket.Net.Tests.Common.Mcp;

public class McpExtensionsTests
{
    #region TakeWithPaginationAsync Tests

    [Fact]
    public async Task TakeWithPaginationAsync_WithFewerItemsThanLimit_ReturnsAllItems()
    {
        // Arrange
        var source = CreateAsyncEnumerable(5);

        // Act
        var result = await source.TakeWithPaginationAsync(10);

        // Assert
        Assert.Equal(5, result.Items.Count);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    [Fact]
    public async Task TakeWithPaginationAsync_WithExactlyLimitItems_ReturnsAllItemsNoMore()
    {
        // Arrange
        var source = CreateAsyncEnumerable(10);

        // Act
        var result = await source.TakeWithPaginationAsync(10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    [Fact]
    public async Task TakeWithPaginationAsync_WithMoreItemsThanLimit_ReturnsLimitWithHasMore()
    {
        // Arrange
        var source = CreateAsyncEnumerable(20);

        // Act
        var result = await source.TakeWithPaginationAsync(10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.True(result.HasMore);
        Assert.Equal(10, result.NextOffset);
    }

    [Fact]
    public async Task TakeWithPaginationAsync_WithEmptySource_ReturnsEmptyResult()
    {
        // Arrange
        var source = CreateAsyncEnumerable(0);

        // Act
        var result = await source.TakeWithPaginationAsync(10);

        // Assert
        Assert.Empty(result.Items);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    [Fact]
    public async Task TakeWithPaginationAsync_SupportsDeconstruction()
    {
        // Arrange
        var source = CreateAsyncEnumerable(15);

        // Act
        var (items, hasMore, nextOffset) = await source.TakeWithPaginationAsync(10);

        // Assert
        Assert.Equal(10, items.Count);
        Assert.True(hasMore);
        Assert.Equal(10, nextOffset);
    }

    [Fact]
    public async Task TakeWithPaginationAsync_RespectsItemOrder()
    {
        // Arrange
        var source = CreateAsyncEnumerable(5);

        // Act
        var result = await source.TakeWithPaginationAsync(5);

        // Assert
        Assert.Equal(new[] { 0, 1, 2, 3, 4 }, result.Items);
    }

    #endregion

    #region TakeAsync Tests

    [Fact]
    public async Task TakeAsync_WithFewerItemsThanLimit_ReturnsAllItems()
    {
        // Arrange
        var source = CreateAsyncEnumerable(5);

        // Act
        var result = await source.TakeAsync(10).ToListAsync();

        // Assert
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public async Task TakeAsync_WithMoreItemsThanLimit_ReturnsExactlyLimit()
    {
        // Arrange
        var source = CreateAsyncEnumerable(100);

        // Act
        var result = await source.TakeAsync(10).ToListAsync();

        // Assert
        Assert.Equal(10, result.Count);
        Assert.Equal([.. Enumerable.Range(0, 10)], result);
    }

    [Fact]
    public async Task TakeAsync_StopsEnumerationAfterLimit()
    {
        // Arrange
        int itemsGenerated = 0;
        var source = CreateCountingAsyncEnumerable(100, () => itemsGenerated++);

        // Act
        var result = await source.TakeAsync(10).ToListAsync();

        // Assert
        Assert.Equal(10, result.Count);
        // Iterator may request one more item to check if enumeration should continue
        // The important thing is we don't enumerate all 100 items
        Assert.True(itemsGenerated <= 11, $"Expected at most 11 items generated, but got {itemsGenerated}");
    }

    #endregion

    #region SkipAsync Tests

    [Fact]
    public async Task SkipAsync_WithValidOffset_SkipsCorrectItems()
    {
        // Arrange
        var source = CreateAsyncEnumerable(10);

        // Act
        var result = await source.SkipAsync(5).ToListAsync();

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal(new[] { 5, 6, 7, 8, 9 }, result);
    }

    [Fact]
    public async Task SkipAsync_WithOffsetGreaterThanCount_ReturnsEmpty()
    {
        // Arrange
        var source = CreateAsyncEnumerable(5);

        // Act
        var result = await source.SkipAsync(10).ToListAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SkipAsync_WithZeroOffset_ReturnsAllItems()
    {
        // Arrange
        var source = CreateAsyncEnumerable(5);

        // Act
        var result = await source.SkipAsync(0).ToListAsync();

        // Assert
        Assert.Equal(5, result.Count);
    }

    #endregion

    #region PageAsync Tests

    [Fact]
    public async Task PageAsync_ReturnsCorrectPage()
    {
        // Arrange
        var source = CreateAsyncEnumerable(100);

        // Act - Get page 3 (offset 20, limit 10)
        var result = await source.PageAsync(offset: 20, limit: 10);

        // Assert
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(Enumerable.Range(20, 10).ToList(), result.Items);
        Assert.True(result.HasMore);
        Assert.Equal(10, result.NextOffset); // Relative to the page, not absolute
    }

    [Fact]
    public async Task PageAsync_LastPage_HasMoreIsFalse()
    {
        // Arrange
        var source = CreateAsyncEnumerable(25);

        // Act - Get last page (offset 20, limit 10 but only 5 items left)
        var result = await source.PageAsync(offset: 20, limit: 10);

        // Assert
        Assert.Equal(5, result.Items.Count);
        Assert.False(result.HasMore);
        Assert.Null(result.NextOffset);
    }

    [Fact]
    public async Task PageAsync_BeyondData_ReturnsEmpty()
    {
        // Arrange
        var source = CreateAsyncEnumerable(10);

        // Act
        var result = await source.PageAsync(offset: 20, limit: 10);

        // Assert
        Assert.Empty(result.Items);
        Assert.False(result.HasMore);
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task TakeWithPaginationAsync_RespecsCancellation()
    {
        // Arrange - create source that checks cancellation
        using var cts = new CancellationTokenSource();
        var source = CreateCancellableAsyncEnumerable(100, cts.Token);

        // Cancel after a short delay
        _ = Task.Run(async () =>
        {
            await Task.Delay(10);
            await cts.CancelAsync();
        });

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => source.TakeWithPaginationAsync(100, cts.Token));
    }

    [Fact]
    public async Task TakeAsync_RespectsCancellation()
    {
        // Arrange - create source that checks cancellation
        using var cts = new CancellationTokenSource();
        var source = CreateCancellableAsyncEnumerable(100, cts.Token);

        // Cancel after a short delay
        _ = Task.Run(async () =>
        {
            await Task.Delay(10);
            await cts.CancelAsync();
        });

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await source.TakeAsync(100, cts.Token).ToListAsync());
    }

    #endregion

    #region Helper Methods

    private static async IAsyncEnumerable<int> CreateAsyncEnumerable(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await Task.Yield(); // Simulate async operation
            yield return i;
        }
    }

    private static async IAsyncEnumerable<int> CreateCountingAsyncEnumerable(int count, System.Action onGenerate)
    {
        for (int i = 0; i < count; i++)
        {
            onGenerate();
            await Task.Yield();
            yield return i;
        }
    }

    private static async IAsyncEnumerable<int> CreateCancellableAsyncEnumerable(
        int count,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(5, cancellationToken); // Small delay to allow cancellation to propagate
            yield return i;
        }
    }

    #endregion
}

// Helper extension for tests
internal static class AsyncEnumerableExtensions
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