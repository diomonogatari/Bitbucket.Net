using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class ChangesAndFilesMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task GetChangesAsync_ReturnsChanges()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetChanges(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var changes = await client.GetChangesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            until: "HEAD");

        Assert.NotNull(changes);
        var changeList = changes.ToList();
        Assert.Single(changeList);
        Assert.Equal("MODIFY", changeList[0].Type);
    }

    [Fact]
    public async Task GetCommitChangesAsync_ReturnsChanges()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitChanges(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var changes = await client.GetCommitChangesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.NotNull(changes);
        var changeList = changes.ToList();
        Assert.Single(changeList);
    }

    [Fact]
    public async Task GetRepositoryFilesAsync_ReturnsFiles()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetFiles(TestConstants.TestProjectKey, TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var files = await client.GetRepositoryFilesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);

        Assert.NotNull(files);
        var fileList = files.ToList();
        Assert.Equal(3, fileList.Count);
        Assert.Contains("README.md", fileList);
    }

    [Fact]
    public async Task GetPullRequestChangesAsync_ReturnsChanges()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestChanges(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var changes = await client.GetPullRequestChangesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(changes);
        var changeList = changes.ToList();
        Assert.Single(changeList);
    }

    [Fact]
    public async Task GetPullRequestCommitsAsync_ReturnsCommits()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestCommits(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var commits = await client.GetPullRequestCommitsAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(commits);
        var commitList = commits.ToList();
        Assert.Equal(2, commitList.Count);
    }
}