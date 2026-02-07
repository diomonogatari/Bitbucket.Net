using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Bitbucket.Net.Benchmarks.Serialization;

/// <summary>
/// Benchmarks measuring cold-start/first-call performance where source generation
/// provides the most significant benefits.
/// </summary>
/// <remarks>
/// <para>
/// Source generation's primary benefit is eliminating reflection-based type metadata
/// generation on first use. This benchmark uses:
/// - ColdStart run strategy (single iteration, no warmup)
/// - Fresh JsonSerializerOptions per iteration to simulate cold-start
/// - ProcessCount to get statistical significance through multiple process launches
/// </para>
/// <para>
/// Expected results: Source-gen should be 2-5x faster on first call because
/// reflection-based serialization must build type metadata at runtime.
/// </para>
/// </remarks>
[SimpleJob(RunStrategy.ColdStart, iterationCount: 10)]
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class ColdStartBenchmarks
{
    private string _pagedProjectsJson = null!;

    [GlobalSetup]
    public void Setup()
    {
        _pagedProjectsJson = CreatePagedProjectsJson(25);
    }

    /// <summary>
    /// Cold-start deserialization using reflection (fresh options each time).
    /// </summary>
    [Benchmark(Baseline = true, Description = "Reflection (Cold)")]
    [BenchmarkCategory("Cold-Start Deserialize")]
    public PagedResults<Project>? ColdStart_Reflection()
    {
        // Create fresh options to simulate cold-start (no cached metadata)
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, options);
    }

    /// <summary>
    /// Cold-start deserialization using source generation (pre-computed metadata).
    /// </summary>
    [Benchmark(Description = "Source-Gen (Cold)")]
    [BenchmarkCategory("Cold-Start Deserialize")]
    public PagedResults<Project>? ColdStart_SourceGen()
    {
        // Source-gen context is pre-computed at compile time
        // Even with fresh options, the type metadata is already available
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = BitbucketJsonContext.Default
        };

        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, options);
    }

    /// <summary>
    /// Cold-start serialization using reflection.
    /// </summary>
    [Benchmark(Baseline = true, Description = "Reflection (Cold)")]
    [BenchmarkCategory("Cold-Start Serialize")]
    public string ColdStart_Serialize_Reflection()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };

        var obj = CreatePagedProjectsObject();
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Cold-start serialization using source generation.
    /// </summary>
    [Benchmark(Description = "Source-Gen (Cold)")]
    [BenchmarkCategory("Cold-Start Serialize")]
    public string ColdStart_Serialize_SourceGen()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = BitbucketJsonContext.Default
        };

        var obj = CreatePagedProjectsObject();
        return JsonSerializer.Serialize(obj, options);
    }

    private static string CreatePagedProjectsJson(int count)
    {
        var projects = Enumerable.Range(1, count)
            .Select(i => $$"""
            {
                "key": "PRJ{{i}}",
                "id": {{i}},
                "name": "Project {{i}}",
                "description": "Description for project {{i}}",
                "public": {{(i % 2 == 0 ? "true" : "false")}},
                "type": "NORMAL",
                "links": {
                    "self": [{ "href": "https://bitbucket.example.com/projects/PRJ{{i}}" }]
                }
            }
            """);

        return $$"""
        {
            "size": {{count}},
            "limit": {{count}},
            "isLastPage": true,
            "values": [{{string.Join(",", projects)}}],
            "start": 0
        }
        """;
    }

    private static PagedResults<Project> CreatePagedProjectsObject()
    {
        var projects = Enumerable.Range(1, 25)
            .Select(i => new Project
            {
                Key = $"PRJ{i}",
                Id = i,
                Name = $"Project {i}",
                Description = $"Description for project {i}",
                Public = i % 2 == 0,
                Type = "NORMAL"
            })
            .ToList();

        return new PagedResults<Project>
        {
            Size = 25,
            Limit = 25,
            IsLastPage = true,
            Values = projects,
            Start = 0
        };
    }
}