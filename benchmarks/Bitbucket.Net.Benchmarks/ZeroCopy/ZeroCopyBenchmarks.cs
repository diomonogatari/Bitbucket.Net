using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Benchmarks.ZeroCopy;

/// <summary>
/// Benchmarks measuring the benefits of zero-copy patterns implemented in v2.0.0:
/// 1. ArrayPool&lt;byte&gt; for file upload buffers (vs new byte[] allocation)
/// 2. JsonElement.Deserialize&lt;T&gt;() for streaming JSON (vs GetRawText() + Deserialize)
/// 
/// These patterns reduce heap allocations and GC pressure in high-throughput scenarios.
/// </summary>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
[GcServer(true)]
public class ZeroCopyBenchmarks
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // Test data for buffer benchmarks
    private byte[] _smallFileData = null!;   // 4 KB
    private byte[] _mediumFileData = null!;  // 64 KB
    private byte[] _largeFileData = null!;   // 1 MB

    // Test data for JSON deserialization benchmarks
    private string _diffsJson = null!;
    private byte[] _diffsJsonBytes = null!;

    [Params(10, 50)]
    public int DiffCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Generate file data of various sizes
        _smallFileData = GenerateFileData(4 * 1024);       // 4 KB
        _mediumFileData = GenerateFileData(64 * 1024);     // 64 KB
        _largeFileData = GenerateFileData(1024 * 1024);    // 1 MB

        // Generate diff JSON for deserialization benchmarks
        _diffsJson = CreateDiffsJson(DiffCount);
        _diffsJsonBytes = Encoding.UTF8.GetBytes(_diffsJson);
    }

    #region ArrayPool<byte> vs new byte[] Benchmarks

    /// <summary>
    /// Baseline: Allocates a new byte array for each operation (traditional approach).
    /// This causes heap allocations that must be garbage collected.
    /// </summary>
    [Benchmark(Baseline = true, Description = "new byte[] - 4KB")]
    [BenchmarkCategory("ArrayPool", "Small")]
    public int NewByteArray_Small()
    {
        byte[] buffer = new byte[_smallFileData.Length];
        _smallFileData.CopyTo(buffer, 0);
        return ProcessBuffer(buffer);
    }

    /// <summary>
    /// Optimized: Uses ArrayPool to rent/return buffers, avoiding heap allocations.
    /// </summary>
    [Benchmark(Description = "ArrayPool - 4KB")]
    [BenchmarkCategory("ArrayPool", "Small")]
    public int ArrayPool_Small()
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(_smallFileData.Length);
        try
        {
            _smallFileData.CopyTo(buffer, 0);
            return ProcessBuffer(buffer.AsSpan(0, _smallFileData.Length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Baseline: Allocates a new byte array (64KB).
    /// </summary>
    [Benchmark(Description = "new byte[] - 64KB")]
    [BenchmarkCategory("ArrayPool", "Medium")]
    public int NewByteArray_Medium()
    {
        byte[] buffer = new byte[_mediumFileData.Length];
        _mediumFileData.CopyTo(buffer, 0);
        return ProcessBuffer(buffer);
    }

    /// <summary>
    /// Optimized: Uses ArrayPool (64KB).
    /// </summary>
    [Benchmark(Description = "ArrayPool - 64KB")]
    [BenchmarkCategory("ArrayPool", "Medium")]
    public int ArrayPool_Medium()
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(_mediumFileData.Length);
        try
        {
            _mediumFileData.CopyTo(buffer, 0);
            return ProcessBuffer(buffer.AsSpan(0, _mediumFileData.Length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Baseline: Allocates a new byte array (1MB).
    /// Large Object Heap allocation - more expensive to collect.
    /// </summary>
    [Benchmark(Description = "new byte[] - 1MB")]
    [BenchmarkCategory("ArrayPool", "Large")]
    public int NewByteArray_Large()
    {
        byte[] buffer = new byte[_largeFileData.Length];
        _largeFileData.CopyTo(buffer, 0);
        return ProcessBuffer(buffer);
    }

    /// <summary>
    /// Optimized: Uses ArrayPool (1MB).
    /// Avoids Large Object Heap allocations entirely.
    /// </summary>
    [Benchmark(Description = "ArrayPool - 1MB")]
    [BenchmarkCategory("ArrayPool", "Large")]
    public int ArrayPool_Large()
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(_largeFileData.Length);
        try
        {
            _largeFileData.CopyTo(buffer, 0);
            return ProcessBuffer(buffer.AsSpan(0, _largeFileData.Length));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Stress test: Multiple small allocations in sequence (traditional).
    /// Demonstrates cumulative GC pressure.
    /// </summary>
    [Benchmark(Description = "new byte[] - 100x 4KB")]
    [BenchmarkCategory("ArrayPool", "Stress")]
    public int NewByteArray_Stress()
    {
        int total = 0;
        for (int i = 0; i < 100; i++)
        {
            byte[] buffer = new byte[_smallFileData.Length];
            _smallFileData.CopyTo(buffer, 0);
            total += ProcessBuffer(buffer);
        }
        return total;
    }

    /// <summary>
    /// Stress test: Multiple pooled allocations in sequence.
    /// Demonstrates ArrayPool reuse benefits.
    /// </summary>
    [Benchmark(Description = "ArrayPool - 100x 4KB")]
    [BenchmarkCategory("ArrayPool", "Stress")]
    public int ArrayPool_Stress()
    {
        int total = 0;
        for (int i = 0; i < 100; i++)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(_smallFileData.Length);
            try
            {
                _smallFileData.CopyTo(buffer, 0);
                total += ProcessBuffer(buffer.AsSpan(0, _smallFileData.Length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        return total;
    }

    #endregion

    #region JsonElement.Deserialize vs GetRawText Benchmarks

    /// <summary>
    /// Legacy approach: GetRawText() creates an intermediate string allocation,
    /// then Deserialize parses that string again.
    /// </summary>
    [Benchmark(Description = "GetRawText + Deserialize")]
    [BenchmarkCategory("JsonElement")]
    public List<BenchmarkDiff> JsonElement_GetRawText()
    {
        var results = new List<BenchmarkDiff>();
        using var doc = JsonDocument.Parse(_diffsJsonBytes);

        if (doc.RootElement.TryGetProperty("diffs", out var diffsArray) &&
            diffsArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var diffElement in diffsArray.EnumerateArray())
            {
                // Legacy: GetRawText() allocates a string, then deserialize parses it again
                var rawText = diffElement.GetRawText();
                var diff = JsonSerializer.Deserialize<BenchmarkDiff>(rawText, s_jsonOptions);
                if (diff is not null)
                {
                    results.Add(diff);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Optimized approach: Deserialize directly from JsonElement.
    /// No intermediate string allocation - zero-copy deserialization.
    /// </summary>
    [Benchmark(Description = "JsonElement.Deserialize (zero-copy)")]
    [BenchmarkCategory("JsonElement")]
    public List<BenchmarkDiff> JsonElement_DirectDeserialize()
    {
        var results = new List<BenchmarkDiff>();
        using var doc = JsonDocument.Parse(_diffsJsonBytes);

        if (doc.RootElement.TryGetProperty("diffs", out var diffsArray) &&
            diffsArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var diffElement in diffsArray.EnumerateArray())
            {
                // Optimized: Deserialize directly from JsonElement - no string allocation
                var diff = diffElement.Deserialize<BenchmarkDiff>(s_jsonOptions);
                if (diff is not null)
                {
                    results.Add(diff);
                }
            }
        }

        return results;
    }

    /// <summary>
    /// Alternative: Deserialize the entire array at once.
    /// Useful when you need all items anyway.
    /// </summary>
    [Benchmark(Description = "Deserialize entire array")]
    [BenchmarkCategory("JsonElement")]
    public List<BenchmarkDiff>? JsonElement_DeserializeArray()
    {
        using var doc = JsonDocument.Parse(_diffsJsonBytes);

        if (doc.RootElement.TryGetProperty("diffs", out var diffsArray))
        {
            return diffsArray.Deserialize<List<BenchmarkDiff>>(s_jsonOptions);
        }

        return null;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Simulates processing a buffer (checksum calculation).
    /// </summary>
    private static int ProcessBuffer(ReadOnlySpan<byte> buffer)
    {
        int checksum = 0;
        foreach (byte b in buffer)
        {
            checksum = unchecked(checksum + b);
        }
        return checksum;
    }

    /// <summary>
    /// Generates random file data of specified size.
    /// </summary>
    private static byte[] GenerateFileData(int size)
    {
        var data = new byte[size];
        var random = new Random(42); // Fixed seed for reproducibility
        random.NextBytes(data);
        return data;
    }

    /// <summary>
    /// Creates a JSON payload containing diff entries for deserialization benchmarks.
    /// Mirrors the structure returned by Bitbucket's diff API.
    /// </summary>
    private static string CreateDiffsJson(int diffCount)
    {
        var diffs = Enumerable.Range(1, diffCount)
            .Select(i => $$"""
            {
                "source": {
                    "components": ["src", "main", "java", "File{{i}}.java"],
                    "parent": "src/main/java",
                    "name": "File{{i}}.java",
                    "extension": "java",
                    "toString": "src/main/java/File{{i}}.java"
                },
                "destination": {
                    "components": ["src", "main", "java", "File{{i}}.java"],
                    "parent": "src/main/java",
                    "name": "File{{i}}.java",
                    "extension": "java",
                    "toString": "src/main/java/File{{i}}.java"
                },
                "hunks": [
                    {
                        "sourceLine": {{i * 10}},
                        "sourceSpan": 5,
                        "destinationLine": {{i * 10}},
                        "destinationSpan": 7,
                        "segments": [],
                        "truncated": false
                    },
                    {
                        "sourceLine": {{i * 20}},
                        "sourceSpan": 3,
                        "destinationLine": {{i * 20 + 2}},
                        "destinationSpan": 5,
                        "segments": [],
                        "truncated": false
                    }
                ],
                "truncated": false
            }
            """);

        return $$"""
        {
            "fromHash": "abc123def456abc123def456abc123def456abc1",
            "toHash": "def456abc123def456abc123def456abc123def4",
            "contextLines": 10,
            "whitespace": "SHOW",
            "diffs": [{{string.Join(",", diffs)}}]
        }
        """;
    }

    #endregion
}

/// <summary>
/// Benchmark-specific model classes that match actual Bitbucket API JSON structure.
/// These are separate from the library models to avoid any serialization quirks.
/// </summary>
public sealed class BenchmarkDiff
{
    public BenchmarkPath? Source { get; set; }
    public BenchmarkPath? Destination { get; set; }
    public List<BenchmarkDiffHunk>? Hunks { get; set; }
    public bool Truncated { get; set; }
}

public sealed class BenchmarkPath
{
    public List<string>? Components { get; set; }
    public string? Parent { get; set; }
    public string? Name { get; set; }
    public string? Extension { get; set; }

    [JsonPropertyName("toString")]
    public string? PathString { get; set; }
}

public sealed class BenchmarkDiffHunk
{
    public int SourceLine { get; set; }
    public int SourceSpan { get; set; }
    public int DestinationLine { get; set; }
    public int DestinationSpan { get; set; }
    public List<object>? Segments { get; set; }
    public bool Truncated { get; set; }
}