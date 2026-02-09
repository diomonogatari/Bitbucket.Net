using Bitbucket.Net.Common.Models.Search;
using Bitbucket.Net.Serialization;
using System.Text.Json;
using Xunit;

namespace Bitbucket.Net.Tests.UnitTests;

/// <summary>
/// Unit tests for the code search model serialization and deserialization.
/// These verify that the source-generated JSON context handles the undocumented
/// Bitbucket Server Code Search API contract correctly.
/// </summary>
public class CodeSearchSerializationTests
{
    private readonly JsonSerializerOptions _options = BitbucketJsonContext.Default.Options;

    #region CodeSearchRequest Serialization

    [Fact]
    public void CodeSearchRequest_RoundTrips()
    {
        var request = new CodeSearchRequest
        {
            Query = "project:PROJ repo:my-repo HttpClient",
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits { Primary = 10, Secondary = 5 }
        };

        var json = JsonSerializer.Serialize(request, _options);
        var deserialized = JsonSerializer.Deserialize<CodeSearchRequest>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(request.Query, deserialized.Query);
        Assert.NotNull(deserialized.Entities);
        Assert.NotNull(deserialized.Entities.Code);
        Assert.Equal(10, deserialized.Limits.Primary);
        Assert.Equal(5, deserialized.Limits.Secondary);
    }

    [Fact]
    public void CodeSearchRequest_CodeEntities_SerializesAsEmptyObject()
    {
        var request = new CodeSearchRequest
        {
            Query = "test",
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits()
        };

        var json = JsonSerializer.Serialize(request, _options);

        Assert.Contains("\"code\":{}", json);
    }

    [Fact]
    public void CodeSearchRequest_DefaultLimits_AreCorrect()
    {
        var request = new CodeSearchRequest
        {
            Query = "test",
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits()
        };

        var json = JsonSerializer.Serialize(request, _options);

        Assert.Contains("\"primary\":25", json);
        Assert.Contains("\"secondary\":10", json);
    }

    [Fact]
    public void CodeSearchRequest_CustomLimits_Serialize()
    {
        var request = new CodeSearchRequest
        {
            Query = "test",
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits { Primary = 50, Secondary = 20 }
        };

        var json = JsonSerializer.Serialize(request, _options);

        Assert.Contains("\"primary\":50", json);
        Assert.Contains("\"secondary\":20", json);
    }

    [Fact]
    public void CodeSearchRequest_Query_PreservesSearchSyntax()
    {
        var query = "project:PROJ repo:my-repo ext:cs path:src/ async await";
        var request = new CodeSearchRequest
        {
            Query = query,
            Entities = SearchEntities.CodeOnly,
            Limits = new SearchLimits()
        };

        var json = JsonSerializer.Serialize(request, _options);

        Assert.Contains(query, json);
    }

    #endregion

    #region SearchEntities

    [Fact]
    public void SearchEntities_CodeOnly_HasNonNullCode()
    {
        var entities = SearchEntities.CodeOnly;

        Assert.NotNull(entities.Code);
    }

    [Fact]
    public void SearchEntities_Default_HasNullCode()
    {
        var entities = new SearchEntities();

        Assert.Null(entities.Code);
    }

    [Fact]
    public void SearchEntities_NullCode_OmittedFromJson()
    {
        var entities = new SearchEntities();
        var json = JsonSerializer.Serialize(entities, _options);

        Assert.DoesNotContain("\"code\"", json);
    }

    #endregion

    #region CodeSearchResponse Deserialization

