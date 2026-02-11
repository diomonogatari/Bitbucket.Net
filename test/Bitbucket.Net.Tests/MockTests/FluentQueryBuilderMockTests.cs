using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class FluentQueryBuilderMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    private BitbucketClient CreateClient()
    {
        _fixture.Reset();
        return _fixture.CreateClient();
    }

    private WireMock.Server.WireMockServer Server => _fixture.Server;

    [Fact]
    public async Task PullRequestQueryBuilder_DefaultParams_ReturnsPullRequests()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":1,"limit":25,"isLastPage":true,"values":[{"id":1,"title":"Test PR","state":"OPEN","open":true,"closed":false}]}"""));

        var result = await client.PullRequests("PRJ", "my-repo").GetAsync();

        Assert.NotEmpty(result);
        Assert.Equal("Test PR", result[0].Title);
    }

    [Fact]
    public async Task PullRequestQueryBuilder_WithAllOptions_AppliesQueryParams()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/pull-requests")
                .WithParam("state", "MERGED")
                .WithParam("order", "OLDEST")
                .WithParam("direction", "OUTGOING")
                .WithParam("at", "refs/heads/feature")
                .WithParam("limit", "50")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":0,"limit":50,"isLastPage":true,"values":[]}"""));

        var result = await client.PullRequests("PRJ", "my-repo")
            .InState(PullRequestStates.Merged)
            .OrderBy(PullRequestOrders.Oldest)
            .WithDirection(PullRequestDirections.Outgoing)
            .AtBranch("refs/heads/feature")
            .PageSize(50)
            .GetAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task PullRequestQueryBuilder_StreamAsync_YieldsItems()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/pull-requests")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":2,"limit":25,"isLastPage":true,"values":[{"id":1,"title":"PR 1","state":"OPEN","open":true,"closed":false},{"id":2,"title":"PR 2","state":"OPEN","open":true,"closed":false}]}"""));

        var items = new List<PullRequest>();
        await foreach (var pr in client.PullRequests("PRJ", "my-repo").StreamAsync())
        {
            items.Add(pr);
        }

        Assert.Equal(2, items.Count);
        Assert.Equal("PR 1", items[0].Title);
        Assert.Equal("PR 2", items[1].Title);
    }

    [Fact]
    public async Task CommitQueryBuilder_DefaultParams_ReturnsCommits()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/commits")
                .WithParam("until", "HEAD")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":1,"limit":25,"isLastPage":true,"values":[{"id":"abc123","message":"Initial commit"}]}"""));

        var result = await client.Commits("PRJ", "my-repo", "HEAD").GetAsync();

        Assert.NotEmpty(result);
        Assert.Equal("abc123", result[0].Id);
    }

    [Fact]
    public async Task CommitQueryBuilder_WithOptions_AppliesQueryParams()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/commits")
                .WithParam("until", "main")
                .WithParam("since", "v1.0")
                .WithParam("path", "src/file.cs")
                .WithParam("merges", "include")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":0,"limit":25,"isLastPage":true,"values":[]}"""));

        var result = await client.Commits("PRJ", "my-repo", "main")
            .Since("v1.0")
            .AtPath("src/file.cs")
            .Merges(MergeCommits.Include)
            .GetAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task BranchQueryBuilder_DefaultParams_ReturnsBranches()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/branches")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":1,"limit":25,"isLastPage":true,"values":[{"id":"refs/heads/main","displayId":"main","isDefault":true}]}"""));

        var result = await client.Branches("PRJ", "my-repo").GetAsync();

        Assert.NotEmpty(result);
        Assert.Equal("main", result[0].DisplayId);
    }

    [Fact]
    public async Task BranchQueryBuilder_WithOptions_AppliesQueryParams()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects/PRJ/repos/my-repo/branches")
                .WithParam("filterText", "feature")
                .WithParam("orderBy", "MODIFICATION")
                .WithParam("details", "true")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":0,"limit":25,"isLastPage":true,"values":[]}"""));

        var result = await client.Branches("PRJ", "my-repo")
            .FilterBy("feature")
            .OrderBy(BranchOrderBy.Modification)
            .WithDetails()
            .GetAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ProjectQueryBuilder_DefaultParams_ReturnsProjects()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":1,"limit":25,"isLastPage":true,"values":[{"key":"PRJ","name":"My Project"}]}"""));

        var result = await client.Projects().GetAsync();

        Assert.NotEmpty(result);
        Assert.Equal("PRJ", result[0].Key);
    }

    [Fact]
    public async Task ProjectQueryBuilder_WithNameFilter_AppliesQueryParam()
    {
        var client = CreateClient();
        Server
            .Given(Request.Create()
                .WithPath("/rest/api/1.0/projects")
                .WithParam("name", "Test")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithBody("""{"size":0,"limit":25,"isLastPage":true,"values":[]}"""));

        var result = await client.Projects()
            .NameFilter("Test")
            .GetAsync();

        Assert.Empty(result);
    }

    [Fact]
    public void PullRequestQueryBuilder_NullProjectKey_Throws()
    {
        var client = CreateClient();
        Assert.Throws<ArgumentNullException>(() => client.PullRequests(null!, "repo"));
    }

    [Fact]
    public void CommitQueryBuilder_NullUntil_Throws()
    {
        var client = CreateClient();
        Assert.Throws<ArgumentNullException>(() => client.Commits("PRJ", "repo", null!));
    }

    [Fact]
    public void BranchQueryBuilder_EmptyRepoSlug_Throws()
    {
        var client = CreateClient();
        Assert.Throws<ArgumentException>(() => client.Branches("PRJ", ""));
    }
}