using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using Flurl.Http;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Proves the scenario from issue #4: a consumer can subclass <see cref="BitbucketClient"/>
/// and implement a custom paged endpoint by reusing the now-<c>protected</c> primitives
/// (<c>GetBaseUrl</c> + <c>GetPagedAsync</c>) — no forking required.
/// </summary>
public class ExtensibilityMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    /// <summary>
    /// A consumer-defined client that adds an endpoint the library does not ship,
    /// built entirely from protected helpers.
    /// </summary>
    private sealed class ExtendedBitbucketClient(string url, string username, string password)
        : BitbucketClient(url, username, password)
    {
        public Task<IReadOnlyList<Repository>> GetCustomReposAsync(
            string projectKey,
            int? maxPages = null,
            int? limit = null,
            int? start = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(projectKey);

            var queryParamValues = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["limit"] = limit,
                ["start"] = start,
            };

            return GetPagedAsync<Repository>(
                GetBaseUrl().AppendPathSegment($"/projects/{projectKey}/custom-repos"),
                queryParamValues,
                maxPages,
                cancellationToken);
        }
    }

    [Fact]
    public async Task Subclass_ReusesProtectedHelpers_ToCallCustomPagedEndpoint()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create()
                .WithPath($"/rest/api/1.0/projects/{TestConstants.TestProjectKey}/custom-repos")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"size\":2,\"limit\":25,\"isLastPage\":true,\"start\":0,\"values\":[{\"slug\":\"r1\"},{\"slug\":\"r2\"}]}"));

        using var client = new ExtendedBitbucketClient(
            _fixture.BaseUrl, TestConstants.TestUsername, TestConstants.TestPassword);

        var repos = await client.GetCustomReposAsync(TestConstants.TestProjectKey);

        Assert.Equal(2, repos.Count);
        Assert.Equal("r1", repos[0].Slug);
        Assert.Equal("r2", repos[1].Slug);
    }

    [Fact]
    public async Task Subclass_ReusesPagedResults_AcrossMultiplePages()
    {
        _fixture.Reset();

        // Page 1 (start=0) is not the last page and points to start=2.
        _fixture.Server
            .Given(Request.Create()
                .WithPath($"/rest/api/1.0/projects/{TestConstants.TestProjectKey}/custom-repos")
                .WithParam("start", "0")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"size\":2,\"limit\":2,\"isLastPage\":false,\"nextPageStart\":2,\"start\":0,\"values\":[{\"slug\":\"r1\"},{\"slug\":\"r2\"}]}"));

        // Page 2 (start=2) is the last page.
        _fixture.Server
            .Given(Request.Create()
                .WithPath($"/rest/api/1.0/projects/{TestConstants.TestProjectKey}/custom-repos")
                .WithParam("start", "2")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{\"size\":1,\"limit\":2,\"isLastPage\":true,\"start\":2,\"values\":[{\"slug\":\"r3\"}]}"));

        using var client = new ExtendedBitbucketClient(
            _fixture.BaseUrl, TestConstants.TestUsername, TestConstants.TestPassword);

        // Page through both pages, proving the protected GetPagedResultsAsync loop is reusable.
        var repos = await client.GetCustomReposAsync(TestConstants.TestProjectKey, start: 0);

        Assert.Equal(3, repos.Count);
        Assert.Equal(["r1", "r2", "r3"], repos.Select(r => r.Slug));
    }
}
