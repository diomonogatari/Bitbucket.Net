using Bitbucket.Net.Models.Core.Projects;
using System.Runtime.CompilerServices;

namespace Bitbucket.Net.Common.Mcp;

/// <summary>
/// MCP-optimized diff streaming extensions for context window management.
/// Diffs are typically the largest response payloads in MCP usage (100KB-10MB+).
/// These extensions provide line-count-aware streaming with early termination.
/// </summary>
public static class DiffStreamingExtensions
{
    /// <summary>
    /// Streams diff hunks from an async enumerable of diffs with line and file limits.
    /// Enables early termination when MCP context window limits are reached.
    /// </summary>
    /// <param name="diffs">The async enumerable of diffs to process.</param>
    /// <param name="maxLines">Maximum total lines to yield across all diffs. Null for unlimited.</param>
    /// <param name="maxFiles">Maximum number of files to process. Null for unlimited.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of diff results with truncation metadata.</returns>
    public static async IAsyncEnumerable<DiffStreamResult> StreamDiffsWithLimitsAsync(
        this IAsyncEnumerable<Diff> diffs,
        int? maxLines = null,
        int? maxFiles = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int totalLines = 0;
        int totalFiles = 0;

        await foreach (var diff in diffs.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            // Check file limit
            if (maxFiles.HasValue && totalFiles >= maxFiles.Value)
            {
                yield return DiffStreamResult.CreateTruncated(totalLines, totalFiles, "max_files_reached");
                yield break;
            }

            int diffLineCount = CountDiffLines(diff);

            // Check if this diff would exceed line limit
            if (maxLines.HasValue && totalLines + diffLineCount > maxLines.Value)
            {
                // Calculate how many lines we can still include
                int remainingLines = maxLines.Value - totalLines;

                if (remainingLines > 0)
                {
                    // Truncate this diff and yield partial result
                    var truncatedDiff = TruncateDiff(diff, remainingLines);
                    yield return DiffStreamResult.CreatePartial(truncatedDiff, totalLines + remainingLines, totalFiles + 1);
                }

                yield return DiffStreamResult.CreateTruncated(totalLines + remainingLines, totalFiles + 1, "max_lines_reached");
                yield break;
            }

            totalLines += diffLineCount;
            totalFiles++;

            yield return DiffStreamResult.Create(diff, totalLines, totalFiles);
        }
    }

    /// <summary>
    /// Takes diffs up to specified line and file limits, returning pagination metadata.
    /// </summary>
    /// <param name="diffs">The async enumerable of diffs to process.</param>
    /// <param name="maxLines">Maximum total lines. Null for unlimited.</param>
    /// <param name="maxFiles">Maximum number of files. Null for unlimited.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the diffs and truncation metadata.</returns>
    public static async Task<DiffPaginatedResult> TakeDiffsWithLimitsAsync(
        this IAsyncEnumerable<Diff> diffs,
        int? maxLines = null,
        int? maxFiles = null,
        CancellationToken cancellationToken = default)
    {
        var collectedDiffs = new List<Diff>();
        int totalLines = 0;
        int totalFiles = 0;
        bool wasTruncated = false;
        string? truncationReason = null;

        await foreach (var diff in diffs.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            // Check file limit
            if (maxFiles.HasValue && totalFiles >= maxFiles.Value)
            {
                wasTruncated = true;
                truncationReason = "max_files_reached";
                break;
            }

            int diffLineCount = CountDiffLines(diff);

            // Check if this diff would exceed line limit
            if (maxLines.HasValue && totalLines + diffLineCount > maxLines.Value)
            {
                int remainingLines = maxLines.Value - totalLines;

                if (remainingLines > 0)
                {
                    var truncatedDiff = TruncateDiff(diff, remainingLines);
                    collectedDiffs.Add(truncatedDiff);
                    totalLines += remainingLines;
                    totalFiles++;
                }

                wasTruncated = true;
                truncationReason = "max_lines_reached";
                break;
            }

            collectedDiffs.Add(diff);
            totalLines += diffLineCount;
            totalFiles++;
        }

        return new DiffPaginatedResult(
            collectedDiffs,
            totalLines,
            totalFiles,
            wasTruncated,
            truncationReason);
    }

    /// <summary>
    /// Counts the total number of lines in a diff.
    /// </summary>
    public static int CountDiffLines(Diff diff)
    {
        if (diff.Hunks == null)
            return 0;

        return diff.Hunks.Sum(hunk =>
            hunk.Segments?.Sum(segment => segment.Lines?.Count ?? 0) ?? 0);
    }

    private static Diff TruncateDiff(Diff original, int maxLines)
    {
        if (original.Hunks == null || maxLines <= 0)
        {
            return new Diff
            {
                Source = original.Source,
                Destination = original.Destination,
                Hunks = [],
            };
        }

        var truncatedHunks = new List<DiffHunk>();
        int linesRemaining = maxLines;

        foreach (var hunk in original.Hunks)
        {
            if (linesRemaining <= 0)
                break;

            var truncatedHunk = TruncateHunk(hunk, linesRemaining);
            truncatedHunks.Add(truncatedHunk);

            int hunkLines = truncatedHunk.Segments?.Sum(s => s.Lines?.Count ?? 0) ?? 0;
            linesRemaining -= hunkLines;
        }

        return new Diff
        {
            Source = original.Source,
            Destination = original.Destination,
            Hunks = truncatedHunks,
        };
    }

