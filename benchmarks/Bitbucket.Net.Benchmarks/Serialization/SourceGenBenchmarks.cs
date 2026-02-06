using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using Bitbucket.Net.Common.Converters;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Serialization;

namespace Bitbucket.Net.Benchmarks.Serialization;

/// <summary>
/// Benchmarks comparing source-generated JSON serialization vs reflection-based serialization.
/// Measures the performance benefits of using <see cref="BitbucketJsonContext"/> source generation.
/// </summary>
/// <remarks>
/// <para>
/// Source generation provides:
/// - Faster startup (no runtime reflection for type metadata)
/// - Up to 3x faster serialization throughput (fast-path mode with Utf8JsonWriter)
/// - Reduced memory allocations (no reflection-based metadata caching)
/// - AOT/Trimming compatibility (eliminates reflection requirements)
/// </para>
/// <para>
/// <b>Benchmark Design Notes:</b>
/// Custom converters (like UnixDateTimeOffsetConverter) can block the fast-path optimization.
/// This benchmark suite includes tests both WITH and WITHOUT custom converters to isolate
/// the source-gen performance impact from converter overhead.
/// </para>
/// </remarks>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class SourceGenBenchmarks
{
    // ========================================================================
    // OPTIONS WITH CUSTOM CONVERTERS (Production-like)
    // Custom converters may block fast-path optimization
    // ========================================================================

    /// <summary>
    /// Reflection-based options with custom converters (baseline).
    /// </summary>
    private static readonly JsonSerializerOptions s_reflectionWithConverters = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        Converters =
        {
            new UnixDateTimeOffsetConverter(),
            new NullableUnixDateTimeOffsetConverter()
        }
    };

    /// <summary>
    /// Source-generated options with custom converters.
    /// </summary>
    private static readonly JsonSerializerOptions s_sourceGenWithConverters = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = BitbucketJsonContext.Default,
        Converters =
        {
            new UnixDateTimeOffsetConverter(),
            new NullableUnixDateTimeOffsetConverter()
        }
    };

    // ========================================================================
    // OPTIONS WITHOUT CUSTOM CONVERTERS (Pure Source-Gen Test)
    // These isolate the source-gen performance benefit
    // ========================================================================

    /// <summary>
    /// Reflection-based options WITHOUT custom converters (pure baseline).
    /// </summary>
    private static readonly JsonSerializerOptions s_reflectionPure = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
    };

    /// <summary>
    /// Source-generated options WITHOUT custom converters (pure fast-path).
    /// </summary>
    private static readonly JsonSerializerOptions s_sourceGenPure = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = BitbucketJsonContext.Default
    };

    /// <summary>
    /// Combined options (production) - source-gen with reflection fallback.
    /// </summary>
    private static readonly JsonSerializerOptions s_combinedOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = JsonTypeInfoResolver.Combine(
            BitbucketJsonContext.Default,
            new DefaultJsonTypeInfoResolver()
        ),
        Converters =
        {
            new UnixDateTimeOffsetConverter(),
            new NullableUnixDateTimeOffsetConverter()
        }
    };

    private string _projectJson = null!;
    private string _repositoryJson = null!;
    private string _commitJson = null!;
    private string _pagedProjectsJson = null!;
    private string _pagedRepositoriesJson = null!;
    private string _pagedCommitsJson = null!;

    // JSON without timestamps (for pure source-gen testing)
    private string _projectJsonNoTimestamp = null!;
    private string _pagedProjectsJsonNoTimestamp = null!;

    private Project _projectObject = null!;
    private Repository _repositoryObject = null!;
    private PagedResults<Project> _pagedProjectsObject = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Create test JSON payloads (with timestamps for production-like tests)
        _projectJson = CreateProjectJson();
        _repositoryJson = CreateRepositoryJson();
        _commitJson = CreateCommitJson();
        _pagedProjectsJson = CreatePagedProjectsJson(25);
        _pagedRepositoriesJson = CreatePagedRepositoriesJson(25);
        _pagedCommitsJson = CreatePagedCommitsJson(100);

        // Create JSON without timestamps (for pure source-gen testing)
        _projectJsonNoTimestamp = CreateProjectJsonNoTimestamp();
        _pagedProjectsJsonNoTimestamp = CreatePagedProjectsJsonNoTimestamp(25);

        // Pre-deserialize objects for serialization benchmarks
        _projectObject = JsonSerializer.Deserialize<Project>(_projectJson, s_reflectionWithConverters)!;
        _repositoryObject = JsonSerializer.Deserialize<Repository>(_repositoryJson, s_reflectionWithConverters)!;
        _pagedProjectsObject = JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_reflectionWithConverters)!;
    }

    // ========================================================================
    // PURE SOURCE-GEN TESTS (No Custom Converters)
    // These isolate the true source-gen performance benefit
    // ========================================================================

    #region Pure Source-Gen (No Converters)

    [Benchmark(Baseline = true, Description = "Reflection (Pure)")]
    [BenchmarkCategory("Deserialize Project (No Converters)")]
    public Project? DeserializeProject_Reflection_Pure()
    {
        return JsonSerializer.Deserialize<Project>(_projectJsonNoTimestamp, s_reflectionPure);
    }

    [Benchmark(Description = "Source-Gen (Pure)")]
    [BenchmarkCategory("Deserialize Project (No Converters)")]
    public Project? DeserializeProject_SourceGen_Pure()
    {
        return JsonSerializer.Deserialize<Project>(_projectJsonNoTimestamp, s_sourceGenPure);
    }

    [Benchmark(Baseline = true, Description = "Reflection (Pure)")]
    [BenchmarkCategory("Deserialize PagedResults<Project> (No Converters)")]
    public PagedResults<Project>? DeserializePagedProjects_Reflection_Pure()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJsonNoTimestamp, s_reflectionPure);
    }

    [Benchmark(Description = "Source-Gen (Pure)")]
    [BenchmarkCategory("Deserialize PagedResults<Project> (No Converters)")]
    public PagedResults<Project>? DeserializePagedProjects_SourceGen_Pure()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJsonNoTimestamp, s_sourceGenPure);
    }

    [Benchmark(Baseline = true, Description = "Reflection (Pure)")]
    [BenchmarkCategory("Serialize Project (No Converters)")]
    public string SerializeProject_Reflection_Pure()
    {
        return JsonSerializer.Serialize(_projectObject, s_reflectionPure);
    }

    [Benchmark(Description = "Source-Gen (Pure)")]
    [BenchmarkCategory("Serialize Project (No Converters)")]
    public string SerializeProject_SourceGen_Pure()
    {
        return JsonSerializer.Serialize(_projectObject, s_sourceGenPure);
    }

    [Benchmark(Baseline = true, Description = "Reflection (Pure)")]
    [BenchmarkCategory("Serialize PagedResults<Project> (No Converters)")]
    public string SerializePagedProjects_Reflection_Pure()
    {
        return JsonSerializer.Serialize(_pagedProjectsObject, s_reflectionPure);
    }

    [Benchmark(Description = "Source-Gen (Pure)")]
    [BenchmarkCategory("Serialize PagedResults<Project> (No Converters)")]
    public string SerializePagedProjects_SourceGen_Pure()
    {
        return JsonSerializer.Serialize(_pagedProjectsObject, s_sourceGenPure);
    }

    #endregion

    // ========================================================================
    // PRODUCTION-LIKE TESTS (With Custom Converters)
    // These show real-world performance with custom converters
    // ========================================================================

    #region Single Object Deserialization (With Converters)

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Deserialize Project (With Converters)")]
    public Project? DeserializeProject_Reflection()
    {
        return JsonSerializer.Deserialize<Project>(_projectJson, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Deserialize Project (With Converters)")]
    public Project? DeserializeProject_SourceGen()
    {
        return JsonSerializer.Deserialize<Project>(_projectJson, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Deserialize Project (With Converters)")]
    public Project? DeserializeProject_Combined()
    {
        return JsonSerializer.Deserialize<Project>(_projectJson, s_combinedOptions);
    }

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Deserialize Repository (With Converters)")]
    public Repository? DeserializeRepository_Reflection()
    {
        return JsonSerializer.Deserialize<Repository>(_repositoryJson, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Deserialize Repository (With Converters)")]
    public Repository? DeserializeRepository_SourceGen()
    {
        return JsonSerializer.Deserialize<Repository>(_repositoryJson, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Deserialize Repository (With Converters)")]
    public Repository? DeserializeRepository_Combined()
    {
        return JsonSerializer.Deserialize<Repository>(_repositoryJson, s_combinedOptions);
    }

    #endregion

    #region Paged Results Deserialization (With Converters)

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Deserialize PagedResults<Project> (With Converters)")]
    public PagedResults<Project>? DeserializePagedProjects_Reflection()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Deserialize PagedResults<Project> (With Converters)")]
    public PagedResults<Project>? DeserializePagedProjects_SourceGen()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Deserialize PagedResults<Project> (With Converters)")]
    public PagedResults<Project>? DeserializePagedProjects_Combined()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_combinedOptions);
    }

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Deserialize PagedResults<Repository> (With Converters)")]
    public PagedResults<Repository>? DeserializePagedRepositories_Reflection()
    {
        return JsonSerializer.Deserialize<PagedResults<Repository>>(_pagedRepositoriesJson, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Deserialize PagedResults<Repository> (With Converters)")]
    public PagedResults<Repository>? DeserializePagedRepositories_SourceGen()
    {
        return JsonSerializer.Deserialize<PagedResults<Repository>>(_pagedRepositoriesJson, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Deserialize PagedResults<Repository> (With Converters)")]
    public PagedResults<Repository>? DeserializePagedRepositories_Combined()
    {
        return JsonSerializer.Deserialize<PagedResults<Repository>>(_pagedRepositoriesJson, s_combinedOptions);
    }

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Deserialize PagedResults<Commit> (With Converters)")]
    public PagedResults<Commit>? DeserializeLargePagedCommits_Reflection()
    {
        return JsonSerializer.Deserialize<PagedResults<Commit>>(_pagedCommitsJson, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Deserialize PagedResults<Commit> (With Converters)")]
    public PagedResults<Commit>? DeserializeLargePagedCommits_SourceGen()
    {
        return JsonSerializer.Deserialize<PagedResults<Commit>>(_pagedCommitsJson, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Deserialize PagedResults<Commit> (With Converters)")]
    public PagedResults<Commit>? DeserializeLargePagedCommits_Combined()
    {
        return JsonSerializer.Deserialize<PagedResults<Commit>>(_pagedCommitsJson, s_combinedOptions);
    }

    #endregion

    #region Serialization Benchmarks (With Converters)

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Serialize Project (With Converters)")]
    public string SerializeProject_Reflection()
    {
        return JsonSerializer.Serialize(_projectObject, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Serialize Project (With Converters)")]
    public string SerializeProject_SourceGen()
    {
        return JsonSerializer.Serialize(_projectObject, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Serialize Project (With Converters)")]
    public string SerializeProject_Combined()
    {
        return JsonSerializer.Serialize(_projectObject, s_combinedOptions);
    }

    [Benchmark(Baseline = true, Description = "Reflection")]
    [BenchmarkCategory("Serialize PagedResults<Project> (With Converters)")]
    public string SerializePagedProjects_Reflection()
    {
        return JsonSerializer.Serialize(_pagedProjectsObject, s_reflectionWithConverters);
    }

    [Benchmark(Description = "Source-Gen")]
    [BenchmarkCategory("Serialize PagedResults<Project> (With Converters)")]
    public string SerializePagedProjects_SourceGen()
    {
        return JsonSerializer.Serialize(_pagedProjectsObject, s_sourceGenWithConverters);
    }

    [Benchmark(Description = "Combined")]
    [BenchmarkCategory("Serialize PagedResults<Project> (With Converters)")]
    public string SerializePagedProjects_Combined()
    {
        return JsonSerializer.Serialize(_pagedProjectsObject, s_combinedOptions);
    }

    #endregion

    #region Test Data Generators

    private static string CreateProjectJson()
    {
        return """
        {
            "key": "PRJ",
            "id": 1,
            "name": "My Project",
            "description": "A test project for benchmarking source generation performance",
            "public": true,
            "type": "NORMAL",
            "links": {
                "self": [{ "href": "https://bitbucket.example.com/projects/PRJ" }]
            }
        }
        """;
    }

    private static string CreateProjectJsonNoTimestamp()
    {
        return """
        {
            "key": "PRJ",
            "id": 1,
            "name": "My Project",
            "description": "A test project for benchmarking source generation performance without timestamps",
            "public": true,
            "type": "NORMAL",
            "links": {
                "self": [{ "href": "https://bitbucket.example.com/projects/PRJ" }]
            }
        }
        """;
    }

    private static string CreateRepositoryJson()
    {
        return """
        {
            "slug": "my-repo",
            "id": 1,
            "name": "My Repository",
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
                "self": [{ "href": "https://bitbucket.example.com/projects/PRJ/repos/my-repo/browse" }],
                "clone": [
                    { "href": "https://bitbucket.example.com/scm/prj/my-repo.git", "name": "http" },
                    { "href": "ssh://git@bitbucket.example.com:7999/prj/my-repo.git", "name": "ssh" }
                ]
            }
        }
        """;
    }

    private static string CreateCommitJson()
    {
        return """
        {
            "id": "abc123def456abc123def456abc123def456abc1",
            "displayId": "abc123d",
            "author": {
                "name": "John Doe",
                "emailAddress": "john.doe@example.com"
            },
            "authorTimestamp": 1700000000000,
            "committer": {
                "name": "John Doe",
                "emailAddress": "john.doe@example.com"
            },
            "committerTimestamp": 1700000000000,
            "message": "feat: Add new feature for improved performance",
            "parents": [
                { "id": "def789ghi012def789ghi012def789ghi012def7", "displayId": "def789g" }
            ]
        }
        """;
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

    private static string CreatePagedProjectsJsonNoTimestamp(int count)
    {
        var projects = Enumerable.Range(1, count)
            .Select(i => $$"""
            {
                "key": "PRJ{{i}}",
                "id": {{i}},
                "name": "Project {{i}}",
                "description": "Description for project {{i}} - no timestamp version",
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

    private static string CreatePagedRepositoriesJson(int count)
    {
        var repos = Enumerable.Range(1, count)
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

        return $$"""
        {
            "size": {{count}},
            "limit": {{count}},
            "isLastPage": true,
            "values": [{{string.Join(",", repos)}}],
            "start": 0
        }
        """;
    }

    private static string CreatePagedCommitsJson(int count)
    {
        var commits = Enumerable.Range(1, count)
            .Select(i =>
            {
                var commitId = $"{Guid.NewGuid():N}{Guid.NewGuid():N}"[..40];
                var commitType = i % 3 == 0 ? "feat" : i % 3 == 1 ? "fix" : "chore";

                return $$"""
                {
                    "id": "{{commitId}}",
                    "displayId": "{{commitId[..7]}}",
                    "author": {
                        "name": "Developer {{i % 5}}",
                        "emailAddress": "dev{{i % 5}}@example.com"
                    },
                    "authorTimestamp": {{1700000000000 + i * 3600000}},
                    "committer": {
                        "name": "Developer {{i % 5}}",
                        "emailAddress": "dev{{i % 5}}@example.com"
                    },
                    "committerTimestamp": {{1700000000000 + i * 3600000}},
                    "message": "Commit message {{i}}: {{commitType}}: Update component {{i}}",
                    "parents": []
                }
                """;
            });

        return $$"""
        {
            "size": {{count}},
            "limit": {{count}},
            "isLastPage": false,
            "values": [{{string.Join(",", commits)}}],
            "start": 0,
            "nextPageStart": {{count}}
        }
        """;
    }

    #endregion
}
