using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Bitbucket.Net.Common.Mcp;
using Bitbucket.Net.Models.Core.Projects;
using Xunit;

namespace Bitbucket.Net.Tests.Common.Mcp
{
    public class DiffStreamingExtensionsTests
    {
        #region CountDiffLines Tests

        [Fact]
        public void CountDiffLines_WithNullHunks_ReturnsZero()
        {
            // Arrange
            var diff = new Diff { Hunks = null };

            // Act
            var count = DiffStreamingExtensions.CountDiffLines(diff);

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void CountDiffLines_WithEmptyHunks_ReturnsZero()
        {
            // Arrange
            var diff = new Diff { Hunks = [] };

            // Act
            var count = DiffStreamingExtensions.CountDiffLines(diff);

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void CountDiffLines_WithMultipleHunksAndSegments_CountsAllLines()
        {
            // Arrange
            var diff = CreateDiff(hunks: 2, segmentsPerHunk: 3, linesPerSegment: 5);

            // Act
            var count = DiffStreamingExtensions.CountDiffLines(diff);

            // Assert
            Assert.Equal(30, count); // 2 * 3 * 5 = 30
        }

        #endregion

        #region TakeDiffsWithLimitsAsync Tests

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_WithNoLimits_ReturnsAllDiffs()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(5, linesPerDiff: 10);

            // Act
            var result = await diffs.TakeDiffsWithLimitsAsync();

            // Assert
            Assert.Equal(5, result.Diffs.Count);
            Assert.Equal(50, result.TotalLines);
            Assert.Equal(5, result.TotalFiles);
            Assert.False(result.WasTruncated);
            Assert.Null(result.TruncationReason);
        }

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_WithMaxFiles_TruncatesAtFileLimit()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(10, linesPerDiff: 5);

            // Act
            var result = await diffs.TakeDiffsWithLimitsAsync(maxFiles: 3);

            // Assert
            Assert.Equal(3, result.Diffs.Count);
            Assert.Equal(3, result.TotalFiles);
            Assert.True(result.WasTruncated);
            Assert.Equal("max_files_reached", result.TruncationReason);
            Assert.True(result.HasMore);
        }

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_WithMaxLines_TruncatesAtLineLimit()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(5, linesPerDiff: 20);

            // Act
            var result = await diffs.TakeDiffsWithLimitsAsync(maxLines: 35);

            // Assert
            Assert.True(result.TotalLines <= 35);
            Assert.True(result.WasTruncated);
            Assert.Equal("max_lines_reached", result.TruncationReason);
        }

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_WithBothLimits_RespectsFirstHit()
        {
            // Arrange - 10 diffs with 20 lines each = 200 total lines
            var diffs = CreateAsyncDiffs(10, linesPerDiff: 20);

            // Act - max 3 files OR max 100 lines (file limit should hit first)
            var result = await diffs.TakeDiffsWithLimitsAsync(maxLines: 100, maxFiles: 3);

            // Assert
            Assert.Equal(3, result.TotalFiles);
            Assert.Equal("max_files_reached", result.TruncationReason);
        }

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_SupportsDeconstruction()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(3, linesPerDiff: 10);

            // Act
            var (diffList, hasMore, totalLines, totalFiles) = await diffs.TakeDiffsWithLimitsAsync(maxFiles: 2);