    private static DiffHunk TruncateHunk(DiffHunk original, int maxLines)
    {
        if (original.Segments == null || maxLines <= 0)
        {
            return new DiffHunk
            {
                SourceLine = original.SourceLine,
                SourceSpan = original.SourceSpan,
                DestinationLine = original.DestinationLine,
                DestinationSpan = original.DestinationSpan,
                Segments = [],
                Truncated = true,
            };
        }

        var truncatedSegments = new List<Segment>();
        int linesRemaining = maxLines;

        foreach (var segment in original.Segments)
        {
            if (linesRemaining <= 0)
                break;

            var truncatedSegment = TruncateSegment(segment, linesRemaining);
            truncatedSegments.Add(truncatedSegment);

            int segmentLines = truncatedSegment.Lines?.Count ?? 0;
            linesRemaining -= segmentLines;
        }

        return new DiffHunk
        {
            SourceLine = original.SourceLine,
            SourceSpan = original.SourceSpan,
            DestinationLine = original.DestinationLine,
            DestinationSpan = original.DestinationSpan,
            Segments = truncatedSegments,
            Truncated = linesRemaining <= 0 || original.Truncated,
        };
    }

    private static Segment TruncateSegment(Segment original, int maxLines)
    {
        if (original.Lines == null || maxLines <= 0)
        {
            return new Segment
            {
                Type = original.Type,
                Lines = [],
                Truncated = true,
            };
        }

        int linesToTake = Math.Min(original.Lines.Count, maxLines);
        bool needsTruncation = linesToTake < original.Lines.Count;

        return new Segment
        {
            Type = original.Type,
            Lines = [.. original.Lines.Take(linesToTake)],
            Truncated = needsTruncation || original.Truncated,
        };
    }
}

/// <summary>
/// Result of streaming a single diff with metadata.
/// </summary>
public sealed class DiffStreamResult
{
    /// <summary>
    /// The diff content. Null if this is a truncation marker.
    /// </summary>
    public Diff? Diff { get; }

    /// <summary>
    /// Total lines yielded so far (including this diff).
    /// </summary>
    public int TotalLines { get; }

    /// <summary>
    /// Total files yielded so far (including this diff).
    /// </summary>
    public int TotalFiles { get; }

    /// <summary>
    /// True if this diff was partially truncated.
    /// </summary>
    public bool IsPartial { get; }

    /// <summary>
    /// True if streaming was truncated after this result.
    /// </summary>
    public bool IsTruncated { get; }

    /// <summary>
    /// Reason for truncation, if applicable.
    /// </summary>
    public string? TruncationReason { get; }

    private DiffStreamResult(Diff? diff, int totalLines, int totalFiles, bool isPartial, bool isTruncated, string? truncationReason)
    {
        Diff = diff;
        TotalLines = totalLines;
        TotalFiles = totalFiles;
        IsPartial = isPartial;
        IsTruncated = isTruncated;
        TruncationReason = truncationReason;
    }

    internal static DiffStreamResult Create(Diff diff, int totalLines, int totalFiles)
        => new(diff, totalLines, totalFiles, isPartial: false, isTruncated: false, truncationReason: null);

    internal static DiffStreamResult CreatePartial(Diff diff, int totalLines, int totalFiles)
        => new(diff, totalLines, totalFiles, isPartial: true, isTruncated: false, truncationReason: null);

    internal static DiffStreamResult CreateTruncated(int totalLines, int totalFiles, string reason)
        => new(diff: null, totalLines, totalFiles, isPartial: false, isTruncated: true, truncationReason: reason);
}

/// <summary>
/// Result of taking diffs with limits, including truncation metadata.
/// This class is designed to be thread-safe for read operations.
/// </summary>
public sealed class DiffPaginatedResult(List<Diff> diffs, int totalLines, int totalFiles, bool wasTruncated, string? truncationReason)
{
    private readonly List<Diff> _diffs = diffs;

    /// <summary>
    /// The collected diffs (may be truncated). Read-only view.
    /// </summary>
    public IReadOnlyList<Diff> Diffs => _diffs;

    /// <summary>
    /// Total lines in the result.
    /// </summary>
    public int TotalLines { get; } = totalLines;

    /// <summary>
    /// Total files in the result.
    /// </summary>
    public int TotalFiles { get; } = totalFiles;

    /// <summary>
    /// True if the result was truncated due to limits.
    /// </summary>
    public bool WasTruncated { get; } = wasTruncated;

    /// <summary>
    /// Reason for truncation, if applicable. Values: "max_lines_reached", "max_files_reached".
    /// </summary>
    public string? TruncationReason { get; } = truncationReason;

    /// <summary>
    /// Per MCP best practices, indicates if more results exist.
    /// </summary>
    public bool HasMore => WasTruncated;

    /// <summary>
    /// Deconstructs the result for tuple-style usage.
    /// </summary>
    public void Deconstruct(out IReadOnlyList<Diff> diffs, out bool hasMore, out int totalLines, out int totalFiles)
    {
        diffs = Diffs;
        hasMore = HasMore;
        totalLines = TotalLines;
        totalFiles = TotalFiles;
    }
}