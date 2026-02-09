using System.Net;
using Bitbucket.Net.Common.Exceptions;
using Bitbucket.Net.Tests.Infrastructure;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// WireMock-based integration tests for the Bitbucket Server Code Search API.
/// Verifies the full HTTP round-trip: request serialization, POST to /rest/search/latest/search,
/// and response deserialization, using fixture data modeled after the real (undocumented) API shape.
/// </summary>
public class SearchMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task SearchCodeAsync_ReturnsResults_FromMultipleFiles()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-results.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("HttpClient");

        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.Equal(61, result.Code.Count);
        Assert.False(result.Code.IsLastPage);
        Assert.Equal(25, result.Code.NextStart);
        Assert.Equal(2, result.Code.Values.Count);

        var hit1 = result.Code.Values[0];
        Assert.Equal("src/Middleware/RequestHandler.cs", hit1.File);
        Assert.Equal(2, hit1.HitCount);
        Assert.NotNull(hit1.HitContexts);
        Assert.Equal(2, hit1.HitContexts.Count);
        Assert.Equal(3, hit1.HitContexts[0].Count);
        Assert.Equal(14, hit1.HitContexts[0][0].Line);
        Assert.Equal("using System.Net.Http;", hit1.HitContexts[0][0].Text);
        Assert.Contains("<em>HttpClient</em>", hit1.HitContexts[1][0].Text);

        var hit2 = result.Code.Values[1];
        Assert.Equal("tests/HandlerTests.cs", hit2.File);
        Assert.Equal(1, hit2.HitCount);
        Assert.NotNull(hit2.PathMatches);
        Assert.Single(hit2.PathMatches);
        Assert.Equal(6, hit2.PathMatches[0].Start);
        Assert.Equal(7, hit2.PathMatches[0].Length);
    }

    [Fact]
    public async Task SearchCodeAsync_EmptyResults_ReturnsEmptyValues()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-empty.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("nonexistent-query-xyz");

        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.Equal(0, result.Code.Count);
        Assert.True(result.Code.IsLastPage);
        Assert.Empty(result.Code.Values);
    }

    [Fact]
    public async Task SearchCodeAsync_SingleHit_DeserializesCorrectly()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-single-hit.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("ConnectionString");

        Assert.NotNull(result);
        Assert.NotNull(result.Code);
        Assert.Equal(1, result.Code.Count);
        Assert.True(result.Code.IsLastPage);
        Assert.Single(result.Code.Values);

        var hit = result.Code.Values[0];
        Assert.Equal("src/Config.cs", hit.File);
        Assert.Equal(1, hit.HitCount);
        Assert.NotNull(hit.Repository);
        Assert.Equal(TestConstants.TestRepositorySlug, hit.Repository.Slug);
        Assert.NotNull(hit.Repository.Project);
        Assert.Equal(TestConstants.TestProjectKey, hit.Repository.Project.Key);
    }

    [Fact]
    public async Task SearchCodeAsync_SubstitutedQuery_ReflectsInResponse()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-substituted.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("Tset");

        Assert.NotNull(result);
        Assert.NotNull(result.Query);
        Assert.True(result.Query.Substituted);
    }

    [Fact]
    public async Task SearchCodeAsync_DefaultScope_IsGlobal()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-results.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("HttpClient");

        Assert.NotNull(result.Scope);
        Assert.Equal("GLOBAL", result.Scope.Type);
    }

    [Fact]
    public async Task SearchCodeAsync_CustomLimits_AreSerializedInRequest()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-single-hit.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("test", primaryLimit: 5, secondaryLimit: 3);

        Assert.NotNull(result);
        Assert.NotNull(result.Code);

        var logs = _fixture.Server.LogEntries;
        var searchLog = Assert.Single(logs, l => l.RequestMessage.Path == "/rest/search/latest/search");
        Assert.Equal("POST", searchLog.RequestMessage.Method, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("\"primary\":5", searchLog.RequestMessage.Body);
        Assert.Contains("\"secondary\":3", searchLog.RequestMessage.Body);
    }

    [Fact]
    public async Task SearchCodeAsync_RequestBody_ContainsCodeEntitiesAndQuery()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-empty.json");
        var client = _fixture.CreateClient();

        await client.SearchCodeAsync("project:TEST repo:test-repo HttpClient");

        var logs = _fixture.Server.LogEntries;
        var searchLog = Assert.Single(logs, l => l.RequestMessage.Path == "/rest/search/latest/search");
        var body = searchLog.RequestMessage.Body;

        Assert.Contains("\"query\":\"project:TEST repo:test-repo HttpClient\"", body);
        Assert.Contains("\"code\":{}", body);
        Assert.Contains("\"primary\":25", body);
        Assert.Contains("\"secondary\":10", body);
    }

    [Fact]
    public async Task SearchCodeAsync_HitContextLines_PreserveHighlightTags()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-results.json");
        var client = _fixture.CreateClient();

        var result = await client.SearchCodeAsync("HttpClient");

        var firstFile = result.Code!.Values[0];
        var highlightedLine = firstFile.HitContexts![1][0];
        Assert.Equal(42, highlightedLine.Line);
        Assert.Equal("    var client = new <em>HttpClient</em>();", highlightedLine.Text);
    }

    [Fact]
    public async Task SearchCodeAsync_ServerReturns404_ThrowsBitbucketNotFoundException()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCodeError(HttpStatusCode.NotFound);
        var client = _fixture.CreateClient();

        var ex = await Assert.ThrowsAsync<BitbucketNotFoundException>(
            () => client.SearchCodeAsync("anything"));
        Assert.Contains("Search is not available", ex.Message);
    }

    [Fact]
    public async Task SearchCodeAsync_ServerReturns500_ThrowsBitbucketServerException()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCodeError(HttpStatusCode.InternalServerError);
        var client = _fixture.CreateClient();

        var ex = await Assert.ThrowsAsync<BitbucketServerException>(
            () => client.SearchCodeAsync("anything"));
        Assert.Contains("Search is not available", ex.Message);
    }

    [Fact]
    public async Task IsSearchAvailableAsync_ReturnsTrue_WhenEndpointResponds()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-empty.json");
        var client = _fixture.CreateClient();

        var available = await client.IsSearchAvailableAsync();

        Assert.True(available);
    }

    [Fact]
    public async Task IsSearchAvailableAsync_ReturnsFalse_WhenEndpointReturns404()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCodeError(HttpStatusCode.NotFound);
        var client = _fixture.CreateClient();

        var available = await client.IsSearchAvailableAsync();

        Assert.False(available);
    }

    [Fact]
    public async Task IsSearchAvailableAsync_ReturnsFalse_WhenEndpointReturns503()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCodeError(HttpStatusCode.ServiceUnavailable);
        var client = _fixture.CreateClient();

        var available = await client.IsSearchAvailableAsync();

        Assert.False(available);
    }

    [Fact]
    public async Task SearchCodeAsync_CancellationToken_IsPropagated()
    {
        _fixture.Reset();
        _fixture.Server.SetupSearchCode("code-search-results.json");
        var client = _fixture.CreateClient();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Flurl wraps TaskCanceledException in FlurlHttpException
        var ex = await Assert.ThrowsAsync<Flurl.Http.FlurlHttpException>(
            () => client.SearchCodeAsync("test", cancellationToken: cts.Token));
        Assert.IsAssignableFrom<OperationCanceledException>(ex.InnerException);
    }
}
