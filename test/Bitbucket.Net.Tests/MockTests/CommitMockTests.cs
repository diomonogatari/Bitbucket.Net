using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class CommitMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetCommitsAsync_ReturnsCommits()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommits(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var commits = await client.GetCommitsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            until: "HEAD");

        Assert.NotNull(commits);
        var commitList = commits.ToList();
        Assert.Equal(2, commitList.Count);
        Assert.Contains(commitList, c => c.Message == "Initial commit");
        Assert.Contains(commitList, c => c.Message == "Add feature");
    }

    [Fact]
    public async Task GetCommitAsync_ReturnsCommit()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommit(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var commit = await client.GetCommitAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.NotNull(commit);
        Assert.Equal(TestConstants.TestCommitId, commit.Id);
        Assert.Equal("Initial commit", commit.Message);
    }
}