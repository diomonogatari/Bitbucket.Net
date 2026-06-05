using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

/// <summary>
/// Covers the typed <c>archived</c> filter added to the <c>/repos</c> search endpoint
/// (<see cref="ISearchOperations.GetRepositoriesAsync(RepositoryArchivedState, int?, int?, int?, string?, string?, Bitbucket.Net.Models.Core.Admin.Permissions?, bool, System.Threading.CancellationToken)"/>).
/// </summary>
public class RepositorySearchMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private const string ReposPath = "/rest/api/1.0/repos";
    private const string OnePageBody = "{\"size\":1,\"limit\":25,\"isLastPage\":true,\"start\":0,\"values\":[{\"slug\":\"r1\"}]}";

    private readonly BitbucketMockFixture _fixture = fixture;

    private void StubReposRequiringArchived(string archivedValue)
    {
        _fixture.Server
            .Given(Request.Create()
                .WithPath(ReposPath)
                .WithParam("archived", archivedValue)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(OnePageBody));
    }

    [Theory]
    [InlineData(RepositoryArchivedState.Active, "ACTIVE")]
    [InlineData(RepositoryArchivedState.Archived, "ARCHIVED")]
    [InlineData(RepositoryArchivedState.All, "ALL")]
    public async Task GetRepositoriesAsync_WithArchivedState_SendsExpectedArchivedParam(
        RepositoryArchivedState state, string expectedValue)
    {
        _fixture.Reset();
        // Stub matches only when the request carries archived=<expectedValue>.
        StubReposRequiringArchived(expectedValue);
        var client = _fixture.CreateClient();

        var repos = await client.GetRepositoriesAsync(state);

        Assert.Single(repos);
        Assert.Equal("r1", repos[0].Slug);
    }

    [Fact]
    public async Task GetRepositoriesStreamAsync_WithArchivedState_SendsExpectedArchivedParam()
    {
        _fixture.Reset();
        StubReposRequiringArchived("ARCHIVED");
        var client = _fixture.CreateClient();

        var repos = new List<Repository>();
        await foreach (var repo in client.GetRepositoriesStreamAsync(RepositoryArchivedState.Archived))
        {
            repos.Add(repo);
        }

        Assert.Single(repos);
        Assert.Equal("r1", repos[0].Slug);
    }

    [Fact]
    public async Task GetRepositoriesAsync_WithoutArchivedState_OmitsArchivedParam()
    {
        _fixture.Reset();
        _fixture.Server
            .Given(Request.Create()
                .WithPath(ReposPath)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithHeader("Content-Type", "application/json")
                .WithBody(OnePageBody));
        var client = _fixture.CreateClient();

        var repos = await client.GetRepositoriesAsync();

        Assert.Single(repos);
        // The base overload must not introduce an archived filter.
        var logEntry = Assert.Single(_fixture.Server.LogEntries);
        Assert.False(logEntry.RequestMessage.Query?.ContainsKey("archived") ?? false);
    }
}
