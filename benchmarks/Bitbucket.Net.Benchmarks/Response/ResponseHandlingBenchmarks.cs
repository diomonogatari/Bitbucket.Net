using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using System.Text;

namespace Bitbucket.Net.Benchmarks.Response;

/// <summary>
/// Benchmarks for handling large response payloads like diffs and raw file content.
/// Demonstrates the benefits of streaming large content vs buffering.
/// </summary>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
public class ResponseHandlingBenchmarks
{
    private string _smallDiff = null!;
    private string _mediumDiff = null!;
    private string _largeDiff = null!;
    private string _smallFile = null!;
    private string _largeFile = null!;
    private byte[] _largeFileBytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallDiff = GenerateDiff(10);        // ~10 lines changed
        _mediumDiff = GenerateDiff(100);      // ~100 lines changed
        _largeDiff = GenerateDiff(1000);      // ~1000 lines changed

        _smallFile = GenerateFileContent(100);    // 100 lines
        _largeFile = GenerateFileContent(10000);  // 10,000 lines
        _largeFileBytes = Encoding.UTF8.GetBytes(_largeFile);
    }

    #region Diff Processing Benchmarks

    [Benchmark(Description = "Process small diff (~10 lines)")]
    public int ProcessSmallDiff()
    {
        return ProcessDiffContent(_smallDiff);
    }

    [Benchmark(Description = "Process medium diff (~100 lines)")]
    public int ProcessMediumDiff()
    {
        return ProcessDiffContent(_mediumDiff);
    }

    [Benchmark(Description = "Process large diff (~1000 lines)")]
    public int ProcessLargeDiff()
    {
        return ProcessDiffContent(_largeDiff);
    }

    /// <summary>
    /// Simulates buffered diff processing - loads entire diff into memory.
    /// </summary>
    [Benchmark(Description = "Buffered diff processing (1000 lines)")]
    public List<string> BufferedDiffProcessing()
    {
        return [.. _largeDiff.Split('\n')];
    }

    /// <summary>
    /// Simulates streaming diff processing - processes line by line.
    /// </summary>
    [Benchmark(Description = "Streaming diff processing (1000 lines)")]
    public int StreamingDiffProcessing()
    {
        int lineCount = 0;
        int additions = 0;
        int deletions = 0;

        foreach (var line in EnumerateLines(_largeDiff))
        {
            lineCount++;
            if (line.StartsWith('+') && !line.StartsWith("+++"))
                additions++;
            else if (line.StartsWith('-') && !line.StartsWith("---"))
                deletions++;
        }

        return lineCount;
    }

    #endregion

    #region File Content Processing Benchmarks

    [Benchmark(Description = "Read small file content (100 lines)")]
    public string ReadSmallFileContent()
    {
        return _smallFile;
    }

    [Benchmark(Description = "Read large file content (10K lines)")]
    public string ReadLargeFileContent()
    {
        return _largeFile;
    }

    /// <summary>
    /// Simulates string-based file content handling (common approach).
    /// </summary>
    [Benchmark(Description = "String-based file handling")]
    public int StringBasedFileHandling()
    {
        var content = _largeFile;
        return content.Length;
    }

    /// <summary>
    /// Simulates byte-based file content handling (more efficient for binary).
    /// </summary>
    [Benchmark(Description = "Byte-based file handling")]
    public int ByteBasedFileHandling()
    {
        var content = _largeFileBytes;
        return content.Length;
    }

    /// <summary>
    /// Simulates streaming file to disk (chunked writing).
    /// </summary>
    [Benchmark(Description = "Chunked file processing (4KB chunks)")]
    public int ChunkedFileProcessing()
    {
        const int chunkSize = 4096;
        int bytesProcessed = 0;
        var span = _largeFileBytes.AsSpan();

        while (bytesProcessed < span.Length)
        {
            var chunk = span.Slice(bytesProcessed, Math.Min(chunkSize, span.Length - bytesProcessed));
            bytesProcessed += chunk.Length;
            // Simulate processing chunk
        }

        return bytesProcessed;
    }

    #endregion

    #region Memory Pressure Benchmarks

    [Benchmark(Description = "Multiple small allocations (100x small diff)")]
    public int MultipleSmallAllocations()
    {
        int total = 0;
        for (int i = 0; i < 100; i++)
        {
            var lines = _smallDiff.Split('\n');
            total += lines.Length;
        }
        return total;
    }

    [Benchmark(Description = "Reuse StringBuilder for concatenation")]
    public string ReuseStringBuilder()
    {
        var sb = new StringBuilder();
        foreach (var line in EnumerateLines(_mediumDiff))
        {
            sb.AppendLine(line);
        }
        return sb.ToString();
    }

    [Benchmark(Description = "String concatenation (inefficient)")]
    public string StringConcatenation()
    {
        string result = "";
        int count = 0;
        foreach (var line in EnumerateLines(_smallDiff))
        {
            result += line + "\n";
            if (++count > 10) break; // Limit to avoid extremely slow benchmark
        }
        return result;
    }

    #endregion

    #region Helper Methods

    private static int ProcessDiffContent(string diff)
    {
        int additions = 0;
        int deletions = 0;

        foreach (var line in EnumerateLines(diff))
        {
            if (line.StartsWith('+') && !line.StartsWith("+++"))
                additions++;
            else if (line.StartsWith('-') && !line.StartsWith("---"))
                deletions++;
        }

        return additions + deletions;
    }

    private static IEnumerable<string> EnumerateLines(string content)
    {
        int start = 0;
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] == '\n')
            {
                yield return content[start..i];
                start = i + 1;
            }
        }

        if (start < content.Length)
        {
            yield return content[start..];
        }
    }

    private static string GenerateDiff(int lineChanges)
    {
        var sb = new StringBuilder();
        sb.AppendLine("diff --git a/src/file.cs b/src/file.cs");
        sb.AppendLine("index abc1234..def5678 100644");
        sb.AppendLine("--- a/src/file.cs");
        sb.AppendLine("+++ b/src/file.cs");
        sb.AppendLine("@@ -1,100 +1,100 @@");

        for (int i = 0; i < lineChanges; i++)
        {
            if (i % 3 == 0)
            {
                sb.AppendLine($"-    // Old line {i}");
                sb.AppendLine($"+    // New line {i}");
            }
            else if (i % 3 == 1)
            {
                sb.AppendLine($"+    // Added line {i}");
            }
            else
            {
                sb.AppendLine($"     // Context line {i}");
            }
        }

        return sb.ToString();
    }

    private static string GenerateFileContent(int lines)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("namespace BenchmarkData;");
        sb.AppendLine();
        sb.AppendLine("public class GeneratedFile");
        sb.AppendLine("{");

        for (int i = 0; i < lines - 10; i++)
        {
            if (i % 20 == 0)
            {
                sb.AppendLine();
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// Method {i / 20} documentation.");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    public void Method{i / 20}()");
                sb.AppendLine("    {");
            }
            else if (i % 20 == 19)
            {
                sb.AppendLine("    }");
            }
            else
            {
                sb.AppendLine($"        var line{i} = \"Content for line {i}\";");
            }
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    #endregion
}