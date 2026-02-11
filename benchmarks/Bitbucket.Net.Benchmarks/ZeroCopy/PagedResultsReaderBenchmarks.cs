using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using Bitbucket.Net.Common;
using Bitbucket.Net.Common.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Benchmarks.ZeroCopy;

/// <summary>
/// Benchmarks comparing Utf8JsonReader-based metadata extraction
/// vs full JsonSerializer deserialization for paged API responses.
/// </summary>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
public class PagedResultsReaderBenchmarks
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private byte[] _emptyPayload = null!;
    private byte[] _smallPayload = null!;
    private byte[] _largePayload = null!;

    [GlobalSetup]
    public void Setup()
    {
        _emptyPayload = Encoding.UTF8.GetBytes(CreatePagedJson(0));
        _smallPayload = Encoding.UTF8.GetBytes(CreatePagedJson(25));
        _largePayload = Encoding.UTF8.GetBytes(CreatePagedJson(100));
    }

    #region Empty payload (0 items)

    [Benchmark(Baseline = true, Description = "JsonSerializer - Empty")]
    [BenchmarkCategory("Empty")]
    public PagedResultsBase? JsonSerializer_Empty()
    {
        return JsonSerializer.Deserialize<PagedResults<BenchmarkItem>>(_emptyPayload, s_jsonOptions);
    }

    [Benchmark(Description = "Utf8JsonReader - Empty")]
    [BenchmarkCategory("Empty")]
    public int Utf8JsonReader_Empty()
    {
        var m = PagedResultsReader.ReadMetadata(_emptyPayload);
        return m.Size;
    }

    #endregion

    #region Small payload (25 items)

    [Benchmark(Description = "JsonSerializer - 25 items")]
    [BenchmarkCategory("Small")]
    public PagedResultsBase? JsonSerializer_Small()
    {
        return JsonSerializer.Deserialize<PagedResults<BenchmarkItem>>(_smallPayload, s_jsonOptions);
    }

    [Benchmark(Description = "Utf8JsonReader - 25 items")]
    [BenchmarkCategory("Small")]
    public int Utf8JsonReader_Small()
    {
        var m = PagedResultsReader.ReadMetadata(_smallPayload);
        return m.Size;
    }

    #endregion

    #region Large payload (100 items)

    [Benchmark(Description = "JsonSerializer - 100 items")]
    [BenchmarkCategory("Large")]
    public PagedResultsBase? JsonSerializer_Large()
    {
        return JsonSerializer.Deserialize<PagedResults<BenchmarkItem>>(_largePayload, s_jsonOptions);
    }

    [Benchmark(Description = "Utf8JsonReader - 100 items")]
    [BenchmarkCategory("Large")]
    public int Utf8JsonReader_Large()
    {
        var m = PagedResultsReader.ReadMetadata(_largePayload);
        return m.Size;
    }

    #endregion

    #region Helper Methods

    private static string CreatePagedJson(int itemCount)
    {
        var items = Enumerable.Range(1, itemCount)
            .Select(i => $$"""{"id":{{i}},"name":"item-{{i}}","description":"Description for item {{i}}","active":true,"tags":["tag1","tag2"]}""");

        bool isLastPage = itemCount == 0;
        int? nextPageStart = isLastPage ? null : 25;

        return $$"""{"size":{{itemCount}},"limit":25,"isLastPage":{{(isLastPage ? "true" : "false")}},"start":0{{(nextPageStart.HasValue ? $",\"nextPageStart\":{nextPageStart}" : "")}},"values":[{{string.Join(",", items)}}]}""";
    }

    #endregion
}

public sealed class BenchmarkItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; }
    public List<string>? Tags { get; set; }
}