    [Fact]
    public void CodeSearchResponse_FullResponse_Deserializes()
    {
        var json = """
        {
            "scope": { "type": "GLOBAL" },
            "code": {
                "category": "primary",
                "isLastPage": false,
                "count": 61,
                "start": 0,
                "nextStart": 25,
                "values": [
                    {
                        "repository": {
                            "slug": "my-repo",
                            "id": 123,
                            "name": "My Repo",
                            "project": { "key": "PROJ" }
                        },
                        "file": "src/Handler.cs",
                        "hitContexts": [
                            [
                                { "line": 10, "text": "    var x = <em>await</em> DoWork();" },
                                { "line": 11, "text": "    return x;" }
                            ]
                        ],
                        "pathMatches": [],
                        "hitCount": 3
                    }
                ]
            },
            "query": { "substituted": false }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);

        Assert.NotNull(result);
        Assert.NotNull(result.Scope);
        Assert.Equal("GLOBAL", result.Scope.Type);
        Assert.NotNull(result.Code);
        Assert.Equal("primary", result.Code.Category);
        Assert.False(result.Code.IsLastPage);
        Assert.Equal(61, result.Code.Count);
        Assert.Equal(0, result.Code.Start);
        Assert.Equal(25, result.Code.NextStart);
        Assert.Single(result.Code.Values);
        Assert.NotNull(result.Query);
        Assert.False(result.Query.Substituted);
    }

    [Fact]
    public void CodeSearchResult_Repository_DeserializesNestedProjectRef()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 1,
                "start": 0,
                "values": [
                    {
                        "repository": {
                            "slug": "my-repo",
                            "id": 42,
                            "name": "My Repo",
                            "project": {
                                "key": "PROJ"
                            }
                        },
                        "file": "test.cs",
                        "hitContexts": [],
                        "pathMatches": [],
                        "hitCount": 0
                    }
                ]
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);
        var repo = result!.Code!.Values[0].Repository;

        Assert.NotNull(repo);
        Assert.Equal("my-repo", repo.Slug);
        Assert.Equal(42, repo.Id);
        Assert.Equal("My Repo", repo.Name);
        Assert.NotNull(repo.Project);
        Assert.Equal("PROJ", repo.Project.Key);
    }

    [Fact]
    public void CodeSearchResult_MultipleHitContextBlocks_Deserialize()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 1,
                "start": 0,
                "values": [
                    {
                        "file": "app.cs",
                        "hitContexts": [
                            [
                                { "line": 5, "text": "before" },
                                { "line": 6, "text": "<em>match1</em>" },
                                { "line": 7, "text": "after" }
                            ],
                            [
                                { "line": 20, "text": "<em>match2</em>" }
                            ]
                        ],
                        "pathMatches": [],
                        "hitCount": 2
                    }
                ]
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);
        var hit = result!.Code!.Values[0];

        Assert.NotNull(hit.HitContexts);
        Assert.Equal(2, hit.HitContexts.Count);
        Assert.Equal(3, hit.HitContexts[0].Count);
        Assert.Single(hit.HitContexts[1]);
        Assert.Equal(6, hit.HitContexts[0][1].Line);
        Assert.Equal("<em>match1</em>", hit.HitContexts[0][1].Text);
        Assert.Equal(20, hit.HitContexts[1][0].Line);
    }

    [Fact]
    public void CodeSearchResult_PathMatches_Deserialize()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 1,
                "start": 0,
                "values": [
                    {
                        "file": "src/SearchService.cs",
                        "hitContexts": [],
                        "pathMatches": [
                            { "start": 4, "length": 6 },
                            { "start": 18, "length": 6 }
                        ],
                        "hitCount": 0
                    }
                ]
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);
        var pathMatches = result!.Code!.Values[0].PathMatches;

        Assert.NotNull(pathMatches);
        Assert.Equal(2, pathMatches.Count);
        Assert.Equal(4, pathMatches[0].Start);
        Assert.Equal(6, pathMatches[0].Length);
        Assert.Equal(18, pathMatches[1].Start);
    }

    [Fact]
    public void CodeSearchResponse_EmptyResults_Deserializes()
    {
        var json = """
        {
            "scope": { "type": "GLOBAL" },
            "code": {
                "category": "primary",
                "isLastPage": true,
                "count": 0,
                "start": 0,
                "values": []
            },
            "query": { "substituted": false }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);

        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.Equal(0, result.Code.Count);
        Assert.True(result.Code.IsLastPage);
        Assert.Empty(result.Code.Values);
    }

