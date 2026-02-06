using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using Bitbucket.Net.Benchmarks.Config;
using Bitbucket.Net.Common.Models;
using Bitbucket.Net.Models.Core.Projects;

namespace Bitbucket.Net.Benchmarks.Serialization;

/// <summary>
/// Benchmarks comparing JSON serialization approaches.
/// Measures the performance of System.Text.Json for deserializing Bitbucket API responses.
/// </summary>
[Config(typeof(DefaultBenchmarkConfig))]
[MemoryDiagnoser]
public class JsonSerializationBenchmarks
{
    private static readonly JsonSerializerOptions s_defaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly JsonSerializerOptions s_webOptions = new(JsonSerializerDefaults.Web);

    private string _singleProjectJson = null!;
    private string _singleRepositoryJson = null!;
    private string _singleCommitJson = null!;
    private string _pagedProjectsJson = null!;
    private string _pagedRepositoriesJson = null!;
    private string _largePagedCommitsJson = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Single object JSON payloads
        _singleProjectJson = CreateSingleProjectJson();
        _singleRepositoryJson = CreateSingleRepositoryJson();
        _singleCommitJson = CreateSingleCommitJson();

        // Paged results JSON payloads
        _pagedProjectsJson = CreatePagedProjectsJson(25);
        _pagedRepositoriesJson = CreatePagedRepositoriesJson(25);
        _largePagedCommitsJson = CreatePagedCommitsJson(100);
    }

    #region Single Object Deserialization

    [Benchmark(Description = "Deserialize single Project")]
    public Project? DeserializeSingleProject()
    {
        return JsonSerializer.Deserialize<Project>(_singleProjectJson, s_defaultOptions);
    }

    [Benchmark(Description = "Deserialize single Repository")]
    public Repository? DeserializeSingleRepository()
    {
        return JsonSerializer.Deserialize<Repository>(_singleRepositoryJson, s_defaultOptions);
    }

    [Benchmark(Description = "Deserialize single Commit")]
    public Commit? DeserializeSingleCommit()
    {
        return JsonSerializer.Deserialize<Commit>(_singleCommitJson, s_defaultOptions);
    }

    #endregion

    #region Paged Results Deserialization

    [Benchmark(Description = "Deserialize PagedResults<Project> (25 items)")]
    public PagedResults<Project>? DeserializePagedProjects()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_defaultOptions);
    }

    [Benchmark(Description = "Deserialize PagedResults<Repository> (25 items)")]
    public PagedResults<Repository>? DeserializePagedRepositories()
    {
        return JsonSerializer.Deserialize<PagedResults<Repository>>(_pagedRepositoriesJson, s_defaultOptions);
    }

    [Benchmark(Description = "Deserialize PagedResults<Commit> (100 items)")]
    public PagedResults<Commit>? DeserializeLargePagedCommits()
    {
        return JsonSerializer.Deserialize<PagedResults<Commit>>(_largePagedCommitsJson, s_defaultOptions);
    }

    #endregion

    #region Options Comparison

    [Benchmark(Description = "Default options - PagedResults<Project>")]
    [BenchmarkCategory("Options")]
    public PagedResults<Project>? DeserializeWithDefaultOptions()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_defaultOptions);
    }

    [Benchmark(Description = "Web defaults - PagedResults<Project>")]
    [BenchmarkCategory("Options")]
    public PagedResults<Project>? DeserializeWithWebDefaults()
    {
        return JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_webOptions);
    }

    #endregion

    #region Serialization Benchmarks

    [Benchmark(Description = "Serialize PagedResults<Project> (25 items)")]
    public string SerializePagedProjects()
    {
        var data = JsonSerializer.Deserialize<PagedResults<Project>>(_pagedProjectsJson, s_defaultOptions);
        return JsonSerializer.Serialize(data, s_defaultOptions);
    }

    [Benchmark(Description = "Serialize PagedResults<Repository> (25 items)")]
    public string SerializePagedRepositories()
    {
        var data = JsonSerializer.Deserialize<PagedResults<Repository>>(_pagedRepositoriesJson, s_defaultOptions);
        return JsonSerializer.Serialize(data, s_defaultOptions);
    }

    #endregion

    #region Test Data Generators

    private static string CreateSingleProjectJson()
    {
        return """
        {
            "key": "PRJ",
            "id": 1,
            "name": "My Project",
            "description": "A test project for benchmarking",
            "public": true,
            "type": "NORMAL",
            "links": {
                "self": [{ "href": "https://bitbucket.example.com/projects/PRJ" }]
            }
        }
        """;
    }

    private static string CreateSingleRepositoryJson()
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

    private static string CreateSingleCommitJson()
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
                // Generate a 40-character commit SHA (pre-compute outside JSON template)
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
