using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class JiraMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;
    private const string ProjectKey = "PROJ";
    private const string RepoSlug = "repo";

    [Fact]
    public async Task GetJiraIssuesAsync_ReturnsIssues()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetJiraIssues(ProjectKey, RepoSlug, 1);
        var client = _fixture.CreateClient();

        var result = await client.GetJiraIssuesAsync(ProjectKey, RepoSlug, 1);

        Assert.NotNull(result);
        var issues = result.ToList();
        Assert.Equal(2, issues.Count);
        Assert.Equal("PROJ-123", issues[0].Key);
        Assert.Equal("https://jira.example.com/browse/PROJ-123", issues[0].Url);
        Assert.Equal("PROJ-456", issues[1].Key);
    }

    [Fact]
    public async Task CreateJiraIssueAsync_ReturnsCreatedIssue()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateJiraIssue(CommentId);
        var client = _fixture.CreateClient();

        var result = await client.CreateJiraIssueAsync(CommentId, "app-id", "Test Issue", "Bug");

        Assert.NotNull(result);
        Assert.Equal(100, result.CommentId);
        Assert.Equal("PROJ-789", result.IssueKey);
    }

    [Fact]
    public async Task GetChangeSetsAsync_ReturnsChangeSets()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetChangeSets("PROJ-123");
        var client = _fixture.CreateClient();

        var result = await client.GetChangeSetsAsync("PROJ-123");

        Assert.NotNull(result);
        var changeSets = result.ToList();
        Assert.Single(changeSets);
        Assert.NotNull(changeSets[0].ToCommit);
        Assert.Equal("def456abc789", changeSets[0].ToCommit.Id);
    }

    private const string CommentId = "100";
}