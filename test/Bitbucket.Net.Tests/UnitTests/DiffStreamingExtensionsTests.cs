#nullable enable

using Bitbucket.Net.Common.Mcp;
using Bitbucket.Net.Models.Core.Projects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

public class DiffStreamingExtensionsTests
{
    #region CountDiffLines Tests

    [Fact]
    public void CountDiffLines_NullHunks_ReturnsZero()
    {
        var diff = new Diff { Hunks = null! };

        var count = DiffStreamingExtensions.CountDiffLines(diff);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountDiffLines_EmptyHunks_ReturnsZero()
    {
        var diff = new Diff { Hunks = [] };

        var count = DiffStreamingExtensions.CountDiffLines(diff);

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountDiffLines_WithLines_ReturnsCorrectCount()
    {
        var diff = CreateDiffWithLines(10);

        var count = DiffStreamingExtensions.CountDiffLines(diff);

        Assert.Equal(10, count);
    }

    [Fact]
    public void CountDiffLines_MultipleHunks_SumsAllLines()
    {
        var diff = new Diff
        {
            Hunks =
            [
                CreateHunkWithLines(5),
                CreateHunkWithLines(3),
                CreateHunkWithLines(7)
            ]
        };

        var count = DiffStreamingExtensions.CountDiffLines(diff);

        Assert.Equal(15, count);
    }

    [Fact]
    public void CountDiffLines_HunksWithNullSegments_SkipsThem()
    {
        var diff = new Diff
        {
            Hunks =
            [
                CreateHunkWithLines(5),
                new DiffHunk { Segments = null! }
            ]
        };

        var count = DiffStreamingExtensions.CountDiffLines(diff);

        Assert.Equal(5, count);
    }

    #endregion

    #region StreamDiffsWithLimitsAsync Tests

    [Fact]
    public async Task StreamDiffsWithLimitsAsync_NoLimits_YieldsAllDiffs()
    {
        var diffs = CreateDiffsAsync(3, linesPerDiff: 10);

        var results = await ToListAsync(diffs.StreamDiffsWithLimitsAsync());

        Assert.Equal(3, results.Count);
        Assert.All(results, r => Assert.False(r.IsTruncated));
        Assert.All(results, r => Assert.False(r.IsPartial));
    }

    [Fact]
    public async Task StreamDiffsWithLimitsAsync_MaxFilesLimit_TruncatesAtLimit()
    {
        var diffs = CreateDiffsAsync(5, linesPerDiff: 10);

        var results = await ToListAsync(diffs.StreamDiffsWithLimitsAsync(maxFiles: 3));

        // Should have 3 diffs + 1 truncation marker
        Assert.Equal(4, results.Count);
        Assert.True(results.Last().IsTruncated);
        Assert.Equal("max_files_reached", results.Last().TruncationReason);
    }

    [Fact]
    public async Task StreamDiffsWithLimitsAsync_MaxLinesLimit_TruncatesAtLimit()
    {
        var diffs = CreateDiffsAsync(5, linesPerDiff: 10);

        var results = await ToListAsync(diffs.StreamDiffsWithLimitsAsync(maxLines: 25));

        // Should truncate after 2-3 diffs
        Assert.Contains(results, r => r.IsTruncated);
        Assert.Contains(results, r => r.TruncationReason == "max_lines_reached");
    }

    [Fact]
    public async Task StreamDiffsWithLimitsAsync_Empty_YieldsNothing()
    {
        var diffs = AsyncEnumerable.Empty<Diff>();

        var results = await ToListAsync(diffs.StreamDiffsWithLimitsAsync());

        Assert.Empty(results);
    }

    [Fact]
    public async Task StreamDiffsWithLimitsAsync_TracksTotals()
    {
        var diffs = CreateDiffsAsync(3, linesPerDiff: 10);

        var results = await ToListAsync(diffs.StreamDiffsWithLimitsAsync());

        Assert.Equal(10, results[0].TotalLines);
        Assert.Equal(1, results[0].TotalFiles);
        Assert.Equal(20, results[1].TotalLines);
        Assert.Equal(2, results[1].TotalFiles);
        Assert.Equal(30, results[2].TotalLines);
        Assert.Equal(3, results[2].TotalFiles);
    }

    #endregion

    #region TakeDiffsWithLimitsAsync Tests

    [Fact]
    public async Task TakeDiffsWithLimitsAsync_NoLimits_ReturnsAllDiffs()
    {
        var diffs = CreateDiffsAsync(3, linesPerDiff: 10);

        var result = await diffs.TakeDiffsWithLimitsAsync();

        Assert.Equal(3, result.Diffs.Count);
        Assert.Equal(30, result.TotalLines);
        Assert.Equal(3, result.TotalFiles);
        Assert.False(result.WasTruncated);
        Assert.False(result.HasMore);
        Assert.Null(result.TruncationReason);
    }

    [Fact]
    public async Task TakeDiffsWithLimitsAsync_MaxFilesLimit_TruncatesAtLimit()
    {
        var diffs = CreateDiffsAsync(5, linesPerDiff: 10);

        var result = await diffs.TakeDiffsWithLimitsAsync(maxFiles: 3);

        Assert.Equal(3, result.Diffs.Count);
        Assert.Equal(3, result.TotalFiles);
        Assert.True(result.WasTruncated);
        Assert.True(result.HasMore);
        Assert.Equal("max_files_reached", result.TruncationReason);
    }

    [Fact]
    public async Task TakeDiffsWithLimitsAsync_MaxLinesLimit_TruncatesAtLimit()
    {
        var diffs = CreateDiffsAsync(5, linesPerDiff: 10);

        var result = await diffs.TakeDiffsWithLimitsAsync(maxLines: 25);

        Assert.True(result.WasTruncated);
        Assert.True(result.HasMore);
        Assert.Equal("max_lines_reached", result.TruncationReason);
        Assert.True(result.TotalLines <= 25);
    }

    [Fact]
    public async Task TakeDiffsWithLimitsAsync_Empty_ReturnsEmpty()
    {
        var diffs = AsyncEnumerable.Empty<Diff>();

        var result = await diffs.TakeDiffsWithLimitsAsync();

        Assert.Empty(result.Diffs);
        Assert.Equal(0, result.TotalLines);
        Assert.Equal(0, result.TotalFiles);
        Assert.False(result.WasTruncated);
    }

    #endregion

    #region DiffPaginatedResult Tests

    [Fact]
    public void DiffPaginatedResult_Deconstruct_Works()
    {
        var diffs = new List<Diff> { CreateDiffWithLines(5) };
        var result = new DiffPaginatedResult(diffs, totalLines: 5, totalFiles: 1, wasTruncated: true, truncationReason: "test");

        var (returnedDiffs, hasMore, totalLines, totalFiles) = result;

        Assert.Single(returnedDiffs);
        Assert.True(hasMore);
        Assert.Equal(5, totalLines);
        Assert.Equal(1, totalFiles);
    }

    [Fact]
    public void DiffPaginatedResult_HasMore_MatchesWasTruncated()
    {
        var result1 = new DiffPaginatedResult([], totalLines: 0, totalFiles: 0, wasTruncated: true, truncationReason: "test");
        var result2 = new DiffPaginatedResult([], totalLines: 0, totalFiles: 0, wasTruncated: false, truncationReason: null);

        Assert.True(result1.HasMore);
        Assert.False(result2.HasMore);
    }

    #endregion

    #region Helper Methods

    private static Diff CreateDiffWithLines(int lineCount)
    {
        return new Diff
        {
            Source = new Path { Name = "test.cs" },
            Destination = new Path { Name = "test.cs" },
            Hunks = [CreateHunkWithLines(lineCount)]
        };
    }

    private static DiffHunk CreateHunkWithLines(int lineCount)
    {
        var lines = Enumerable.Range(1, lineCount)
            .Select(i => new LineRef { Line = $"Line {i}" })
            .ToList();

        return new DiffHunk
        {
            SourceLine = 1,
            SourceSpan = lineCount,
            DestinationLine = 1,
            DestinationSpan = lineCount,
            Segments =
            [
                new Segment
                {
                    Type = "CONTEXT",
                    Lines = lines
                }
            ]
        };
    }

    private static async IAsyncEnumerable<Diff> CreateDiffsAsync(int count, int linesPerDiff)
    {
        for (int i = 0; i < count; i++)
        {
            yield return CreateDiffWithLines(linesPerDiff);
            await Task.Yield();
        }
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }
        return list;
    }

    #endregion
}