    [Fact]
    public void CodeSearchResponse_NullNextStart_WhenLastPage()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 5,
                "start": 0,
                "values": []
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);

        Assert.Null(result!.Code!.NextStart);
        Assert.True(result.Code.IsLastPage);
    }

    [Fact]
    public void CodeSearchResponse_QuerySubstitution_Detected()
    {
        var json = """
        {
            "code": { "isLastPage": true, "count": 0, "start": 0, "values": [] },
            "query": { "substituted": true }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);

        Assert.NotNull(result!.Query);
        Assert.True(result.Query.Substituted);
    }

    [Fact]
    public void CodeSearchResponse_MissingOptionalFields_DefaultsGracefully()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 1,
                "start": 0,
                "values": [
                    {
                        "file": "readme.md",
                        "hitCount": 1
                    }
                ]
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);

        Assert.Null(result!.Scope);
        Assert.Null(result.Query);
        var hit = result.Code!.Values[0];
        Assert.Null(hit.Repository);
        Assert.Null(hit.HitContexts);
        Assert.Null(hit.PathMatches);
        Assert.Equal("readme.md", hit.File);
        Assert.Equal(1, hit.HitCount);
    }

    [Fact]
    public void CodeSearchResponse_HighlightTags_PreservedInDeserialization()
    {
        var json = """
        {
            "code": {
                "isLastPage": true,
                "count": 1,
                "start": 0,
                "values": [
                    {
                        "file": "test.cs",
                        "hitContexts": [
                            [
                                { "line": 1, "text": "var x = <em>await</em> <em>Task</em>.Run();" }
                            ]
                        ],
                        "hitCount": 1
                    }
                ]
            }
        }
        """;

        var result = JsonSerializer.Deserialize<CodeSearchResponse>(json, _options);
        var text = result!.Code!.Values[0].HitContexts![0][0].Text;

        Assert.Equal("var x = <em>await</em> <em>Task</em>.Run();", text);
    }

    #endregion

    #region SearchLimits Defaults

    [Fact]
    public void SearchLimits_DefaultValues_Are25And10()
    {
        var limits = new SearchLimits();

        Assert.Equal(25, limits.Primary);
        Assert.Equal(10, limits.Secondary);
    }

    [Fact]
    public void SearchLimits_RoundTrips()
    {
        var limits = new SearchLimits { Primary = 100, Secondary = 50 };

        var json = JsonSerializer.Serialize(limits, _options);
        var deserialized = JsonSerializer.Deserialize<SearchLimits>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(100, deserialized.Primary);
        Assert.Equal(50, deserialized.Secondary);
    }

    #endregion

    #region SearchScope

    [Fact]
    public void SearchScope_RoundTrips()
    {
        var scope = new SearchScope { Type = "REPOSITORY" };

        var json = JsonSerializer.Serialize(scope, _options);
        var deserialized = JsonSerializer.Deserialize<SearchScope>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal("REPOSITORY", deserialized.Type);
    }

    #endregion

    #region SearchPathMatch

    [Fact]
    public void SearchPathMatch_RoundTrips()
    {
        var match = new SearchPathMatch { Start = 10, Length = 5 };

        var json = JsonSerializer.Serialize(match, _options);
        var deserialized = JsonSerializer.Deserialize<SearchPathMatch>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(10, deserialized.Start);
        Assert.Equal(5, deserialized.Length);
    }

    #endregion

    #region CodeSearchHitLine

    [Fact]
    public void CodeSearchHitLine_RoundTrips()
    {
        var hitLine = new CodeSearchHitLine { Line = 42, Text = "the answer" };

        var json = JsonSerializer.Serialize(hitLine, _options);
        var deserialized = JsonSerializer.Deserialize<CodeSearchHitLine>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Equal(42, deserialized.Line);
        Assert.Equal("the answer", deserialized.Text);
    }

    [Fact]
    public void CodeSearchHitLine_NullText_Allowed()
    {
        var hitLine = new CodeSearchHitLine { Line = 1 };

        var json = JsonSerializer.Serialize(hitLine, _options);
        var deserialized = JsonSerializer.Deserialize<CodeSearchHitLine>(json, _options);

        Assert.NotNull(deserialized);
        Assert.Null(deserialized.Text);
    }

    #endregion

    #region SearchQuery

    [Fact]
    public void SearchQuery_RoundTrips()
    {
        var query = new SearchQuery { Substituted = true };

        var json = JsonSerializer.Serialize(query, _options);
        var deserialized = JsonSerializer.Deserialize<SearchQuery>(json, _options);

        Assert.NotNull(deserialized);
        Assert.True(deserialized.Substituted);
    }

    #endregion

    #region SearchEntityFilter

    [Fact]
    public void SearchEntityFilter_SerializesToEmptyObject()
    {
        var filter = new SearchEntityFilter();
        var json = JsonSerializer.Serialize(filter, _options);

        Assert.Equal("{}", json);
    }

    [Fact]
    public void SearchEntityFilter_RoundTrips()
    {
        var filter = new SearchEntityFilter();

        var json = JsonSerializer.Serialize(filter, _options);
        var deserialized = JsonSerializer.Deserialize<SearchEntityFilter>(json, _options);

        Assert.NotNull(deserialized);
    }

    #endregion
}