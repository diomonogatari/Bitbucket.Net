using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bitbucket.Net.Benchmarks.Streaming;

/// <summary>
/// Benchmarks comparing streaming (IAsyncEnumerable) vs buffered (List) pagination approaches.
/// Demonstrates memory efficiency and time-to-first-result improvements.
/// </summary>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
public class StreamingBenchmarks
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private List<string> _pagedResponses = null!;

    [Params(5, 10, 25)]
    public int PageCount { get; set; }

    [Params(25, 100)]
    public int ItemsPerPage { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _pagedResponses = [.. Enumerable.Range(0, PageCount).Select(pageIndex => CreatePagedRepositoriesJson(ItemsPerPage, pageIndex, pageIndex < PageCount - 1))];
    }

    /// <summary>
    /// Simulates the buffered approach - collecting all items into a List before returning.
    /// </summary>
    [Benchmark(Baseline = true, Description = "Buffered (List<T>)")]
    public async Task<List<Repository>> BufferedApproach()
    {
        var results = new List<Repository>();

        foreach (var pageJson in _pagedResponses)
        {
            // Simulate async API call delay
            await Task.Yield();

            var page = JsonSerializer.Deserialize<PagedResults<Repository>>(pageJson, s_jsonOptions);
            if (page?.Values != null)
            {
                results.AddRange(page.Values);
            }
        }

        return results;
    }

    /// <summary>
    /// Simulates the streaming approach - yielding items as they arrive.
    /// </summary>
    [Benchmark(Description = "Streaming (IAsyncEnumerable)")]
    public async Task<int> StreamingApproach()
    {
        int count = 0;

        await foreach (var item in StreamItemsAsync())
        {
            count++;
            // Simulate processing each item
        }

        return count;
    }

    /// <summary>
    /// Measures time to first item - streaming should win significantly here.
    /// </summary>
    [Benchmark(Description = "Time-to-first-item (Streaming)")]
    public async Task<Repository?> StreamingFirstItem()
    {
        await foreach (var item in StreamItemsAsync())
        {
            return item; // Return immediately after first item
        }

        return null;
    }

    /// <summary>
    /// Measures time to first item with buffered approach - must wait for full first page.
    /// </summary>
    [Benchmark(Description = "Time-to-first-item (Buffered)")]
    public async Task<Repository?> BufferedFirstItem()
    {
        var results = await BufferedApproach().ConfigureAwait(false);
        return results.FirstOrDefault();
    }

    /// <summary>
    /// Simulates early termination with streaming - stops after N items.
    /// </summary>
    [Benchmark(Description = "Early termination (Streaming) - 10 items")]
    public async Task<List<Repository>> StreamingEarlyTermination()
    {
        var results = new List<Repository>();

        await foreach (var item in StreamItemsAsync())
        {
            results.Add(item);
            if (results.Count >= 10)
            {
                break;
            }
        }

        return results;
    }

    /// <summary>
    /// Simulates early termination with buffered approach - still loads all data first.
    /// </summary>
    [Benchmark(Description = "Early termination (Buffered) - 10 items")]
    public async Task<List<Repository>> BufferedEarlyTermination()
    {
        var allResults = await BufferedApproach().ConfigureAwait(false);
        return [.. allResults.Take(10)];
    }

    private async IAsyncEnumerable<Repository> StreamItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var pageJson in _pagedResponses)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Simulate async API call delay
            await Task.Yield();

            var page = JsonSerializer.Deserialize<PagedResults<Repository>>(pageJson, s_jsonOptions);
            if (page?.Values != null)
            {
                foreach (var item in page.Values)
                {
                    yield return item;
                }
            }
        }
    }

    private static string CreatePagedRepositoriesJson(int count, int pageIndex, bool hasMore)
    {
        var startIndex = pageIndex * count;
        var repos = Enumerable.Range(startIndex, count)
            .Select(i => $$"""
            {
                "slug": "repo-{{i}}",
                "id": {{i}},
                "name": "Repository {{i}}",
                "scmId": "git",
                "state": "AVAILABLE",
                "statusMessage": "Available",
                "forkable": true,
                "public": false,
                "project": {
                    "key": "PRJ",
                    "id": 1,
                    "name": "My Project",
                    "public": true,
                    "type": "NORMAL"
                },
                "links": {
                    "self": [{ "href": "https://bitbucket.example.com/projects/PRJ/repos/repo-{{i}}/browse" }],
                    "clone": [
                        { "href": "https://bitbucket.example.com/scm/prj/repo-{{i}}.git", "name": "http" }
                    ]
                }
            }
            """);

        var nextPageStart = hasMore ? $", \"nextPageStart\": {startIndex + count}" : "";

        return $$"""
        {
            "size": {{count}},
            "limit": {{count}},
            "isLastPage": {{(!hasMore).ToString().ToLowerInvariant()}},
            "values": [{{string.Join(",", repos)}}],
            "start": {{startIndex}}{{nextPageStart}}
        }
        """;
    }
}