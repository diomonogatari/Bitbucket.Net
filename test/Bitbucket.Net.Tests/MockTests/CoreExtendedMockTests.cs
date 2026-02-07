using Bitbucket.Net.Models.Core.Projects;
using Bitbucket.Net.Tests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Bitbucket.Net.Tests.MockTests;

public class CoreExtendedMockTests(BitbucketMockFixture fixture) : IClassFixture<BitbucketMockFixture>
{
    private readonly BitbucketMockFixture _fixture = fixture;

    [Fact]
    public async Task BrowseProjectRepositoryAsync_ReturnsBrowseItem()
    {
        _fixture.Reset();
        _fixture.Server.SetupBrowseRepository(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var browseItem = await client.BrowseProjectRepositoryAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            at: "refs/heads/master");

        Assert.NotNull(browseItem);
    }

    [Fact]
    public async Task GetProjectRepositoryLastModifiedAsync_ReturnsLastModified()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetLastModified(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var lastModified = await client.GetProjectRepositoryLastModifiedAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            at: "refs/heads/master");

        Assert.NotNull(lastModified);
    }

    [Fact]
    public async Task GetRepositoryCompareChangesAsync_ReturnsChanges()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCompareChanges(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug);
        var client = _fixture.CreateClient();

        var changes = await client.GetRepositoryCompareChangesAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            from: "refs/heads/feature",
            to: "refs/heads/master");

        Assert.NotNull(changes);
        var changeList = changes.ToList();
        Assert.Single(changeList);
    }

    [Fact]
    public async Task GetCommitDiffAsync_ReturnsDifferences()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitDiff(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var diff = await client.GetCommitDiffAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.NotNull(diff);
        Assert.NotNull(diff.Diffs);
    }

    [Fact]
    public async Task GetPullRequestMergeBaseAsync_ReturnsCommit()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetPullRequestMergeBase(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);
        var client = _fixture.CreateClient();

        var commit = await client.GetPullRequestMergeBaseAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestPullRequestId);

        Assert.NotNull(commit);
        Assert.Equal(TestConstants.TestCommitId, commit.Id);
    }

    [Fact]
    public async Task CreateCommitWatchAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateCommitWatch(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var result = await client.CreateCommitWatchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCommitWatchAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteCommitWatch(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var result = await client.DeleteCommitWatchAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);

        Assert.True(result);
    }

    [Fact]
    public async Task CreateCommitCommentAsync_ReturnsComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupCreateCommitComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId);
        var client = _fixture.CreateClient();

        var commentInfo = new CommentInfo { Text = "Test comment" };

        var comment = await client.CreateCommitCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            commentInfo);

        Assert.NotNull(comment);
    }

    [Fact]
    public async Task GetCommitCommentAsync_ReturnsComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupGetCommitComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1);
        var client = _fixture.CreateClient();

        var comment = await client.GetCommitCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1);

        Assert.NotNull(comment);
    }

    [Fact]
    public async Task UpdateCommitCommentAsync_ReturnsComment()
    {
        _fixture.Reset();
        _fixture.Server.SetupUpdateCommitComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1);
        var client = _fixture.CreateClient();

        var commentText = new CommentText { Text = "Updated comment", Version = 0 };

        var comment = await client.UpdateCommitCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1,
            commentText);

        Assert.NotNull(comment);
    }

    [Fact]
    public async Task DeleteCommitCommentAsync_ReturnsTrue()
    {
        _fixture.Reset();
        _fixture.Server.SetupDeleteCommitComment(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1);
        var client = _fixture.CreateClient();

        var result = await client.DeleteCommitCommentAsync(
            TestConstants.TestProjectKey,
            TestConstants.TestRepositorySlug,
            TestConstants.TestCommitId,
            1,
            version: 0);

        Assert.True(result);
    }
}