            // Assert
            Assert.Equal(2, diffList.Count);
            Assert.True(hasMore);
            Assert.Equal(20, totalLines);
            Assert.Equal(2, totalFiles);
        }

        #endregion

        #region StreamDiffsWithLimitsAsync Tests

        [Fact]
        public async Task StreamDiffsWithLimitsAsync_WithNoLimits_YieldsAllDiffs()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(5, linesPerDiff: 10);

            // Act
            var results = new List<DiffStreamResult>();
            await foreach (var result in diffs.StreamDiffsWithLimitsAsync())
            {
                results.Add(result);
            }

            // Assert
            Assert.Equal(5, results.Count);
            Assert.All(results, r => Assert.NotNull(r.Diff));
            Assert.All(results, r => Assert.False(r.IsTruncated));
        }

        [Fact]
        public async Task StreamDiffsWithLimitsAsync_WithMaxFiles_YieldsTruncationMarker()
        {
            // Arrange
            var diffs = CreateAsyncDiffs(10, linesPerDiff: 5);

            // Act
            var results = new List<DiffStreamResult>();
            await foreach (var result in diffs.StreamDiffsWithLimitsAsync(maxFiles: 3))
            {
                results.Add(result);
            }

            // Assert
            Assert.Equal(4, results.Count); // 3 diffs + 1 truncation marker
            Assert.True(results.Last().IsTruncated);
            Assert.Equal("max_files_reached", results.Last().TruncationReason);
        }

        [Fact]
        public async Task StreamDiffsWithLimitsAsync_TracksRunningTotals()
        {
            // Arrange - 3 diffs with 10 lines each
            var diffs = CreateAsyncDiffs(3, linesPerDiff: 10);

            // Act
            var results = new List<DiffStreamResult>();
            await foreach (var result in diffs.StreamDiffsWithLimitsAsync())
            {
                results.Add(result);
            }

            // Assert
            Assert.Equal(10, results[0].TotalLines);
            Assert.Equal(1, results[0].TotalFiles);

            Assert.Equal(20, results[1].TotalLines);
            Assert.Equal(2, results[1].TotalFiles);

            Assert.Equal(30, results[2].TotalLines);
            Assert.Equal(3, results[2].TotalFiles);
        }

        #endregion

        #region Cancellation Tests

        [Fact]
        public async Task TakeDiffsWithLimitsAsync_RespectsCancellation()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            var diffs = CreateCancellableAsyncDiffs(100, linesPerDiff: 10, cts.Token);

            // Cancel after a short delay
            _ = Task.Run(async () =>
            {
                await Task.Delay(10);
                cts.Cancel();
            });

            // Act & Assert - TaskCanceledException inherits from OperationCanceledException
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => diffs.TakeDiffsWithLimitsAsync(cancellationToken: cts.Token));
        }

        #endregion

        #region Helper Methods

        private static Diff CreateDiff(int hunks = 1, int segmentsPerHunk = 1, int linesPerSegment = 10)
        {
            return new Diff
            {
                Source = new Path { toString = "source.cs" },
                Destination = new Path { toString = "dest.cs" },
                Hunks = Enumerable.Range(0, hunks).Select(_ => new DiffHunk
                {
                    SourceLine = 1,
                    SourceSpan = linesPerSegment * segmentsPerHunk,
                    DestinationLine = 1,
                    DestinationSpan = linesPerSegment * segmentsPerHunk,
                    Segments = Enumerable.Range(0, segmentsPerHunk).Select(_ => new Segment
                    {
                        Type = "CONTEXT",
                        Lines = Enumerable.Range(0, linesPerSegment).Select(i => new LineRef
                        {
                            Source = i + 1,
                            Destination = i + 1,
                            Line = $"Line {i + 1}"
                        }).ToList()
                    }).ToList()
                }).ToList()
            };
        }

        private static async IAsyncEnumerable<Diff> CreateAsyncDiffs(int count, int linesPerDiff)
        {
            // Calculate how to distribute lines: single hunk, single segment
            for (int i = 0; i < count; i++)
            {
                await Task.Yield();
                yield return CreateDiff(hunks: 1, segmentsPerHunk: 1, linesPerSegment: linesPerDiff);
            }
        }

        private static async IAsyncEnumerable<Diff> CreateCancellableAsyncDiffs(
            int count,
            int linesPerDiff,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(5, cancellationToken);
                yield return CreateDiff(hunks: 1, segmentsPerHunk: 1, linesPerSegment: linesPerDiff);
            }
        }

        #endregion
    }
}
